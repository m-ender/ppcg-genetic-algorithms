## Ruby controller for the [Genetic Algorithms PPCG Challenge]

### Running the game

Clone the repository, go into the `./ruby` directory and run as

    ruby game.rb

### Adding your player

Your player should be added to `player.rb` as a subclass of `Player`, and the final line of `player.rb` should be changed to indicate the subclass name of the player to be used.

Your subclass should implement a method `turn` which takes no parameters and returns a `Vector2D` with integer components in the range `-1` to `1`. The superclass `Player` provides a couple of useful members for your convenience:

    @rng        A seeded random number generator. This is the only source of
                randomness you may use.

    @genome     A 100-bit integer representing the current specimen's genome.
    bit_at      Takes an integer i and gives you the bit at that position in
                the genome.
    bit_range   Takes two integers, start and stop, and gives you the integer
                represented by the bits from start to stop-1.
    bit_chunk   Takes two integers, start and length, and gives you the integer
                represented by the 'length' first bits from start.

    @vision     A 5x5 array of integers representing colours. The first index
                is x, the second is y. The specimen itself is located at (2,2).
    vision_at   A convenience method, which takes x and y relative to the
                current specimen, i.e. in range [-2,2].

There are a few simple example subclasses to give you the idea of how this works.

### Ruby versions

The controller has been tested in **Ruby 2.0.0**, but should work with any newer version as well.


[Genetic Algorithms PPCG Challenge]: http://codegolf.stackexchange.com/questions/44707/lab-rat-race-an-exercise-in-genetic-algorithms
