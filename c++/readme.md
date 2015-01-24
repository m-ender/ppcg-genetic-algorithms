#Using the C++ Controller

##Interfacing with the controller
Your function should have a signature of the type of `coord_t f(dna_t d, view_t v)`. Bits in the genome can be accessed by array index notation `d[0]` ... `d[99]`. The colors of a square in the bot's neighborhood at the relative position (dx, dy) can be accessed by `v(dx, dy)`. The return type, `coord_t`, is a struct with two `int` members `x` and `y`. For example `return {1, -1};`. If a bot tries to examine the color of or move to a location beyond the acceptable range, <strike>a warning message will be printed, but execution will continue</strike>it will choke and die. 

Your function should not save any state between calls, except perhaps effects on a random number generator. The random seed used to generate each board is printed out, which should make it possible to exactly reproduce runs, if you use the same generator (see under Multithreading). `#define`ing `RANDOM_SEED` to some value will cause every game to be set up identically, and allows a specific game to be reproduced.

##Examples
A few example players which should demonstrate how to use the provided functionality are located in `main.cpp`.

##Compiling
Your compiler should support C++11. The controller and player can be built by compiling `main.cpp`, which `#include`s the other files. For example, 
`g++ -std=c++11 -O2 main.cpp`.

The player (function) to test should be set by changing the argument of `runsimulation` in the `main` function.

##Multithreading
Multithreading support can be enabled by setting `USE_MULTITHREADING` to 1 in `magicnum.h`. The number of threads can be controlled by `N_THREADS`. Hopefully, it works correctly.

If you enable multithreading, you should not use the C library's `rand()` function, which is not thread-safe. The `view_t` class has a member `rng` with a few random functions which may be used for convenience. `rng.rint(n)` gives a random number from 0 to n-1 inclusive. `rng.rint(a,b)` gives one between a and b inclusive. `rng.rndouble()` is a double between 0 and 1.

##Other settings
can also be adjusted in `magicnum.h`, such as the number of games. A few types of debug output can be enabled or disabled.