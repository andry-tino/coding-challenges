// Solver.cpp

#include "Solver.h"

using namespace challenge::whiterabbithole;

// Ctors

Solver::Solver(const std::string& anagram_phrase, const std::string& dbfile_path, std::ostream* log_stream)
{
	this->anagram_phrase = anagram_phrase;
	this->dbfile_path = dbfile_path;
	this->log_stream = log_stream;
	this->words = 0;
	this->use_words = 0;
}

Solver::Solver(const Solver& other)
{
	this->anagram_phrase = other.anagram_phrase;
	this->dbfile_path = other.dbfile_path;

	// Copy the state as well
	this->words = new wordset();
	if (other.words)
	{
		*(this->words) = *(other.words);
	}
	
	this->use_words = new usewordset();
	if (other.use_words)
	{
		*(this->use_words) = *(other.use_words);
	}
}

Solver::~Solver()
{
	if (this->words)
	{
		delete this->words;
	}

	if (this->use_words)
	{
		delete this->use_words;
	}
}

// Public methods

const Solver::result_t& Solver::solve()
{
	if (!this->use_words)
	{
		if (!this->words)
		{
			// Load the db file on demand only once
			this->load_words();
		}

		// Extract the usewords from words
		this->process_words();
	}

	unsigned int words_count = this->get_phrase_words_count();

	// Take dispositions of use_words in groups of words_count, so
	// check the length of the sentence keeping into account the spaces for the first check
	// and if positive, proceed with hash check
	DispositionsTreeWalkState* state = new DispositionsTreeWalkState();
	result_t result;
	this->dispositions_use_words(words_count, state, &result);
	delete state;

	return result;
}

// Private methods

void Solver::log(const std::string& what) const {
	if (!this->log_stream)
	{
		return;
	}

	*(this->log_stream) << what;
}

bool Solver::check_dbfile_path() const
{
	return std::ifstream(this->dbfile_path).good();
}

void Solver::load_words()
{
	if (!this->check_dbfile_path())
	{
		throw std::exception("Invalid dbfile path");
	}

	// Check the set of words has not previously created, in which case delete
	if (this->words)
	{
		delete this->words;
	}

	std::ifstream dbfile(this->dbfile_path);
	if (!dbfile.is_open())
	{
		throw std::exception("Could not open dbfile");
	}

	this->words = new wordset();

	std::string line;
	while (std::getline(dbfile, line))
	{
		if (line.length() == 0)
		{
			continue;
		}
		this->words->push_back(line);
	}

	dbfile.close();
}

void Solver::process_words()
{
	// In order to proceed, we need the word set to be filled
	if (!this->words || this->words->size() == 0)
	{
		throw std::exception("No words available, cannot proceed processing words");
	}

	// Check the set of usewords has not previously created, in which case delete
	if (this->use_words)
	{
		delete this->use_words;
	}

	this->use_words = new usewordset();

	// For each word, include it in use_words only if all its characters are all
	// contained in the anagram phrase
	for (wordset::const_iterator it = this->words->begin(); it != this->words->end(); it++)
	{
		if (this->accept_word(*it))
		{
			this->use_words->push_back(*it);
		}
	}
}

bool Solver::accept_word(const std::string& word) const
{
	for (std::string::const_iterator it = word.begin(); it != word.end(); it++)
	{
		if (this->anagram_phrase.find(*it) == std::string::npos)
		{
			// Found a character in word that is not present in anagram_phrase
			return false;
		}
	}

	return true;
}

unsigned int Solver::get_phrase_words_count() const
{
	return std::count(
		this->anagram_phrase.begin(),
		this->anagram_phrase.end(),
		' ') + 1;
}

unsigned int Solver::get_phrase_char_count() const
{
	return this->anagram_phrase.length();
}

void Solver::dispositions_use_words(
	unsigned int group_size,
	const DispositionsTreeWalkState* state,
	result_t* result) const
{
	if (state->get_disposition()->size() == group_size)
	{
		// Process this disposition
		this->run_disposition(group_size, state, result);

		return;
	}

	// A valid disposition has to be created as state contains an incomplete one
	// Get residual array: all the indices not contained in state->disposition
	const DispositionsTreeWalkState::disposition_t& residuals =
		this->get_residual_indices(*(state->get_disposition()), group_size);

	for (
		DispositionsTreeWalkState::disposition_t::const_iterator it = residuals.begin();
		it != residuals.end();
		it++)
	{
		// Add the residual to the disposition
		state->push_to_disposition(*it);

		// Recursively process the new disposition
		this->dispositions_use_words(group_size, state, result);

		// Remove the residual as not needed anymore
		state->pop_from_disposition();
	}
}

void Solver::run_disposition(
	unsigned int group_size,
	const DispositionsTreeWalkState* state,
	result_t* result) const
{
	// Build the try-phrase
	phrase_t try_phrase;
	unsigned int interspace_count = group_size - 1;
	unsigned int try_phrase_len = interspace_count; // sum of chars in all elements + interspaces
	for (
		DispositionsTreeWalkState::disposition_t::const_iterator it = state->get_disposition()->begin();
		it != state->get_disposition()->end();
		it++)
	{
		unsigned int index = *it;
		std::string word = (*(this->use_words))[1];
		try_phrase.push_back(word);
		try_phrase_len += word.length();
	}

	// Try the try-phrase
	// 1. Check the phrase length first
	// 2. If the length matches, move to the hash check
	unsigned int phrase_len = this->get_phrase_char_count();
	if (try_phrase_len == phrase_len)
	{
		// Candidate, proceed with hash check
		if (true)
		{
			// Valid, add it among result
			result->push_back(try_phrase);
		}
	}
}

const DispositionsTreeWalkState::disposition_t& Solver::get_residual_indices(
	const DispositionsTreeWalkState::disposition_t& disposition, unsigned int group_size) const
{
	DispositionsTreeWalkState::disposition_t ret_disposition;

	// Example: disposition = [2,3], group_size = 3 => ret = [0, 1]
	for (unsigned int i = 0; i < group_size; i++)
	{
		if (std::find(disposition.begin(), disposition.end(), i) != disposition.end())
		{
			ret_disposition.push_back(i);
		}
	}

	return ret_disposition;
}
