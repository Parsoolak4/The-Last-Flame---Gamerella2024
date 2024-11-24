using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int Index { get; set; } // The (i, j) index of this tile on the grid
    public Unit Unit { get; set; } // The current unit occupying this tile

    [SerializeField] Sprite highlighted;
    [SerializeField] Sprite normal;

    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void AddHighlight() {
        spriteRenderer.sprite = highlighted;
    }

    public void RemoveHighlight() {
        spriteRenderer.sprite = normal;
    }
 
    public void SetColor(Color color) {
        spriteRenderer.color = color;
    }

}