package game.players;

import java.awt.*;
import java.util.Map;

public abstract class Player {
    public static Player currentPlayer = new ColorScorePlayer();
    public abstract Point takeTurn(String genome, Map<Point, Integer> vision);

}
