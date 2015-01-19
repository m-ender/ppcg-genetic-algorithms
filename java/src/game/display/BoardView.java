package game.display;

import game.Board;
import game.Square;
import game.Utils;
import static game.Constants.*;

import javax.swing.*;
import java.awt.*;

public class BoardView extends JPanel {
    private final Board board;
    public BoardView(Board board){
        this.board = board;
        setPreferredSize(new Dimension(CELL_SCALAR*BOARD_EXTENDED_WIDTH, CELL_SCALAR*BOARD_HEIGHT));

    }

    @Override
    protected void paintComponent(Graphics g) {
        super.paintComponent(g);
        for(int x = 0; x < BOARD_EXTENDED_WIDTH; x++) {
            for (int y = 0; y < BOARD_HEIGHT; y++) {
                Point cell = new Point(x, y);
                Square square = board.getSquare(cell);
                Color color;
                if (!square.teleportsTo.equals(new Point(0, 0))) {
                    color = TELEPORT_COLOR;
                } else if (square.isWall) {
                    color = WALL_COLOR;
                } else if (square.kills) {
                    color = DEATH_COLOR;
                } else {
                    color = EMPTY_COLOR;
                }
                Point position = Utils.scale(cell, CELL_SCALAR);
                g.setColor(color);
                g.fillRect(position.x, position.y, CELL_SCALAR, CELL_SCALAR);
                if (board.hasSpecimen(cell)) {
                    g.setColor(SPECIMEN_COLOR);
                    g.fillRect(position.x + 1, position.y + 1, CELL_SCALAR - 2, CELL_SCALAR - 2);
                }
            }
        }

    }
}


