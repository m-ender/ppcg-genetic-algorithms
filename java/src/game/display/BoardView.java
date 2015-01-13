package game.display;

import game.Board;
import game.Utils;

import javax.swing.*;
import java.awt.*;

public class BoardView extends JPanel {
    private final Board board;
    public BoardView(Board board){
        this.board = board;
        setPreferredSize(new Dimension(Display.SQUARE_SIZE*Board.BOARD_WIDTH, Display.SQUARE_SIZE*Board.BOARD_HEIGHT));

    }

    @Override
    protected void paintComponent(Graphics g) {
        super.paintComponent(g);
        for(Point changedCell: board.allLocations){
            Color color = board.getSquare(changedCell).colorCode.color;
            Point position = Utils.scale(changedCell, Display.SQUARE_SIZE);
            g.setColor(color);
            g.fillRect(position.x, position.y, Display.SQUARE_SIZE, Display.SQUARE_SIZE);
            if (board.hasSpecimen(changedCell)){
                g.setColor(Color.white);
                g.fillRect(position.x+1, position.y+1, Display.SQUARE_SIZE-2, Display.SQUARE_SIZE-2);
            }
        }

    }
}


