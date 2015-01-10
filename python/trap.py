import coordinates

class Trap(object):
    def __init__(self, board):
        self.board = board

    def turn(self, coordinates):
        pass

class DeathTrap(Trap):
    def __init__(self, board, direction):
        Trap.__init__(self, board)
        self.direction = direction

    def turn(self, coordinates):
        self.board.specimens[coordinates+self.direction] = []


class TeleportationTrap(Trap):
    def __init__(self, board, direction):
        Trap.__init__(self, board)
        self.direction = direction

    def turn(self, coordinates):
        self.board.specimens[coordinates].extend(
            self.board.specimens[coordinates+self.direction])
        self.board.specimens[coordinates+self.direction] = []


trap_types = DeathTrap, TeleportationTrap