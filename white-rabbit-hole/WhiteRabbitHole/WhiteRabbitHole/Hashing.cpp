// Hashing.cpp

#define CRYPTOPP_ENABLE_NAMESPACE_WEAK 1 // Needed to use MD5 in Crypto++

#include <cryptlib.h>
#include <md5.h>

#include "Hashing.h"

using namespace CryptoPP;

std::string challenge::whiterabbithole::get_hash(const std::string& str)
{
	std::string digest;
	Weak::MD5 hash;

	hash.Update((const byte*)(str.data()), str.size());
	digest.resize(hash.DIGESTSIZE);
	hash.Final((byte*)&digest[0]);

	return digest;
}

bool challenge::whiterabbithole::compare_hashes(const std::string& str1, const std::string& str2)
{
	std::string hash1 = get_hash(str1);
	std::string hash2 = get_hash(str2);

	return hash1.compare(hash2) == 0;
}
