## C# controller for the [Genetic Algorithms PPCG Challenge]

### Running the game

Clone the repository, go into the `./csharp` directory and compile the code

    csc *.cs

This will produce a Program.exe which you can run

Alternatively, you can load the `ppcggacscontroller.csproj` file into your favourite .NET IDE, and work from there

### Adding your player

Your player should be added to `Program.cs` as a static method, compatible with the `GameLogic.PlayerDel` delegate, described in `GameLogic.cs`. To use your player method, pass it as the only paramater to the constructor for the `GameLogic.Game` object in the Main method of `Program.cs`

The `GameLogic.PlayerDel` delegate takes 3 input parameters, and has 2 output parameters.

    @vision     Holds a 5x5 array of the colours which surround the current specimen.
    this[,]     Takes offset x and offset y relative to the current specimen, i.e. in range [-2,2],
                yielding the colour of the cell at that offset, your specimen is situation at
                0, 0 relative to this
    xd          The x dimension of the vision, how far you can see left and right (2)
    yd          The y dimension of the vision, how far you can see up and down (2)
    
    @genome     A 100-bit integer representing the current specimen's genome.
    this[]      Takes an integer i and gives you the bit at that position in
                the genome.
    cutOutInt   Takes two integers, start and length, and gives you the unsigned integer
                represented by the 'length' first bits from start.
    length      The length of the genome (100)
    
    @random     A `System.Random` instance you should use to proide any randomness you want
    
    @ox (out)   An integer you must set, this is the direction you will move along the x axis [-1,1]
    
    @oy (out)   An integer you must set, this is the direction you will move along the y axis [-1,1]

There are a few simple example player methods to give you the idea of how this works.

### Graphical Display

If you do not wish the Graphical Display to show when you run the controller, remove or comment out the line of code that sets the `displayer` property of the `GameLogic.Game` instance in the Main method of `Program.cs`

You can close the Graphical Display once it has loaded without terminating the controller, but you won't be able to get it back

### C# versions

The controller depends on C# 3.5 to compile


[Genetic Algorithms PPCG Challenge]: http://codegolf.stackexchange.com/questions/44707/lab-rat-race-an-exercise-in-genetic-algorithms
