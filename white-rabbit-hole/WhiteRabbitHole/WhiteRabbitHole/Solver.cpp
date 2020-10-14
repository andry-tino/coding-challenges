// Solver.cpp

#include <exception>
#include <algorithm>

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
	this->words = 0;
	this->use_words = 0;
	this->result = 0;
}

Solver::Solver(const Solver& other)
{
	this->anagram_phrase = other.anagram_phrase;
	this->dbfile_path = other.dbfile_path;

	// Copy the state as well
	this->words = new wordset_t();
	if (other.words)
	{
		*(this->words) = *(other.words);
	}
	
	this->use_words = new usewordset_t();
	if (other.use_words)
	{
		*(this->use_words) = *(other.use_words);
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

		// Extract the usewords from words
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
		delete this->use_words;
	}

	this->use_words = new usewordset_t();

	// For each word, include it in use_words only if:
	// 1. The word has length matching either of the words in the anagram phrase
	//        - The anagram phrase shows the exact number of words in the original
	// 2. All its characters are all contained in the anagram phrase
	for (wordset_t::const_iterator it = this->words->begin(); it != this->words->end(); it++)
	{
		if (this->accept_word(*it))
		{
			this->use_words->push_back(*it);
		}
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
		if (!walkCombinationsOnly || (walkCombinationsOnly && !state->is_disposition_in_cache()))
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
	unsigned int interspace_count = group_size - 1;
	unsigned int try_phrase_len = interspace_count; // sum of chars in all elements + interspaces
	for (
		DispositionsTreeWalkState::disposition_t::const_iterator it = state->get_disposition()->begin();
		it != state->get_disposition()->end();
		it++)
	{
		unsigned int index = *it;
		std::string word = usewordset.at(index);
		try_phrase.push_back(word);
		try_phrase_len += word.length();
	}

	// Try the try-phrase
	// 1. Check the phrase length first
	// 2. If the length matches, move to the hash check
	unsigned int phrase_len = this->get_phrase_char_count();
	DispositionRunResult run_result = DispositionRunResult::No;
	if (try_phrase_len == phrase_len)
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
	this->dispositions_cache = new dispositions_cache_t();
}

DispositionsTreeWalkState::DispositionsTreeWalkState(const DispositionsTreeWalkState& other)
{
	this->disposition = new disposition_t();
	*(this->disposition) = *(other.disposition);

	this->dispositions_cache = new dispositions_cache_t();
	*(this->dispositions_cache) = *(other.dispositions_cache);
}

DispositionsTreeWalkState::~DispositionsTreeWalkState()
{
	if (this->disposition)
	{
		this->disposition->clear();
		delete this->disposition;
	}

	if (this->dispositions_cache)
	{
		this->dispositions_cache->clear();
		delete this->dispositions_cache;
	}
}

// Public methods

const DispositionsTreeWalkState::disposition_t* DispositionsTreeWalkState::get_disposition() const
{ 
	return this->disposition; // TODO: return const reference
}

bool DispositionsTreeWalkState::is_disposition_in_cache() const
{
	return this->is_disposition_in_cache(*(this->get_disposition()));
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
	// Must happen in pop, before actually popping because the current disposition will be added to cache.
	// If we do in push, then we would have the disposition already in cache before processing it
	this->add_to_cache();

	this->disposition->pop_back();
}

// Private methods

void DispositionsTreeWalkState::add_to_cache() const
{
	if (!this->use_cache)
	{
		return;
	}

	// Rebuild the disposition sorting ascending, this is because we
	// want to use the cache to store combinations
	disposition_t sorted_disposition = *(this->get_disposition()); // Copy
	this->sort_disposition(sorted_disposition);

	// Add to the dictionary to keep track
	std::pair<std::string, bool> cache_val =
		std::pair<std::string, bool>(this->get_disposition_str(sorted_disposition), true);
	std::pair<std::map<std::string, bool>::iterator, bool> insert_res =
		this->dispositions_cache->insert(cache_val);

	if (!insert_res.second)
	{
		// Was not inserted because already present
	}
}

bool DispositionsTreeWalkState::is_disposition_in_cache(const disposition_t& disposition) const
{
	// Rebuild the disposition sorting ascending, this is because we
	// want to use the cache to store combinations
	disposition_t sorted_disposition = disposition; // Copy
	this->sort_disposition(sorted_disposition);

	return this->dispositions_cache->find(disposition_to_string(sorted_disposition)) != this->dispositions_cache->end();
}

std::string DispositionsTreeWalkState::get_disposition_str(const disposition_t& disposition) const
{
	return disposition_to_string(disposition);
}

std::string DispositionsTreeWalkState::get_disposition_str() const
{
	return disposition_to_string(*(this->get_disposition()));
}

void DispositionsTreeWalkState::sort_disposition(disposition_t& disposition) const
{
	std::sort(disposition.begin(), disposition.end());
}
