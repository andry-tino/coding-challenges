// Program.cpp

#include "Program.h"
#include "Solver.h"

using namespace challenge::whiterabbithole;

int main()
{
	std::cout << "Program started!" << std::endl;

	// Acquire anagram phrase
	std::string anagram_phrase;
	std::cout << "Type in the anagram phrase..." << std::endl;
	std::getline(std::cin, anagram_phrase);
	std::cout << "Anagram phrase acquired: '" << anagram_phrase << "'" << std::endl;

	// Acquire correct phrase hash
	std::string phrase_hash;
	std::cout << "Type in the phrase MD5 hash..." << std::endl;
	std::getline(std::cin, phrase_hash);
	std::cout << "Phrase MD5 hash acquired: '" << phrase_hash << "'" << std::endl;

	// Acquire path to words dbfile
	std::string dbfile_path;
	std::cout << "Type in the word file location..." << std::endl;
	std::getline(std::cin, dbfile_path);
	std::cout << "Word dbfile location acquired: '" << dbfile_path << "'" << std::endl;

	// Allocating solver and running it
	Solver solver(anagram_phrase, dbfile_path, std::cout);
	solver.load_all_res(); // Will log meaningful values out

	std::getline(std::cin, std::string()); // Pause before starting

	std::cout << "Starting algorithm..." << std::endl;
	Solver::result_t result = solver.solve();
	std::cout << "Algorithm has ended!" << std::endl;

	std::cout << "Printing result..." << std::endl;
	Solver::print_result(result, std::cout);

	// All good
	return 0;
}
