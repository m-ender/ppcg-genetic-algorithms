
class Trap(object):
    def __init__(self, board, direction, color):
        self.board = board
        self.direction = direction
        self.color = color

    def turn(self, coordinates):
        pass

    def moved_to(self, coordinates, origin):
        pass


class DeathTrap(Trap):
    def turn(self, coordinates):
        self.board.specimens[coordinates] = []


class TeleportationTrap(Trap):
    def moved_to(self, coordinates, origin):
        self.board.specimens[coordinates+self.direction].extend(
            self.board.specimens[coordinates])
        self.board.specimens[coordinates] = []


class WallTrap(Trap):
    def moved_to(self, coordinates, origin):
        self.board.specimens[origin].extend(
            self.board.specimens[coordinates])
        self.board.specimens[coordinates+self.direction] = []


trap_types = DeathTrap, TeleportationTrap, WallTrap