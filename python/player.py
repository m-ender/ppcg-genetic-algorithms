from coordinates import Coordinate
import random


class Player(object):
    def __init__(self):
        self.vision = None
        self.specimen = None

    def take_turn(self, specimen, vision):
        self.vision = vision
        self.specimen = specimen
        self.turn()

    def turn(self):
        return Coordinate(0, 1)

    def vision_at(self, coordinates):
        adjusted_coordinates = Coordinate(2, 2)+coordinates
        return self.vision[adjusted_coordinates.x, adjusted_coordinates.y]




class RandomPlayer(Player):
    def take_turn(self, specimen, vision):
        return Coordinate(random.randint(-1, 1), 1)

class LinearCombinationPlayer(Player):
    def __init__(self):
        self.coords = [#Coordinate(-1,-1),
                       #Coordinate( 0,-1),
                       #Coordinate( 1,-1),
                       #Coordinate(-1, 0),
                       #Coordinate( 1, 0),
                       Coordinate(-1, 1),
                       Coordinate( 0, 1),
                       Coordinate( 1, 1)]

    def take_turn(self, specimen, vision):
        s = 0
        for i in range(25):
            s += specimen.bit_range(2*i,2*i+2)*vision[i].color
        return self.coords[s%3]


PLAYER_TYPE = LinearCombinationPlayer