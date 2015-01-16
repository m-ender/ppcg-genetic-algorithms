import pygame
import random
import sys

from constants import CELL_SCALAR, TITLE

random = random.Random()


class Display:
    def __init__(self, width, height):
        pygame.init()
        self.screen = pygame.display.set_mode(
            (height*CELL_SCALAR, width*CELL_SCALAR),
            0, 32)
        pygame.display.set_caption(TITLE)
        self.colors = {}

    def __del__(self):
        pygame.display.quit()

    def draw_cell(self, coordinates, board):
        color_code = board.get_color(coordinates)
        if color_code not in self.colors:
            color = [random.randrange(256) for _ in range(3)]
            self.colors[color_code] = color
        else:
            color = self.colors[color_code]
        self.screen.fill(color, pygame.Rect(
            (coordinates.x*CELL_SCALAR, coordinates.y*CELL_SCALAR),
            (CELL_SCALAR, CELL_SCALAR)
        ))
        if coordinates in board.specimens and board.specimens[coordinates]:
            self.screen.fill((255, 255, 255), pygame.Rect(
                (coordinates.x*CELL_SCALAR+1, coordinates.y*CELL_SCALAR+1),
                (CELL_SCALAR-2, CELL_SCALAR-2)
            ))

    def update(self):
        self.screen.blit(self.screen, (0, 0))
        pygame.display.update()
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                sys.exit(0)
