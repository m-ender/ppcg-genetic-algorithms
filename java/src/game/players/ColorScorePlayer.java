package game.players;

import java.awt.*;
import java.util.Map;

public class ColorScorePlayer extends Player{
    private static final Point[] possibleMoves = {new Point(1, 0), new Point(1, -1), new Point(1, 1)};

    @Override
    public Point takeTurn(String genome, Map<Point, Integer> vision) {
        int chunkLength = genome.length()/16;
        int maxSum = -1;
        Point maxSumMove = possibleMoves[0];
        for (Point move: possibleMoves){
            if (vision.get(move) == -1){
                continue;
            }
            int initialPoint = chunkLength*vision.get(move);
            int sum = 0;
            for (int i = initialPoint; i < initialPoint + chunkLength; i++){
                sum = (sum<<1)+Integer.parseInt(genome.charAt(i)+"");
            }
            if (sum > maxSum){
                maxSum = sum;
                maxSumMove = move;
            }
        }
        return maxSumMove;
    }
}
