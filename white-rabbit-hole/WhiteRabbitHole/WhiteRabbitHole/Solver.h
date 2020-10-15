// Solver.h

#ifndef SOLVER_H_
#define SOLVER_H_

#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <map>

#include "Common.h"
#include "Histogram.h"

namespace challenge {
	namespace whiterabbithole {

		/// <summary>
		/// Represents a utility object for algorithmic purposes.
		/// </summary>
		struct DispositionsTreeWalkState {
		public:
			typedef std::vector<unsigned int> disposition_t;
		public:
			DispositionsTreeWalkState(bool use_ordering = true);
			DispositionsTreeWalkState(const DispositionsTreeWalkState& other);
			~DispositionsTreeWalkState();
		private:
			bool use_cache;
			disposition_t* disposition;
		public:
			const disposition_t* get_disposition() const;
			void push_to_disposition(unsigned int index) const;
			void pop_from_disposition() const;
			bool is_disposition_ordered() const;
			static std::string get_disposition_words_str(const disposition_t& disposition,
				const std::vector<std::string>& words);
		private:
			bool is_disposition_ordered(const disposition_t& disposition) const;
			std::string get_disposition_str(const disposition_t& disposition) const;
			std::string get_disposition_str() const;
		};

		/// <summary>
		/// Describes a solver.
		/// </summary>
		class Solver
		{
		public:
			typedef std::vector<std::vector<std::string>> result_t;
		private:
			typedef std::vector<std::string> wordset_t;
			typedef std::vector<std::string> usewordset_t;
			enum DispositionRunResult { No, Candidate, Valid, Skipped };

		public:
			/// <summary>
			/// Initializes a new instance of this class.
			/// </summary>
			/// <param name="anagram_phrase">The anagram phrase to handle.</param>
			/// <param name="dbfile_path">The path to the words file.</param>
			/// <param name="log_stream">Log stream.</param>
			Solver(const std::string& anagram_phrase, const std::string& dbfile_path,
				const std::string& phrase_hash, std::ostream& log_stream);

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
			std::string phrase_hash;
			std::ostream* log_stream;
			Histogram* anagram_phrase_histo;
			wordset_t* words;
			usewordset_t* use_words;
			alphabet_t* alphabet;
			result_t* result;

		public:
			/// <summary>
			/// Executes the solving process.
			/// </summary>
			virtual void solve();

			/// <summary>
			/// Loads all on demand resources.
			/// </summary>
			void load_all_res();

			/// <summary>
			/// Prints the result in the provided stream.
			/// </summary>
			/// <param name="result"></param>
			/// <param name="stream"></param>
			void print_result(std::ostream& stream) const;

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
				const usewordset_t& usewordset,
				unsigned int group_size,
				const DispositionsTreeWalkState* state,
				result_t* result,
				bool walkCombinationsOnly) const;
			DispositionsTreeWalkState::disposition_t get_residual_indices(
				const usewordset_t& usewordset,
				const DispositionsTreeWalkState::disposition_t& disposition) const;
			DispositionRunResult run_disposition(
				const usewordset_t& usewordset,
				unsigned int group_size,
				const DispositionsTreeWalkState* state,
				result_t* result,
				bool checkValid) const;
			bool is_phrase_candidate(const phrase_t& phrase) const;
			bool check_phrase_hash(const phrase_t& phrase) const;
			std::string phrase_to_string(const phrase_t& phrase) const;
		}; // class Solver

	} // namespace whiterabbithole
} // namespace challenge

#endif
