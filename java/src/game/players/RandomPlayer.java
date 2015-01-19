package game.players;

import game.Constants;

import java.awt.*;
import java.util.Map;
import java.util.Random;

public class RandomPlayer extends Player {
    @Override
    public Point takeTurn(String genome, Map<Point, Integer> vision) {
        return new Point(1, new Random().nextInt(3)-1);
    }
}
