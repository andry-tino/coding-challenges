// Utils.cpp

#include "Utils.h"

std::string challenge::whiterabbithole::disposition_to_string(const std::vector<unsigned int>& disposition)
{
	if (disposition.size() == 0)
	{
		return std::string("''");
	}

	std::string s;
	for (std::vector<unsigned int>::const_iterator it = disposition.begin(); it != disposition.end(); it++)
	{
		s = s + std::to_string((*it)) + ((it+1 == disposition.end()) ? "" : ",");
	}

	return s;
}

std::string challenge::whiterabbithole::phrase_to_string(const std::vector<std::string>& phrase)
{
	if (phrase.size() == 0)
	{
		return std::string("''");
	}

	std::string s;
	for (std::vector<std::string>::const_iterator it = phrase.begin(); it != phrase.end(); it++)
	{
		s = s + (*it) + ((it + 1 == phrase.end()) ? "" : ",");
	}

	return s;
}
