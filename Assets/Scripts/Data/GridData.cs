using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Data/Grid", order = 1)]
public class GridData : ScriptableObject
{
    [SerializeField] GameObject tilePrefab;
    [SerializeField][Range(1, 10)] int width; 
    [SerializeField][Range(1, 10)] int height;
    [SerializeField] UnitData[] units;

    public GameObject TilePrefab => tilePrefab;
    public int Width => width;
    public int Height => height;
    public UnitData[] Units => units;
}