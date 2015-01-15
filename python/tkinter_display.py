try:
    import tkinter
except ImportError:
    import Tkinter as tkinter
import random
import array
from coordinates import Coordinate
from constants import TITLE, CELL_SCALAR, EMPTY_COLOR, SPECIMEN_COLOR, DEATH_COLOR, TELEPORT_COLOR, WALL_COLOR

random = random.Random()


class Display:

    def __init__(self, height, width):
        self.width_in_pixels = width * CELL_SCALAR
        self.height_in_pixels = height * CELL_SCALAR
        number_of_pixels = self.width_in_pixels*self.height_in_pixels

        empty_red, empty_green, empty_blue = EMPTY_COLOR
        self.red_values = array.array('B', [empty_red]*number_of_pixels)
        self.green_values = array.array('B', [empty_green]*number_of_pixels)
        self.blue_values = array.array('B', [empty_blue]*number_of_pixels)

        self.root = tkinter.Tk()
        self.frame = tkinter.Frame(self.root,
                                   width=self.width_in_pixels,
                                   height=self.height_in_pixels
                                   )
        self.frame.pack()
        self.canvas = tkinter.Canvas(self.frame,
                                     width=self.width_in_pixels,
                                     height=self.height_in_pixels,
                                     bg='black'
                                     )
        self.canvas.pack()
        self.picture=tkinter.PhotoImage(width=self.width_in_pixels,
                                        height=self.height_in_pixels
                                        )
        self.canvas.create_image(2,2,image=self.picture,anchor=tkinter.NW)
        self.root.wm_title(TITLE)
        self.colors = {}

    def __del__(self):
        self.root.destroy()

    def draw_cell(self, coordinates, board):
        square = board.get_square(coordinates)
        if not square.teleport == Coordinate(0, 0):
            color = TELEPORT_COLOR
        elif square.wall:
            color = WALL_COLOR
        elif square.killer:
            color = DEATH_COLOR
        else:
            color = EMPTY_COLOR

        # color_code = board.get_color(coordinates)
        # if color_code not in self.colors:
            # color = [random.randrange(256) for __ in range(3)]
            # self.colors[color_code] = color
        # else:
            # color = self.colors[color_code]

        self.rectangle(color,
                       coordinates.x * CELL_SCALAR,
                       coordinates.y * CELL_SCALAR,
                       CELL_SCALAR,
                       CELL_SCALAR
                       )
        if coordinates in board.specimens and board.specimens[coordinates]:
            self.rectangle(SPECIMEN_COLOR,
                           coordinates.x * CELL_SCALAR + 1,
                           coordinates.y * CELL_SCALAR + 1,
                           CELL_SCALAR - 2,
                           CELL_SCALAR - 2
                           )

    def rectangle(self, color, x, y, width, height):
        for i in range(x, x + width):
            for j in range(y, y + height):
                self.red_values[i + j*self.width_in_pixels] = color[0]
                self.green_values[i + j*self.width_in_pixels] = color[1]
                self.blue_values[i + j*self.width_in_pixels] = color[2]

    def display(self,s):
        self.picture.put(s,(0,0))

    def update(self):
        w = self.width_in_pixels
        h = self.height_in_pixels
        r = self.red_values
        g = self.green_values
        b = self.blue_values
        s = " ".join("{" +
                     " ".join(("#%02x%02x%02x" %
                              (r[pixel], g[pixel], b[pixel]))
                              for pixel in range(m, m + w)
                              ) +
                     "} " for m in range(0, w * h, w)
                     )
        self.display(s)
        self.root.update()
