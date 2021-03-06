// Solver.cpp

#include <exception>
#include <algorithm>
#include <unordered_map>

#include "Solver.h"
#include "Utils.h"
#include "Hashing.h"

using namespace challenge::whiterabbithole;

// --- Solver --- //

// Ctors

Solver::Solver(const std::string& anagram_phrase, const std::string& dbfile_path,
	const std::string& phrase_hash, std::ostream& log_stream)
{
	this->anagram_phrase = anagram_phrase;
	this->dbfile_path = dbfile_path;
	this->log_stream = &log_stream;
	this->phrase_hash = phrase_hash;
	this->anagram_phrase_histo = new Histogram(this->anagram_phrase);
	this->words = 0;
	this->use_words = 0;
	this->alphabet = 0;
	this->result = 0;
}

Solver::Solver(const Solver& other)
{
	this->anagram_phrase = other.anagram_phrase;
	this->anagram_phrase_histo = new Histogram(this->anagram_phrase);
	this->dbfile_path = other.dbfile_path;

	// Copy the state as well
	this->words = new wordset_t();
	if (other.words)
	{
		*(this->words) = *(other.words);
	}

	if (other.log_stream)
	{
		this->log_stream = other.log_stream;
	}
	
	this->use_words = new usewordset_t();
	if (other.use_words)
	{
		*(this->use_words) = *(other.use_words);
	}

	this->alphabet = new alphabet_t();
	if (other.alphabet)
	{
		*(this->alphabet) = *(other.alphabet);
	}

	this->result = new result_t();
	if (other.result)
	{
		*(this->result) = *(other.result);
	}
}

Solver::~Solver()
{
	if (this->words)
	{
		this->words->clear();
		delete this->words;
	}

	if (this->use_words)
	{
		this->use_words->clear();
		delete this->use_words;
	}

	if (this->alphabet)
	{
		this->alphabet->clear();
		delete this->alphabet;
	}

	if (this->result)
	{
		result->clear();
		delete this->result;
	}
}

// Public methods

void Solver::solve()
{
	// Ensure resources are loaded.
	this->load_all_res();

	unsigned int words_count = this->get_phrase_words_count();

	// When true, this will use a 2-phase approach:
	// 1. Evaluate combinations of words (not dispositions). In this phase, the words
	//    are checked and the histograms as well.
	// 2. Evaluate all dispositions of the found combinations and check hash.
	//    This is an exhaustive approach on a very small set of words as we scan
	//    dispositions of single combinations.
	bool use_combinations = true;

	// Take combinations of use_words in groups of words_count, so
	// check the length of the sentence keeping into account the spaces
	DispositionsTreeWalkState* state = new DispositionsTreeWalkState();
	result_t result_combinations; // If result_combinations == true => those are dispositions
	this->log("Executing searching candidates...");
	this->walk_dispositions(*(this->use_words), words_count, state, &result_combinations, use_combinations);
	this->log("Candidate search job done!");
	delete state;
	size_t result_combinations_size = result_combinations.size();
	this->log("Found " + std::to_string(result_combinations_size) + " candidates!");

	if (!use_combinations)
	{
		*(this->result) = result_combinations;
		return;
	}

	// Take dispositions of the found valid phrases and check hash
	result_t result_dispositions;
	this->log("Executing searching valid dispositions (from each candidate combination)...");
	unsigned int i = 1;
	for (result_t::const_iterator it = result_combinations.begin(), end = result_combinations.end(); it != end; it++, i++)
	{
		this->log("Running dispositions on combination: " + phrase_to_string(*it) + " - " +
			std::to_string(i) + "/" + std::to_string(result_combinations_size)); // Verbose
		state = new DispositionsTreeWalkState();
		this->walk_dispositions(*it, words_count, state, &result_dispositions, false); // No caching = all dispositions
		delete state;
	}
	this->log("Valid dispositions search job done!");

	*(this->result) = result_dispositions;
}

void Solver::load_all_res()
{
	if (!this->use_words)
	{
		if (!this->words)
		{
			// Load the db file on demand only once
			this->log("Loading words...");
			this->load_words();
			this->log("Words loaded: " + std::to_string(this->words->size()));
		}

		// Extract the usewords from words (and also handle the alphabet)
		this->log("Processing words...");
		this->process_words();
		this->log("Usewords loaded: " + std::to_string(this->use_words->size()));
	}

	if (this->use_words->size() <= 350)
	{
		unsigned int disposition_count = this->get_disposition_count(this->get_phrase_words_count());
		this->log("Number of dipositions to try: " + std::to_string(disposition_count));
	}

	if (this->result) this->result->clear();
	else this->result = new result_t();

	std::string alphabet_str;
	for (alphabet_t::const_iterator it = this->alphabet->begin(); it != this->alphabet->end(); it++)
	{
		alphabet_str += std::string(1, *it) + " ";
	}
	this->log("Alphabet: " + alphabet_str);

	bool verbose = false;
	if (verbose)
	{
		this->log("Use words are (first 100):");
		unsigned int i = 100;
		for (usewordset_t::const_iterator it = this->use_words->begin(); it != this->use_words->end(); it++)
		{
			if (i-- == 0) break;
			this->log("- " + (*it));
		}
	}
}

