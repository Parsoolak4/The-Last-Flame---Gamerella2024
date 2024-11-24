using UnityEngine;

public class Unit : MonoBehaviour{

    [SerializeField] UnitTypes.Movement movements;

    public Vector2Int Index { get; set; }
    public UnitTypes.Movement Movements => movements;

    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(UnitTypes.Movement movementData)
    {
        movements = movementData;
    }
    
    public void SetSortingOrder(int sortingOrder) {
        spriteRenderer.sortingOrder = sortingOrder;
    }
}