package game.players;

import game.Specimen;

import java.util.List;
import java.awt.*;
import java.util.Map;

public abstract class Player {
    public static Player currentPlayer = new RandomPlayer();
    public abstract Point takeTurn(String genome, Map<Point, Integer> vision);

}
