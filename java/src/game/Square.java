package game;

import java.awt.*;

public class Square {
    public final ColorCode colorCode;
    public boolean kills;
    public Point teleportsTo;
    public boolean isWall;
    public Square(ColorCode colorCode){
        this.colorCode = colorCode;
        teleportsTo = new Point(0, 0);
        isWall = false;
        kills = false;
    }
}
