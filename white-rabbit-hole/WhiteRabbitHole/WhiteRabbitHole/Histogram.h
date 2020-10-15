// Histogram.h

#ifndef HISTOGRAM_H_
#define HISTOGRAM_H_

#include <string>
#include <map>

#include "Common.h"

namespace challenge {
	namespace whiterabbithole {

		/// <summary>
		/// Describes an histogram for strings.
		/// </summary>
		class Histogram
		{
		private:
			typedef std::map<char, size_t> histo_t;

		public:
			Histogram(const std::string& phrase);
			Histogram(const Histogram& other);
			~Histogram();

		private:
			std::string phrase;
			histo_t* char_histo;

		public:
			/// <summary>
			/// Compares two histograms.
			/// </summary>
			/// <param name="histogram1">The first histogram to compare.</param>
			/// <param name="histogram2">The second histogram to compare.</param>
			/// <returns>
			/// A value indicating whether one histogram is equal with respect to another.
			/// </returns>
			static bool equals(const Histogram& histogram1, const Histogram& histogram2);

			/// <summary>
			/// Compares two histograms with respect to containment.
			/// </summary>
			/// <param name="histogram1">The first histogram to compare.</param>
			/// <param name="histogram2">The second histogram to compare.</param>
			/// <returns>
			/// A value indicating whether one histogram contains the other.
			/// </returns>
			static bool contains(const Histogram& histogram1, const Histogram& histogram2);

			/// <summary>
			/// Compares two histograms for equality.
			/// </summary>
			/// <param name="other">The other histogram.</param>
			/// <returns>True if both histograms are the same, false otherwise.</returns>
			bool operator==(const Histogram& other) const;

			/// <summary>
			/// Compares two histograms for inequality.
			/// </summary>
			/// <param name="other">The other histogram.</param>
			/// <returns>True if both histograms are not the same, false otherwise.</returns>
			bool operator!=(const Histogram& other) const;

			/// <summary>
			/// Compares two histograms for equality or containment.
			/// </summary>
			/// <param name="other">The other histogram.</param>
			/// <returns>True if this histogram contains or is equal to the other one, false otherwise.</returns>
			bool operator>=(const Histogram& other) const;

			/// <summary>
			/// Compares two histograms for equality or containment.
			/// </summary>
			/// <param name="other">The other histogram.</param>
			/// <returns>True this histogram is contained by or equal to the other one, false otherwise.</returns>
			bool operator<=(const Histogram& other) const;

		private:
			void initialize();
			static bool eval_from_to(const Histogram& histogram1, const Histogram& histogram2,
				bool (*eval)(size_t v1, size_t v2), bool exact_alphabet);
			static bool eval_equal(size_t v1, size_t v2);
			static bool eval_geq(size_t v1, size_t v2);
			static bool eval_leq(size_t v1, size_t v2);
		};

	} // namespace whiterabbithole
} // namespace challenge

#endif
