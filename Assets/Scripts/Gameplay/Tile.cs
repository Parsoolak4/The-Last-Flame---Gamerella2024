using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int Index { get; set; } // The (i, j) index of this tile on the grid
    public Unit Unit { get; set; } // The current unit occupying this tile

    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void AddHighlight() { 
        // TODO
    }

    public void RemoveHighlight() {
        // TODO
    }
 
    public void SetColor(Color color) {
        spriteRenderer.color = color;
    }

}