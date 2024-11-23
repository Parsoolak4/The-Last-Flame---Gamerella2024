
using UnityEngine;

public class Grid
{
    public TilePoint[,] Points { get; private set; }
    
    public Grid(GridData gridData) {
        Points = new TilePoint[gridData.Width, gridData.Height];
        Points[0, 0] = new TilePoint();
    }

    public struct TilePoint {
        public GameObject gameObject;
        // TODO : add character/unit/etc shows if this tilePoint is taken
    }
}
