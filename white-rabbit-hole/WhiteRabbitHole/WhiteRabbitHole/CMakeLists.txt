# CMakeList.txt : CMake project for WhiteRabbitHole, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.8)

include_directories("C:/Users/antino/cryptopp")
link_directories("C:/Users/antino/cryptopp/Win32/DLL_Output/Debug")

# Add source to this project's executable.
add_executable (WhiteRabbitHole "Program.cpp" "Program.h" "Solver.cpp" "Solver.h" "Utils.cpp" "Utils.h" "Hashing.cpp" "Hashing.h")

# TODO: Add tests and install targets if needed.
target_link_libraries(WhiteRabbitHole cryptopp)
