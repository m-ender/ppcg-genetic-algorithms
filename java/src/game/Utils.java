package game;

import java.awt.Point;
import java.util.ArrayList;
import java.util.List;

public class Utils {
    public static List<Point> createArea(int sideLength){
        List<Point> area = new ArrayList<Point>(sideLength*sideLength);
        for (int x = -sideLength/2; x < -sideLength/2+sideLength; x++){
            for (int y = -sideLength/2; y < -sideLength/2+sideLength; y++){
                area.add(new Point(x, y));
            }
        }
        return area;
    }
    public static List<Point> createArea(int width, int height){
        List<Point> area = new ArrayList<Point>(width*height);
        for (int x = 0; x < width; x++){
            for (int y = 0; y <= height; y++){
                area.add(new Point(x, y));
            }
        }
        return area;
    }

    public static Point add(Point p1, Point p2){
        return new Point(p1.x+p2.x,p1.y+p2.y);
    }

    public static <T> T pickOne(List<T> list){
        return list.get(Constants.random.nextInt(list.size()));
    }

    public static Point scale(Point point, int scalar){
        return new Point(point.x*scalar, point.y*scalar);
    }

}
