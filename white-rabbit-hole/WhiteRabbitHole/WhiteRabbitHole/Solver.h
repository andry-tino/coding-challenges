// Solver.h

#ifndef SOLVER_H_
#define SOLVER_H_

#include <iostream>
#include <fstream>
#include <string>
#include <exception>
#include <vector>
#include <map>
#include <algorithm>

namespace challenge {
	namespace whiterabbithole {

		typedef std::vector<std::string> phrase_t;

		/// <summary>
		/// Represents a utility object for algorithmic purposes.
		/// </summary>
		struct DispositionsTreeWalkState {
		public:
			typedef std::vector<unsigned int> disposition_t;
			typedef std::map<std::string, bool> dispositions_cache_t;
		public:
			DispositionsTreeWalkState(bool use_cache = true);
			DispositionsTreeWalkState(const DispositionsTreeWalkState& other);
			~DispositionsTreeWalkState();
		private:
			bool use_cache;
			disposition_t* disposition;
			dispositions_cache_t* dispositions_cache;
		public:
			const disposition_t* get_disposition() const;
			void push_to_disposition(unsigned int index) const;
			void pop_from_disposition() const;
			bool is_disposition_in_cache(const disposition_t& disposition) const;
			static std::string get_disposition_words_str(const disposition_t& disposition,
				const std::vector<std::string>& words);
		private:
			std::string get_disposition_str() const;
		};

		/// <summary>
		/// Describes a solver.
		/// </summary>
		class Solver
		{
		private:
			typedef std::vector<std::string> wordset;
			typedef std::vector<std::string> usewordset;
			typedef std::vector<std::vector<std::string>> result_t;
			enum DispositionRunResult { No, Candidate, Valid };

		public:
			/// <summary>
			/// Initializes a new instance of this class.
			/// </summary>
			/// <param name="anagram_phrase">The anagram phrase to handle.</param>
			/// <param name="dbfile_path">The path to the words file.</param>
			/// <param name="log_stream">Log stream.</param>
			Solver(const std::string& anagram_phrase, const std::string& dbfile_path, std::ostream& log_stream);

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
			std::ostream* log_stream;
			wordset* words;
			usewordset* use_words;

		public:
			/// <summary>
			/// Executes the solving process.
			/// </summary>
			virtual const result_t& solve();

			/// <summary>
			/// Loads all on demand resources.
			/// </summary>
			void load_all_res();

		private:
			void log(const std::string& what) const;
			unsigned int get_disposition_count(unsigned int group_size) const;
			bool check_dbfile_path() const;
			void load_words();
			void process_words();
			std::vector<std::string> get_words_in_phrase(const std::string& phrase) const;
			bool accept_word(const std::string& word) const;
			unsigned int get_phrase_words_count() const;
			unsigned int get_phrase_char_count() const;
			void walk_dispositions(
				unsigned int group_size,
				const DispositionsTreeWalkState* state,
				result_t* result) const;
			DispositionsTreeWalkState::disposition_t get_residual_indices(
				const DispositionsTreeWalkState::disposition_t& disposition) const;
			DispositionRunResult run_disposition(
				unsigned int group_size,
				const DispositionsTreeWalkState* state,
				result_t* result) const;
		}; // class Solver

	} // namespace whiterabbithole
} // namespace challenge

#endif
