// Utils.h

#ifndef UTILS_H_
#define UTILS_H_

#include <iostream>
#include <string>
#include <vector>

namespace challenge {
	namespace whiterabbithole {

		/// <summary>
		/// Gets a string representation of a disposition.
		/// </summary>
		/// <param name="disposition"></param>
		/// <returns></returns>
		std::string disposition_to_string(const std::vector<unsigned int>& disposition);

		/// <summary>
		/// Gets a string representation of a phrase
		/// </summary>
		/// <param name="disposition"></param>
		/// <returns></returns>
		std::string phrase_to_string(const std::vector<std::string>& disposition);

	} // namespace challenge
} // namespace whiterabbithole

#endif
