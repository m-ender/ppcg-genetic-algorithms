import itertools

class Coordinate():
    def __init__(self, x, y):
        self.x = x
        self.y = y

    def __add__(self, other):
        return Coordinate(other.x+self.x, other.y+self.y)

    def __sub__(self, other):
        return Coordinate(self.x-other.x, self.y-other.y)

    def __hash__(self):
        return hash((self.x,self.y))

    def __eq__(self, other):
        return self.x == other.x and self.y == other.y

    def __mul__(self, other):
        return Coordinate(self.x*other, self.y*other)

cardinals = Up, Left, Down, Right = [Coordinate(1, 0), Coordinate(0, 1),
                                     Coordinate(-1, 0), Coordinate(0, -1)]

diagonals = itertools.permutations(cardinals, 2)

directions = cardinals
directions.extend(cardinals)
directions.append(Coordinate(0,0))