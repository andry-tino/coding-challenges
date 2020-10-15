// Histogram.cpp

#include "Histogram.h"

using namespace challenge::whiterabbithole;

// Ctors

Histogram::Histogram(const std::string& phrase)
{
	this->phrase = phrase;
	this->char_histo = 0;

	this->initialize();
}

Histogram::Histogram(const Histogram& other)
{
	this->phrase = other.phrase;

	this->char_histo = new histo_t();
	*(this->char_histo) = *(other.char_histo);

	this->initialize();
}

Histogram::~Histogram()
{
	if (this->char_histo)
	{
		this->char_histo->clear();
		delete this->char_histo;
	}
}

// Public methods

bool Histogram::equals(const Histogram& histogram1, const Histogram& histogram2)
{
	return eval_from_to(histogram1, histogram2, eval_equal, true) &&
		eval_from_to(histogram2, histogram1, eval_equal, true);
}

bool Histogram::contains(const Histogram& histogram1, const Histogram& histogram2)
{
	return eval_from_to(histogram1, histogram2, eval_geq, false) &&
		eval_from_to(histogram2, histogram1, eval_leq, false);
}

bool Histogram::operator==(const Histogram& other) const
{
	return equals(*this, other);
}

bool Histogram::operator!=(const Histogram& other) const
{
	return !equals(*this, other);
}

bool Histogram::operator>=(const Histogram& other) const
{
	return contains(*this, other);
}

bool Histogram::operator<=(const Histogram& other) const
{
	return contains(other, *this);
}

// Private methods

// At construction time we generate the histo data structure
void Histogram::initialize()
{
	if (this->char_histo)
	{
		this->char_histo->clear();
		delete this->char_histo;
	}
	this->char_histo = new histo_t();

	for (size_t i = 0, l = this->phrase.length(); i < l; i++)
	{
		char char_at = this->phrase.at(i);
		if (this->char_histo->find(char_at) == this->char_histo->end())
		{
			(*(this->char_histo))[char_at] = 0;
		}
		(*(this->char_histo))[char_at] = (*(this->char_histo))[char_at] + 1;
	}
}

bool Histogram::eval_from_to(const Histogram& histogram1, const Histogram& histogram2,
	bool (*eval)(size_t v1, size_t v2), bool exact_alphabet)
{
	for (histo_t::const_iterator it = histogram1.char_histo->begin(); it != histogram1.char_histo->end(); it++)
	{
		char char_at_1 = it->first;
		size_t level_at_1 = it->second;

		histo_t::const_iterator at_2 = histogram2.char_histo->find(char_at_1);
		if (at_2 == histogram2.char_histo->end())
		{
			if (exact_alphabet) return false;
			continue;
		}
		size_t level_at_2 = at_2->second;
		bool eval_res = eval(level_at_1, level_at_2);
		if (!eval_res)
		{
			return false;
		}
	}

	return true;
}

bool Histogram::eval_equal(size_t v1, size_t v2)
{
	return v1 == v2;
}

bool Histogram::eval_geq(size_t v1, size_t v2)
{
	return v1 >= v2;
}

bool Histogram::eval_leq(size_t v1, size_t v2)
{
	return v1 <= v2;
}
