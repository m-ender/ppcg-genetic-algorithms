package game.players;

import java.awt.*;
import java.util.Map;

public class ForwardPlayer extends Player {
    @Override
    public Point takeTurn(String genome, Map<Point, Integer> vision) {
        return new Point(1, 0);
    }
}
