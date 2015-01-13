package game.display;

import game.Board;

import javax.swing.*;
import java.awt.*;

public class Display extends JFrame {
    public static final int SQUARE_SIZE = 4;
    public static final String GAME_TITLE = "The Genetic Rat Race";
    public Display(Board board){
        BoardView boardView = new BoardView(board);
        setLayout(new BorderLayout());
        add(boardView);
        pack();
        validate();
        setDefaultCloseOperation(WindowConstants.EXIT_ON_CLOSE);
        setTitle(GAME_TITLE);
        setLocationRelativeTo(null);
        setVisible(true);
    }
}