void Solver::print_result(std::ostream& stream) const
{
	for (result_t::const_iterator it = this->result->begin(); it != this->result->end(); it++)
	{
		stream << "- " << phrase_to_string(*it) << std::endl;
	}
}

// Private methods

void Solver::log(const std::string& what) const {
	if (!this->log_stream)
	{
		return;
	}

	*(this->log_stream) << what << std::endl;
}

unsigned int Solver::get_disposition_count(unsigned int group_size) const
{
	unsigned int use_words_count = this->use_words->size();
	unsigned int count = this->use_words->size();
	for (unsigned int i = 1; i < group_size; i++)
	{
		count = count * (use_words_count - i);
	}

	return count;
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

	this->words = new wordset_t();

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
		this->use_words->clear();
		delete this->use_words;
	}
	this->use_words = new usewordset_t(); // Reset (in case)

	// Here we also build the alphabet
	std::unordered_map<char, bool> alphabet_map;
	// Keep track of added words to avoid duplicates
	std::unordered_map<std::string, bool> use_words_map;

	// For each word, include it in use_words only if all its characters
	// are contained in the anagram phrase
	for (wordset_t::const_iterator it = this->words->begin(); it != this->words->end(); it++)
	{
		if (use_words_map.find(*it) != use_words_map.end())
		{
			// Duplicate found => Discard it
			continue;
		}
		use_words_map[*it] = true;

		// Not a duplicate => Consider it
		if (this->accept_word(*it))
		{
			this->use_words->push_back(*it);

			// Process the word to extract its symbols and add them to the alphabet
			for (size_t i = 0, l = it->length(); i < l; i++)
			{
				alphabet_map[it->at(i)] = true;
			}
		}
	}
	use_words_map.clear(); // Not needed anymore

	// Check the alphabet has not previously created, in which case delete
	if (this->alphabet)
	{
		this->alphabet->clear();
		delete this->alphabet;
	}
	this->alphabet = new alphabet_t(); // Reset (in case)
	// Extract the vector of symbols for the alphabet
	for (std::unordered_map<char, bool>::const_iterator it = alphabet_map.begin(); it != alphabet_map.end(); it++)
	{
		this->alphabet->push_back(it->first);
	}
}

std::vector<std::string> Solver::get_words_in_phrase(const std::string& phrase) const
{
	size_t pos = 0;
	std::string word;
	std::string s = phrase;
	std::string delimiter = " ";
	std::vector<std::string> result;
	while ((pos = s.find(delimiter)) != std::string::npos) {
		word = s.substr(0, pos);
		result.push_back(word);
		s.erase(0, pos + delimiter.length());
	}

	return result;
}

