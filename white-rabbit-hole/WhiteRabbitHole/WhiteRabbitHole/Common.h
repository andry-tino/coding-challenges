// Common.h

#ifndef COMMON_H_
#define COMMON_H_

#include <string>
#include <vector>

namespace challenge {
	namespace whiterabbithole {

		/// <summary>
		///  Represents a phrase.
		/// </summary>
		typedef std::vector<std::string> phrase_t;

		/// <summary>
		/// Represents an alphabest of symbols that can be encountered in words.
		/// </summary>
		typedef std::vector<char> alphabet_t;

	} // namespace whiterabbithole
} // namespace challenge

#endif
