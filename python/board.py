from random import Random
from coordinates import BOARD_HEIGHT, BOARD_WIDTH, Coordinate
from trap import trap_types, Trap
from square import Square

OUT_OF_BOUNDS_COLOR = -1


class Board(object):
    def __init__(self, seed, colors):
        self.random = Random(seed)
        self.specimens = {}
        self.traps = {}
        self.next_specimens = {}
        self.changed_cells = set()
        self.out_of_bounds = Square(OUT_OF_BOUNDS_COLOR)
        self.out_of_bounds.killer = True
        self.finish_line = Square(colors[-1])
        self.squares = [[Square(self.random.choice(colors))
                        for __ in xrange(BOARD_WIDTH)]
                        for __ in xrange(BOARD_HEIGHT)]
        self.traps = [Trap(self, Coordinate(0, 0))]*len(colors)
        for trap_type in trap_types:
            used_traps = self.random.sample(trap_type.possible_directions,
                                            trap_type.max_traps)
            coloring = zip(used_traps, colors)
            colors = colors[len(used_traps):]
            for direction, color in coloring:
                self.traps[color] = trap_type(self, direction)
        for y in xrange(BOARD_HEIGHT):
            for x in xrange(BOARD_WIDTH):
                coordinate = Coordinate(x, y)
                self.changed_cells.add(coordinate)
                trap = self.traps[self.get_square(coordinate).color]
                if trap.is_killer():
                    self.get_square(trap.direction+coordinate).killer = True
                if trap.is_mover():
                    self.get_square(coordinate).teleport = trap.direction
                if trap.is_wall():
                    self.get_square(coordinate).killer = True
                    self.get_square(coordinate).wall = True

    def get_square(self, coordinate):
        if coordinate.out_of_bounds():
            return self.out_of_bounds
        if coordinate.at_finish():
            return self.finish_line
        return self.squares[coordinate.y][coordinate.x]

    def add_specimen(self, specimen, coordinates):
        if coordinates in self.specimens:
            self.specimens[coordinates].append(specimen)
        else:
            self.specimens[coordinates] = [specimen]
        self.changed_cells.add(coordinates)

    def get_color(self, coordinate):
        return self.get_square(coordinate).color

    def get_changed_cells(self):
        changed = self.changed_cells
        self.changed_cells = set()
        return changed

    def update_specimens(self):
        self.changed_cells.update(self.next_specimens.keys())
        self.changed_cells.update(self.specimens.keys())
        self.specimens = self.next_specimens
        self.next_specimens = {}