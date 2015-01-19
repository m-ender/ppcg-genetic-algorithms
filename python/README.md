## Python controller for the [Genetic Algorithms PPCG Challenge]

### Running the game

Clone the repository, go into the `./python` directory and run as

    python game.py

or just

    game.py

if your computer is set up to run python files directly.

### Adding your player

Your player should be added to `player.py` as a subclass of `Player()`, and the final line of `player.py` should be changed to indicate the subclass name of the player to be used. There are a few simple example subclasses to give you the idea of how this works.

### Python versions

The controller should work with __Python 2.6+__ and __Python 3.2+__ so you can write your player in whichever you prefer. For a significant speed increase you can also use __[PyPy]__.

### Display

If you wish to visualise the progress you can choose a display option in `constants.py`.  About a page down, under `#Display related constants`, there are a number of import statements under the header `#Pick one of the following`. By default this will be `no_display`, but you can comment this out and uncomment one of the others instead. `tkinter_display` uses tkinter, which comes installed with most versions of python. If you have also installed pygame, you can choose `pygame_display` which will be faster. Note that `no_display` will always be fastest.


[Genetic Algorithms PPCG Challenge]: http://meta.codegolf.stackexchange.com/questions/2140/sandbox-for-proposed-challenges/4656#4656

[PyPy]: http://pypy.org/

