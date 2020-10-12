// Hashing.h

#ifndef HASHING_H_
#define HASHING_H_

#include <string>

namespace challenge {
	namespace whiterabbithole {

		/// <summary>
		/// Computes the hash of a string.
		/// </summary>
		/// <param name="str">The input string.</param>
		/// <returns>The MD5 hash (string representation).</returns>
		std::string get_hash(const std::string& str);

		/// <summary>
		/// Compares the hashes of two strings.
		/// </summary>
		/// <param name="str1">The first string.</param>
		/// <param name="str2">The second string.</param>
		/// <returns>A value indicating whether the hashes are the same or not.</returns>
		bool compare_hashes(const std::string& str1, const std::string& str2);

	} // namespace whiterabbithole
} // namespace challenge

#endif
