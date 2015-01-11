BOARD_WIDTH = 50
BOARD_HEIGHT = 200


class Coordinate():
    def __init__(self, x, y):
        self.x = x % BOARD_WIDTH
        self.y = y % BOARD_HEIGHT
        self.real_y = y
        self.real_x = x

    def __add__(self, other):
        return Coordinate(other.real_x+self.real_x, other.real_y+self.real_y)

    def __sub__(self, other):
        return Coordinate(self.real_x-other.real_x, self.real_y-other.real_y)

    def __hash__(self):
        return hash((self.real_x, self.real_y))

    def __eq__(self, other):
        return self.real_x == other.real_x and self.real_y == other.real_y

    def __mul__(self, other):
        return Coordinate(self.real_x*other, self.real_y*other)

directions = [Coordinate(x,y) for x in (-1,0,1) for y in (-1,0,1)]