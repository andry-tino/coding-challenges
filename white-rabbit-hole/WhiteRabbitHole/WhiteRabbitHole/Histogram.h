// Histogram.h

#ifndef HISTOGRAM_H_
#define HISTOGRAM_H_

#include <string>

#include "Common.h"
#include "Solver.h"

namespace challenge {
	namespace whiterabbithole {

		/// <summary>
		/// Describes an histogram for strings.
		/// </summary>
		class Histogram
		{
		public:
			Histogram(const phrase_t& phrase);
			Histogram(const Histogram& other);
			~Histogram();

		private:
			phrase_t phrase;
			std::map<char, size_t> char_histo;

		public:
			static bool compare(const Histogram& histogram1, const Histogram& histogram2);
			bool operator==(const Histogram& other) const;
			bool operator!=(const Histogram& other) const;

		private:
			void initialize();
		};

	} // namespace whiterabbithole
} // namespace challenge

#endif
