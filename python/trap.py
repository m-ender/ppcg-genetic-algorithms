import coordinates
from sys import version_info

from constants import NUMBER_OF_KILLERS, NUMBER_OF_TELEPORTERS, NUMBER_OF_WALLS, MAX_TELEPORT_DISTANCE

if version_info >= (3,):
    xrange = range

class Trap(object):
    possible_directions = []
    max_traps = 4

    def __init__(self, direction):
        self.direction = direction

    def is_killer(self):
        return False

    def is_mover(self):
        return False

    def is_wall(self):
        return False


class DeathTrap(Trap):
    possible_directions = coordinates.directions
    max_traps = NUMBER_OF_KILLERS

    def is_killer(self):
        return True


class TeleportationTrap(Trap):
    possible_directions = [coordinates.Coordinate(x,y)
                           for x in xrange(-MAX_TELEPORT_DISTANCE,
                                           MAX_TELEPORT_DISTANCE + 1)
                           for y in xrange(-MAX_TELEPORT_DISTANCE,
                                           MAX_TELEPORT_DISTANCE + 1)
                           if x != 0 or y != 0
                           ]
    max_traps = NUMBER_OF_TELEPORTERS

    def is_mover(self):
        return True

class WallTrap(Trap):
    possible_directions = coordinates.directions
    max_traps = NUMBER_OF_WALLS

    def is_wall(self):
        return True


trap_types = DeathTrap, TeleportationTrap, WallTrap
