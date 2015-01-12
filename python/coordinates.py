BOARD_WIDTH = 200
BOARD_HEIGHT = 50


class Coordinate():
    def __init__(self, x, y):
        self.x = x
        self.y = y 
        self.hash = hash((x, y))

    def __add__(self, other):
        return Coordinate(other.x+self.x, other.y+self.y)

    def __sub__(self, other):
        return Coordinate(self.x-other.x, self.y-other.y)

    def __hash__(self):
        return self.hash

    def __eq__(self, other):
        return self.x == other.x and self.y == other.y

    def __mul__(self, other):
        return Coordinate(self.x*other, self.y*other)

directions = [Coordinate(x,y) for x in (-1,0,1) for y in (-1,0,1)]