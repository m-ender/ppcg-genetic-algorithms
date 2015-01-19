## Ruby controller for the [Genetic Algorithms PPCG Challenge]

### Running the game

Clone the repository, go into the `./ruby` directory and run as

    ruby game.rb

### Adding your player

Your player should be added to `player.rb` as a subclass of `Player`, and the final line of `player.rb` should be changed to indicate the subclass name of the player to be used.

Your subclass should implement a method `turn` which takes no parameters and returns a `Vector2D` with integer components in the range `-1` to `1`. The superclass `Player` provides a couple of useful members for your convenience:

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

### Python versions

The controller should work with __Python 2.6+__ and __Python 3.2+__ so you can write your player in whichever you prefer. For a significant speed increase you can also use __[PyPy]__.

### Display

If you wish to visualise the progress you can choose a display option in `constants.py`.  About a page down, under `#Display related constants`, there are a number of import statements under the header `#Pick one of the following`. By default this will be `no_display`, but you can comment this out and uncomment one of the others instead. `tkinter_display` uses tkinter, which comes installed with most versions of python. If you have also installed pygame, you can choose `pygame_display` which will be faster. Note that `no_display` will always be fastest.


[Genetic Algorithms PPCG Challenge]: http://meta.codegolf.stackexchange.com/questions/2140/sandbox-for-proposed-challenges/4656#4656

[PyPy]: http://pypy.org/