bool Solver::accept_word(const std::string& word) const
{
	// Check that every character in the word is present in the anagram phrase
	for (std::string::const_iterator it = word.begin(); it != word.end(); it++)
	{
		if (this->anagram_phrase.find(*it) == std::string::npos)
		{
			// Found a character in word that is not present in anagram_phrase
			return false;
		}
	}

	// If the character check passes, then consider the histogram
	return *(this->anagram_phrase_histo) >= Histogram(word);
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

void Solver::walk_dispositions(
	const usewordset_t& usewordset,
	unsigned int group_size,
	const DispositionsTreeWalkState* state,
	result_t* result,
	bool walkCombinationsOnly) const
{
	if (state->get_disposition()->size() == group_size)
	{
		// Process this disposition as this is a complete disposition (leaf in the recursion-tree)
		DispositionRunResult run_result = DispositionRunResult::Skipped;
		// Ordered disposition processing allows us to evaluate only combinations of words. It is the
		// same concepts as caching, but caching will be really memory-intensive
		if (!walkCombinationsOnly || (walkCombinationsOnly && state->is_disposition_ordered()))
		{
			run_result = this->run_disposition(usewordset, group_size, state, result, !walkCombinationsOnly);
		}

		bool verbose = false;
		if (verbose || (run_result != DispositionRunResult::No && run_result != DispositionRunResult::Skipped))
			this->log("Disposition: " +
			DispositionsTreeWalkState::get_disposition_words_str(*(state->get_disposition()), usewordset) +
			" - " + disposition_to_string(*(state->get_disposition())));

		if (run_result == DispositionRunResult::Candidate)
		{
			this->log("|- Candidate");
		}
		else if (run_result == DispositionRunResult::Valid)
		{
			this->log("|- Valid => !!FOUND ONE!!");
		}
		else if (verbose && run_result == DispositionRunResult::Skipped)
		{
			this->log("|- Skipped");
		}

		return;
	}

	// A valid disposition has to be created as state contains an incomplete one
	// Get residual array: all the indices not contained in state->disposition
	DispositionsTreeWalkState::disposition_t residuals =
		this->get_residual_indices(usewordset, *(state->get_disposition()));

	for (
		DispositionsTreeWalkState::disposition_t::const_iterator it = residuals.begin();
		it != residuals.end();
		it++)
	{
		// Add the residual to the disposition
		state->push_to_disposition(*it);

		// Recursively process the new disposition
		this->walk_dispositions(usewordset, group_size, state, result, walkCombinationsOnly);

		// Remove the residual as not needed anymore
		state->pop_from_disposition();
	}
}

Solver::DispositionRunResult Solver::run_disposition(
	const usewordset_t& usewordset,
	unsigned int group_size,
	const DispositionsTreeWalkState* state,
	result_t* result,
	bool checkValid) const
{
	// Build the try-phrase
	phrase_t try_phrase;
	for (
		DispositionsTreeWalkState::disposition_t::const_iterator it = state->get_disposition()->begin();
		it != state->get_disposition()->end();
		it++)
	{
		unsigned int index = *it;
		std::string word = usewordset.at(index);
		try_phrase.push_back(word);
	}

	// Try the try-phrase
	// 1. Check the phrase length first
	// 2. If the length matches, move to the hash check
	DispositionRunResult run_result = DispositionRunResult::No;
	if (this->is_phrase_candidate(try_phrase))
	{
		// Candidate, proceed with hash check
		run_result = DispositionRunResult::Candidate;

		if (checkValid && this->check_phrase_hash(try_phrase))
		{
			run_result = DispositionRunResult::Valid;
			result->push_back(try_phrase); // Result to contain all valids
		}
		else if (!checkValid) // We just want to analyze the candidates
		{
			result->push_back(try_phrase); // Result to contain all candidates
		}
	}

	return run_result;
}

bool Solver::is_phrase_candidate(const phrase_t& phrase) const
{
	return this->phrase_to_string(phrase).length() == this->get_phrase_char_count() &&
		Histogram(this->phrase_to_string(phrase)) == *(this->anagram_phrase_histo);
}

bool Solver::check_phrase_hash(const phrase_t& phrase) const
{
	return compare_hashes(
		this->phrase_hash,
		get_hash(this->phrase_to_string(phrase)));
}

std::string Solver::phrase_to_string(const phrase_t& phrase) const
{
	std::string str_phrase;
	for (phrase_t::const_iterator it = phrase.begin(); it != phrase.end(); it++)
	{
		str_phrase += ((*it) + ((it+1 == phrase.end()) ? "" : " "));
	}

	return str_phrase;
}

DispositionsTreeWalkState::disposition_t Solver::get_residual_indices(
	const usewordset_t& usewordset,
	const DispositionsTreeWalkState::disposition_t& disposition) const
{
	unsigned int usewordset_count = usewordset.size();
	DispositionsTreeWalkState::disposition_t ret_disposition;

	// Example: disposition = [2,3], group_size = 3 => ret = [0, 1]
	for (unsigned int i = 0; i < usewordset_count; i++)
	{
		if (std::find(disposition.begin(), disposition.end(), i) == disposition.end())
		{
			ret_disposition.push_back(i);
		}
	}

	return ret_disposition;
}

// --- DispositionsTreeWalkState --- //

// Ctors

DispositionsTreeWalkState::DispositionsTreeWalkState(bool use_cache)
{
	this->use_cache = use_cache;
	this->disposition = new disposition_t();
}

DispositionsTreeWalkState::DispositionsTreeWalkState(const DispositionsTreeWalkState& other)
{
	this->disposition = new disposition_t();
	*(this->disposition) = *(other.disposition);
}

DispositionsTreeWalkState::~DispositionsTreeWalkState()
{
	if (this->disposition)
	{
		this->disposition->clear();
		delete this->disposition;
	}
}

// Public methods

const DispositionsTreeWalkState::disposition_t* DispositionsTreeWalkState::get_disposition() const
{ 
	return this->disposition; // TODO: return const reference
}

bool DispositionsTreeWalkState::is_disposition_ordered() const
{
	return this->is_disposition_ordered(*(this->get_disposition()));
}

void DispositionsTreeWalkState::push_to_disposition(unsigned int index) const
{
	this->disposition->push_back(index);
}

std::string DispositionsTreeWalkState::get_disposition_words_str(const disposition_t& disposition,
	const std::vector<std::string>& words)
{
	std::vector<std::string> disposition_words;
	for (disposition_t::const_iterator it = disposition.begin(); it != disposition.end(); it++)
	{
		disposition_words.push_back(words.at(*it));
	}

	return phrase_to_string(disposition_words);
}

void DispositionsTreeWalkState::pop_from_disposition() const
{
	this->disposition->pop_back();
}

// Private methods

// Ordering mst be ascending, fx: 2,4,7
bool DispositionsTreeWalkState::is_disposition_ordered(const disposition_t& disposition) const
{
	if (disposition.size() <= 1)
	{
		return true;
	}

	// Guaranteed at least two elements from here on
	for (disposition_t::const_iterator it = disposition.begin()+1; it != disposition.end(); it++)
	{
		if (!(*it > *(it - 1)))
		{
			return false;
		}
	}

	return true;
}

std::string DispositionsTreeWalkState::get_disposition_str(const disposition_t& disposition) const
{
	return disposition_to_string(disposition);
}

std::string DispositionsTreeWalkState::get_disposition_str() const
{
	return disposition_to_string(*(this->get_disposition()));
}
