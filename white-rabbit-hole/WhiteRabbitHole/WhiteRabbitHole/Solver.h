// Solver.h

#ifndef SOLVER_H_
#define SOLVER_H_

#include <iostream>
#include <fstream>
#include <string>
#include <exception>
#include <unordered_set>

namespace challenge {
	namespace whiterabbithole {

		/// <summary>
		/// Describes a solver.
		/// </summary>
		class Solver
		{
		private:
			typedef std::unordered_set<std::string> wordset;

		public:
			/// <summary>
			/// Initializes a new instance of this class.
			/// </summary>
			/// <param name="anagram_phrase">The anagram phrase to handle.</param>
			/// <param name="dbfile_path">The path to the words file.</param>
			Solver(const std::string& anagram_phrase, const std::string& dbfile_path);

			/// <summary>
			/// Copy initializes a new instance of this class.
			/// </summary>
			/// <param name="other">The other instance to copy from.</param>
			Solver(const Solver& other);

			/// <summary>
			/// Destroys an instance of this class.
			/// </summary>
			~Solver();

		private:
			std::string anagram_phrase;
			std::string dbfile_path;
			wordset* words;
			wordset* use_words;

		public:
			/// <summary>
			/// Executes the solving process.
			/// </summary>
			virtual void solve();

		private:
			bool check_dbfile_path() const;
			void load_words();
			void process_words();
			bool accept_word(const std::string& word) const;
			unsigned int get_phrase_words_count() const;
			unsigned int get_phrase_char_count() const;
		}; // class Solver

	} // namespace whiterabbithole
} // namespace challenge

#endif
