# White Rabbit Hole
October 2020.

## Requirements
In order to build the project, you need:

- CMake 3.8+
- Either Windows, MacOS or Linux.

## Usage
The program can be used to hack an anagram phrase by knowing the original sentence's MD5 hash and a set of words (vocabulary). The vocabulary file must be an ASCII file with words separated by a newline.

Run the program by opening a shell and running the executable (on Windows):

```
.\WhiteRabbitHole.exe
```

When prompted, type in the following (in order) pressing `ENTER` each time:

1. The anagram phrase.
2. The MD5 hash.
3. The path to the vocabulary file.

At each input, the program will confirm the value by printing it out before asking the next information.

After typing in all the information, the program will print some data:

- `Words loaded`: The number of words loaded from the vocabulary file.
- `Usewords loaded`: The number of words extracted from the loaded words which are compatible with the anagram phrase. The program will run an initial filter based on the characters present in the anagram phrase: all words that have characters not showing in the anagram phrase are discarded.
- `Alphabet`: The set of characters present in the anagram phrase.

After this information is shown, to start the algorithm, press `ENTER`, the program will then show:

```
Starting algorithm...
```

That is the indication that work is in progress. From now on, the program will crunch operations and will terminate when all cases have been evaluated.

Example:

```
PS C:\Users\myuser\Documents\WhiteRabbitHole> .\WhiteRabbitHole.exe
Program started!
Type in the anagram phrase...
poultry outwits ants
Anagram phrase acquired: 'poultry outwits ants'
Type in the phrase MD5 hash...
e4820b45d2277f3844eac66c903e84be
Phrase MD5 hash acquired: 'e4820b45d2277f3844eac66c903e84be'
Type in the word file location...
C:\Users\antino\Downloads\wordlist.txt
Word dbfile location acquired: 'C:\Users\antino\Downloads\wordlist.txt'
Loading words...
Words loaded: 99175
Processing words...
Usewords loaded: 1659
Alphabet: y i a l s n u r o t w p

Starting algorithm...
Alphabet: y i a l s n u r o t w p
```

### Termination
The final printing should show results if any:

```
Printing result...
==================
- printout stout yawls
==================
```

## How it works
The naive approach is cracking the anagram by trying all permutations of the characters in the anagram phrase. Let $N$ be the number of characters in the anagram phrase (same as the number of characters in the original, unknown, phrase), then $N!$ would be the number of total cases to consider. When $N > 10$ such number starts becoming computationally challenging. A smarter ans faster approach is used.

After an initial configuration stage, where the program selectes the set of words that will be used to crack the anagram (referred to as: `usewords`), two phases will be executed:

1. **Combinations scanning**: All combinations of the usewords are considered. This number is $C = \frac{N!}{(N-m)!m!}$ where $m$ is the number of words in the phrase (deducible by the number of spaces in the anagram phrase). Each combination will be tested against a length and histogram check: the length of the sentence is checked and if it matches the anagram phrase's length then the histogram is checked. The histogram check evaluates that the number of characters in each combination matches the anagram phrase (anagramming a phrase leaves the histogram invariant). If the histogram check passes, that specific combination is added for later evaluation.
2. **Dispositions scanning**: Every combination which passed from the previous phase, will be tested here. For each combination, every permutation will be scanned. This means that for each combination, the number of cases is $P = m!$. Every permutation is tested using the MD5 hashing.

The total number of case evaluations is:

$$
\Omega = C P = \frac{N!}{(N-m)!m!} m! = \frac{N!}{(N-m)!}
$$

This number matche the total number of dispositions of $N$ words taken in groups of $m$. This seems not to justify the 2-phase approach then, however the need for it is when considering performance. The second phase is the only phase where hashing is performed, instead of performing a full set of operations in the first phase.

### Optimizations
This is not the best possible algorithm. This is a good approach. The main drawback is the fact that the program will check every possible case and will not stop when a matching phrase is found. This was done because MD5 is not secure and has been evaluated as weak, therefore all cases are considered.