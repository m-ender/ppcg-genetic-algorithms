package game.display;

import game.Board;
import game.Constants;

import javax.swing.*;
import java.awt.*;

public class Display extends JFrame {
    public Display(Board board){
        BoardView boardView = new BoardView(board);
        setLayout(new BorderLayout());
        add(boardView);
        pack();
        validate();
        setDefaultCloseOperation(WindowConstants.EXIT_ON_CLOSE);
        setTitle(Constants.TITLE);
        setLocationRelativeTo(null);
        setVisible(true);
    }
}
