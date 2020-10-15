# CMakeList.txt : CMake project for WhiteRabbitHole, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.8)

# Add source to this project's executable.
add_executable(WhiteRabbitHole "Program.cpp" "Program.h" "Solver.cpp" "Solver.h" "Utils.cpp" "Utils.h" "Hashing.cpp" "Hashing.h" "MD5.cpp" "MD5.h" "Histogram.h" "Histogram.cpp" "Common.h")

# Link external libraries
#find_library(CRYPTOPP_LIB cryptopp "C:/Users/antino/cryptopp/x64/DLL_Output/Release")
#find_library(CRYPTOPP_LIB cryptopp STATIC "C:/Users/antino/cryptopp/x64/cryptlib/DLL-Import-Release")
#find_library(CRYPTOPP_LIB cryptlib.lib HINTS C:/Users/antino/cryptopp/x64/*)

#target_link_libraries(WhiteRabbitHole PUBLIC "${CRYPTOPP_LIB}")

# Include directories for linking purposes (libraries)
#target_include_directories(WhiteRabbitHole PUBLIC "C:/Users/antino/cryptopp")
