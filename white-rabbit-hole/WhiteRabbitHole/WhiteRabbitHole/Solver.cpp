// Solver.cpp

#include "Solver.h"

using namespace challenge::whiterabbithole;

// Ctors

Solver::Solver(const std::string& anagram_phrase, const std::string& dbfile_path)
{
	this->anagram_phrase = anagram_phrase;
	this->dbfile_path = dbfile_path;
	this->words = 0;
}

Solver::Solver(const Solver& other)
{
	this->anagram_phrase = other.anagram_phrase;
	this->dbfile_path = other.dbfile_path;

	// Copy the state as well
	this->words = new wordset();
	*(this->words) = *(other.words);
	this->use_words = new wordset();
	*(this->use_words) = *(other.use_words);
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

void Solver::solve()
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
	unsigned int phrase_char_count = this->get_phrase_char_count();

	// Take dispositions of use_words in groups of words_count, so
	// check the length of the sentence keeping into account the spaces for the first check
	// and if positive, proceed with hash check

	// TODO
}

// Private methods

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
		this->words->insert(line);
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

	this->use_words = new wordset();

	for (wordset::const_iterator it = this->words->begin(); it != this->words->end(); it++)
	{
		if (this->accept_word(*it))
		{
			this->use_words->insert(*it);
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
}

unsigned int Solver::get_phrase_char_count() const
{
	return this->anagram_phrase.length();
}
