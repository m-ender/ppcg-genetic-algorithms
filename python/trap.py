
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
        if coordinates in self.board.specimens:
            del self.board.specimens[coordinates]


class TeleportationTrap(Trap):
    #TODO make this trap be 9x9
    def moved_to(self, coordinates, origin):
        if coordinates+self.direction in self.board.next_specimens:
            self.board.next_specimens[coordinates+self.direction].extend(
                self.board.next_specimens[coordinates])
        else:
            self.board.next_specimens[coordinates+self.direction] = \
                self.board.next_specimens[coordinates]
        del self.board.next_specimens[coordinates]


class WallTrap(Trap):
    def moved_to(self, coordinates, origin):
        if origin in self.board.next_specimens:
            self.board.next_specimens[origin].extend(
                self.board.next_specimens[coordinates])
        else:
            self.board.next_specimens[origin] = \
                self.board.next_specimens[coordinates]
        del self.board.next_specimens[coordinates]


trap_types = DeathTrap, TeleportationTrap, WallTrap