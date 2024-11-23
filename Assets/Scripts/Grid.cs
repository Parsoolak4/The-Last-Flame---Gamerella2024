
public class Grid
{
    private TilePoint[,] points;
    
    public Grid(int width, int heigth) {
        points = new TilePoint[width, heigth];
        points[0, 0] = new TilePoint();
    }

    public struct TilePoint {
        // TODO : add character/unit/etc shows if this tilePoint is taken
    }
}
