// Hashing.cpp

#include <ctype.h>

#include "Hashing.h"
#include "MD5.h"

std::string challenge::whiterabbithole::get_hash(const std::string& str)
{
	std::string digest = md5(str);

	return digest;
}

bool challenge::whiterabbithole::compare_hashes(const std::string& hash1, const std::string& hash2)
{
	if (hash1.length() != hash2.length())
	{
		return false;
	}

	for (int i = 0; i < hash1.length(); i++)
	{
		if (toupper(hash1.at(i)) != toupper(hash2.at(i)))
		{
			return false;
		}
	}

	return true;
}
