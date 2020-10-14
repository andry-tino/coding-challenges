// Histogram.cpp

#include "Histogram.h"

using namespace challenge::whiterabbithole;

// Ctors

Histogram::Histogram(const phrase_t& phrase)
{
	this->phrase = phrase;
	this->char_histo = std::map<char, size_t>();

	this->initialize();
}

Histogram::Histogram(const Histogram& other)
{
	this->phrase = other.phrase;
	this->char_histo = other.char_histo;

	this->initialize();
}

Histogram::~Histogram()
{
}

// Public methods

bool Histogram::compare(const Histogram& histogram1, const Histogram& histogram2)
{
	return false;
}

bool Histogram::operator==(const Histogram& other) const
{
	return compare(*this, other);
}

bool Histogram::operator!=(const Histogram& other) const
{
	return !compare(*this, other);
}

// Private methods

void Histogram::initialize()
{

}
