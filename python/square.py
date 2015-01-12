import coordinates

class Square(object):
    def __init__(self, color):
        self.color = color
        self.killer = False
        self.teleport = coordinates.Coordinate(0, 0)
        self.wall = False