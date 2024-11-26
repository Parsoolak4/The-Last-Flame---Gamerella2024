
using EasyTransition;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] LayerMask playerLayerMask;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Sprite deadPlayerSprite;
    [SerializeField] GameObject exitPrefab;
    [SerializeField] GameObject finalExitPrefab;
    [SerializeField] GameObject transitionPrefab;
    [SerializeField] float unitMoveDuration;
    [SerializeField] float cameraSpeed;
    [SerializeField] Vector3 spawnOffset;
    [SerializeField] Vector3 cameraOffset;
    [SerializeField] GridData[] grids;

    private Vector3 TILE_RIGHT_UNIT = new Vector2(1.2f, 0.7f);
    private Vector3 TILE_UP_UNIT = new Vector2(-1.2f, 0.7f);

    public Tile[,] Grid { get; private set; }

    public float UnitMoveDuration => unitMoveDuration;
    public Unit Player => player;
    
    public static GameManager Instance {
        get { return _instance; }
        private set { }
    }

    private static GameManager _instance;

    public static AudioManager AudioManager { get; private set; }

    private Coroutine moveUnitRoutine;
    private UnitManager unitManager;
    private Camera mainCamera;
    private GameObject gridParent;
    private Unit player;
    private Unit exit;
    private int currentGridIndex;

    private bool playerIsDead;
    private bool gameIsOver;
    
    private enum Turn { Player, NPC }
    private Turn turn;

    private void Awake() {
        if (_instance == null) {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        } else {
            Destroy(gameObject);
        }
        Application.runInBackground = true;
        Cursor.visible = true;
        mainCamera = GetComponentInChildren<Camera>();
        AudioManager = GetComponentInChildren<AudioManager>();
        unitManager = new();
    }

    private IEnumerator Start() {
        StartGame();
        yield break;
    }

    private void StartGame() {
        Generate(grids[currentGridIndex]);
    }

    public void EndGame(bool died) {
        if(died) {
            AudioManager.PlayPlayerDied();
            player.GetComponent<SpriteRenderer>().sprite = deadPlayerSprite;
            Debug.Log("player is dead");
        } else {
            AudioManager.PlayPlayerWon();
            Debug.Log("Game has won");
        }
        playerIsDead = died;
        gameIsOver = true;
        for (int i = 0; i < Grid.GetLength(0); i++) {
            for (int j = 0; j < Grid.GetLength(1); j++) {
                Grid[i, j].SetColor(Color.white);
            }
        }
    }

    private void Generate(GridData gridData) {

        // Generate Grid
        if (gridParent == null) {
            gridParent = new GameObject("GridParent");
        }
        Grid = new Tile[gridData.Width, gridData.Height];
        for (int i = 0; i < Grid.GetLength(0); i++) {
            for (int j = 0; j < Grid.GetLength(1); j++) {
                Grid[i, j] = Instantiate(gridData.TilePrefab, transform.position + i * TILE_RIGHT_UNIT + j * TILE_UP_UNIT, Quaternion.identity, gridParent.transform).GetComponent<Tile>();
                Grid[i, j].Index = new(i, j);
                Grid[i, j].gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = Grid.GetLength(0) - i + Grid.GetLength(1) - j;
            }
        }

        // Spawn Player
        playerIsDead = false;
        Tile playerTile = Grid[gridData.PlayerSpawn.x, gridData.PlayerSpawn.y];
        player = Instantiate(playerPrefab, playerTile.transform.position + spawnOffset, Quaternion.identity).GetComponent<Unit>();
        player.Index = playerTile.Index;
        playerTile.Unit = player;

        // Spawn Exit
        Tile exitTile = Grid[gridData.ExitSpawn.x, gridData.ExitSpawn.y];
        if (currentGridIndex == grids.Length - 1) {
            // This is the last level
            exit = Instantiate(finalExitPrefab, exitTile.transform.position + spawnOffset, Quaternion.identity).GetComponent<Unit>();
        } else {
            exit = Instantiate(exitPrefab, exitTile.transform.position + spawnOffset, Quaternion.identity).GetComponent<Unit>();
        }
        exit.Index = exitTile.Index;
        exitTile.Unit = exit;

        // Spawn all Units
        unitManager.Generate(gridData, gridData.Units, spawnOffset);

        ReorderUnitSortingOrders();

        ShowAvailablePlayerMoves();

        if (updateRoutine != null) StopCoroutine(updateRoutine);
        updateRoutine = StartCoroutine(UpdateTurn());
    }

    private Coroutine updateRoutine;

    private void OnExitReached() {
        for (int i = 0; i < Grid.GetLength(0); i++) {
            for (int j = 0; j < Grid.GetLength(1); j++) {
                Grid[i, j].SetColor(Color.white);
            }
        }
        StartCoroutine(OnExitReachedRoutine());
    }

    private Transition transition;

    private IEnumerator OnExitReachedRoutine() {

        if (currentGridIndex == grids.Length - 1) {
            //AudioManager.PlayPlayerWon();
        } else {
            AudioManager.PlayLadder();
        }

        transition = Instantiate(transitionPrefab).GetComponent<Transition>();

        yield return new WaitForSeconds(1f);

        unitManager.Clear();

        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units) {
            Destroy(unit.gameObject);
        }
        Destroy(gridParent);
        gridParent = null;

        currentGridIndex++;
        if (currentGridIndex == grids.Length) {
            EndGame(false);
        } else {
            Generate(grids[currentGridIndex]);
            transition.OnGridLoaded();
        }
    }

    private void ReorderUnitSortingOrders() {
        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units) {
            unit.SetSortingOrder(Grid.GetLength(0) - unit.Index.x + Grid.GetLength(1) - unit.Index.y);
        }
    }

    private void Update() {
        if(gameIsOver) {
            if(Input.anyKeyDown) {
                gameIsOver = false;
                StartCoroutine(DoGameEnd());
            }
            return;
        }
        UpdatePlayer();
        if (player != null) {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, player.transform.position + new Vector3(0, 0, -5) + cameraOffset, Time.deltaTime * cameraSpeed);
        }
    }

    private IEnumerator DoGameEnd() {

        if (playerIsDead == false) {
            if (transition) transition.OnGridLoaded();
            else transition = null;
        }

        Transition newTransition = Instantiate(transitionPrefab).GetComponent<Transition>();

        yield return new WaitForSeconds(1f);

        unitManager.Clear();

        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units) {
            Destroy(unit.gameObject);
        }
        Destroy(gridParent);
        gridParent = null;

        if(playerIsDead == false) {
            currentGridIndex = 0;
        }
        StartGame();

        newTransition.OnGridLoaded();
    }

    private IEnumerator MoveUnitToTile(Unit unit, Tile tile) {
        AudioManager.PlayPlayerMove();
        float time = 0;
        Vector3 startPos = unit.transform.position;
        while (time <= unitMoveDuration) {
            unit.transform.position = Vector3.Lerp(startPos, tile.transform.position + spawnOffset, time / unitMoveDuration);
            time += Time.deltaTime;
            yield return null;
        }
        unit.transform.position = tile.transform.position + spawnOffset;
        moveUnitRoutine = null;
        ReorderUnitSortingOrders();

        if (tile.Unit == exit) {
            OnExitReached();
        } else {
            turn = Turn.NPC;
        }
        Grid[player.Index.x, player.Index.y].Unit = null;
        player.Index = tile.Index;
        tile.Unit = player;
        yield break;
    }

    private IEnumerator UpdateTurn() {

        // NPC turn
        yield return new WaitUntil(() => turn == Turn.NPC);

        for (int i = 0; i < Grid.GetLength(0); i++) {
            for (int j = 0; j < Grid.GetLength(1); j++) {
                Grid[i, j].SetColor(Color.white);
            }
        }

        yield return new WaitUntil(() => moveUnitRoutine == null);

        yield return unitManager.Update(ReorderUnitSortingOrders);

        yield return new WaitForSeconds(0.1f);

        // Player turn
        turn = Turn.Player;
        ShowAvailablePlayerMoves();
        updateRoutine = StartCoroutine(UpdateTurn());
        yield break;
    }

    private void ShowAvailablePlayerMoves() {

        Tile tile = Grid[player.Index.x, player.Index.y];

        bool hasAvailableMoves = false;

        if (tile.Index.x + 1 < Grid.GetLength(0)) {
            if (Grid[tile.Index.x + 1, tile.Index.y].Unit) {
                if(Grid[tile.Index.x + 1, tile.Index.y].Unit == exit) {
                    Grid[tile.Index.x + 1, tile.Index.y].SetColor(Color.green);
                    hasAvailableMoves = true;
                } else {
                    Grid[tile.Index.x + 1, tile.Index.y].SetColor(Color.red);
                }
            } else {
                Grid[tile.Index.x + 1, tile.Index.y].SetColor(Color.green);
                hasAvailableMoves = true;
            }
        }
        if (tile.Index.x - 1 >= 0) {
            if (Grid[tile.Index.x - 1, tile.Index.y].Unit) {
                if (Grid[tile.Index.x - 1, tile.Index.y].Unit == exit) {
                    Grid[tile.Index.x - 1, tile.Index.y].SetColor(Color.green);
                    hasAvailableMoves = true;
                } else {
                    Grid[tile.Index.x - 1, tile.Index.y].SetColor(Color.red);
                }
            } else {
                Grid[tile.Index.x - 1, tile.Index.y].SetColor(Color.green);
                hasAvailableMoves = true;
            }
        }
        if (tile.Index.y + 1 < Grid.GetLength(1)) {
            if (Grid[tile.Index.x, tile.Index.y + 1].Unit) {
                if (Grid[tile.Index.x, tile.Index.y + 1].Unit == exit) {
                    Grid[tile.Index.x, tile.Index.y + 1].SetColor(Color.green);
                    hasAvailableMoves = true;
                } else {
                    Grid[tile.Index.x, tile.Index.y + 1].SetColor(Color.red);
                }
            } else {
                Grid[tile.Index.x, tile.Index.y + 1].SetColor(Color.green);
                hasAvailableMoves = true;
            }
        }
        if (tile.Index.y - 1 >= 0) {
            if (Grid[tile.Index.x, tile.Index.y - 1].Unit) {
                if (Grid[tile.Index.x, tile.Index.y - 1].Unit == exit) {
                    Grid[tile.Index.x, tile.Index.y - 1].SetColor(Color.green);
                    hasAvailableMoves = true;
                } else {
                    Grid[tile.Index.x, tile.Index.y - 1].SetColor(Color.red);
                }
            } else {
                Grid[tile.Index.x, tile.Index.y - 1].SetColor(Color.green);
                hasAvailableMoves = true;
            }
        }

        if(hasAvailableMoves == false) {
            EndGame(true);
        }
    }

    private Tile prevSelectedTile;

    private void UpdatePlayer() {

        if (moveUnitRoutine != null) return;
        if (turn == Turn.NPC) return;

        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, 20, playerLayerMask);

        if (hit.collider) {

            Tile tile = hit.collider.GetComponent<Tile>();

            if(prevSelectedTile != null && tile != prevSelectedTile) {
                prevSelectedTile.RemoveHighlight();
            }

            prevSelectedTile = tile;

            tile.AddHighlight();

            if (Input.GetMouseButtonDown(0)) {

                Tile playerTile = Grid[player.Index.x, player.Index.y];

                if ((tile.Index.x == playerTile.Index.x + 1 && tile.Index.y == playerTile.Index.y) ||
                    (tile.Index.x == playerTile.Index.x - 1 && tile.Index.y == playerTile.Index.y) ||
                    (tile.Index.x == playerTile.Index.x && tile.Index.y == playerTile.Index.y + 1) ||
                    (tile.Index.x == playerTile.Index.x && tile.Index.y == playerTile.Index.y - 1)) {

                    if (tile.Index.x >= 0 && tile.Index.x < Grid.GetLength(0) &&
                        tile.Index.y >= 0 && tile.Index.y < Grid.GetLength(1)) {
                        // Move the Player
                        if (tile.Unit == null) {
                            moveUnitRoutine = StartCoroutine(MoveUnitToTile(player, tile));
                            
                        } else {
                            if(tile.Unit == exit) {
                                moveUnitRoutine = StartCoroutine(MoveUnitToTile(player, tile));
                            } else {
                                // Is blocked by NPC
                            }
                        }
                    } else {
                        // out of grid
                    }
                } else {
                    // Is occupied
                }
            }
        } else {
            if(prevSelectedTile) {
                prevSelectedTile.RemoveHighlight();
                prevSelectedTile = null;
            }
        }
    }
}