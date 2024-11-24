
using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] LayerMask playerLayerMask;
    [SerializeField] GameObject playerPrefab;
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
    
    public static GameManager Instance {
        get { return _instance; }
        private set { }
    }

    private static GameManager _instance;

    private Coroutine moveUnitRoutine;
    private UnitManager unitManager;
    private Camera mainCamera;
    private GameObject gridParent;
    private Unit player;
    private Unit exit;
    private int currentGridIndex;
    
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
        unitManager = new();
    }

    private IEnumerator Start() {
        StartGame();
        yield break;
    }

    private void StartGame() {
        Generate(grids[currentGridIndex]);
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

        // TODO : do final exit if this is the last grid instead of exit ladder

        // Spawn all Units
        unitManager.Generate(gridData, gridData.Units, spawnOffset);

        ReorderUnitSortingOrders();

        ShowAvailablePlayerMoves();
        StartCoroutine(UpdateTurn());
    }

    private void OnExitReached() {
        for (int i = 0; i < Grid.GetLength(0); i++) {
            for (int j = 0; j < Grid.GetLength(1); j++) {
                Grid[i, j].SetColor(Color.white);
            }
        }
        StartCoroutine(OnExitReachedRoutine());
    }

    private IEnumerator OnExitReachedRoutine() {

        Transition transition = Instantiate(transitionPrefab).GetComponent<Transition>();

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
            // TODO : game has ended
        } else {
            Generate(grids[currentGridIndex]);
        }

        transition.OnGridLoaded();
    }

    private void ReorderUnitSortingOrders() {
        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units) {
            unit.SetSortingOrder(Grid.GetLength(0) - unit.Index.x + Grid.GetLength(1) - unit.Index.y);
        }
    }

    private void Update() {
        UpdatePlayer();
        if (player != null) {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, player.transform.position + new Vector3(0, 0, -5) + cameraOffset, Time.deltaTime * cameraSpeed);
        }
    }

    private IEnumerator MoveUnitToTile(Unit unit, Tile tile) {
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
        yield break;
    }

    private IEnumerator UpdateTurn() {

        // NPC turn
        yield return new WaitUntil(() => turn == Turn.NPC);
        yield return new WaitUntil(() => moveUnitRoutine == null);

        unitManager.Update();

        // Player turn
        turn = Turn.Player;
        ShowAvailablePlayerMoves();
        StartCoroutine(UpdateTurn());
        yield break;
    }

    private void ShowAvailablePlayerMoves() {

        for (int i = 0; i < Grid.GetLength(0); i++) {
            for (int j = 0; j < Grid.GetLength(1); j++) {
                Grid[i, j].SetColor(Color.white);
            }
        }

        Tile tile = Grid[player.Index.x, player.Index.y];

        if (tile.Index.x + 1 < Grid.GetLength(0)) {
            if (Grid[tile.Index.x + 1, tile.Index.y].Unit) {
                if(Grid[tile.Index.x + 1, tile.Index.y].Unit == exit) {
                    Grid[tile.Index.x + 1, tile.Index.y].SetColor(Color.green);
                } else {
                    Grid[tile.Index.x + 1, tile.Index.y].SetColor(Color.red);
                }
            } else {
                Grid[tile.Index.x + 1, tile.Index.y].SetColor(Color.green);
            }
        }
        if (tile.Index.x - 1 >= 0) {
            if (Grid[tile.Index.x - 1, tile.Index.y].Unit) {
                if (Grid[tile.Index.x - 1, tile.Index.y].Unit == exit) {
                    Grid[tile.Index.x - 1, tile.Index.y].SetColor(Color.green);
                } else {
                    Grid[tile.Index.x - 1, tile.Index.y].SetColor(Color.red);
                }
            } else {
                Grid[tile.Index.x - 1, tile.Index.y].SetColor(Color.green);
            }
        }
        if (tile.Index.y + 1 < Grid.GetLength(1)) {
            if (Grid[tile.Index.x, tile.Index.y + 1].Unit) {
                if (Grid[tile.Index.x, tile.Index.y + 1].Unit == exit) {
                    Grid[tile.Index.x, tile.Index.y + 1].SetColor(Color.green);
                } else {
                    Grid[tile.Index.x, tile.Index.y + 1].SetColor(Color.red);
                }
            } else {
                Grid[tile.Index.x, tile.Index.y + 1].SetColor(Color.green);
            }
        }
        if (tile.Index.y - 1 >= 0) {
            if (Grid[tile.Index.x, tile.Index.y - 1].Unit) {
                if (Grid[tile.Index.x, tile.Index.y - 1].Unit == exit) {
                    Grid[tile.Index.x, tile.Index.y - 1].SetColor(Color.green);
                } else {
                    Grid[tile.Index.x, tile.Index.y - 1].SetColor(Color.red);
                }
            } else {
                Grid[tile.Index.x, tile.Index.y - 1].SetColor(Color.green);
            }
        }
    }

    private void UpdatePlayer() {

        if (moveUnitRoutine != null) return;
        if (turn == Turn.NPC) return;

        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, 20, playerLayerMask);

        if (hit.collider) {

            Tile tile = hit.collider.GetComponent<Tile>();

            // TODO : highlight player moves at start of player turn

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
                            player.Index = tile.Index;
                            tile.Unit = player;
                            playerTile.Unit = null;
                            turn = Turn.NPC;
                        } else {
                            if(tile.Unit == exit) {
                                moveUnitRoutine = StartCoroutine(MoveUnitToTile(player, tile));
                                player.Index = tile.Index;
                                tile.Unit = player;
                                playerTile.Unit = null;

                                OnExitReached();

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
        }
    }
}
    /*
    [Header("Menu")]
    [SerializeField] Animator introAnimator;
    [SerializeField] Animator outroAnimator;

    [Header("Audio")]
    [SerializeField] AudioSource IntroMusic;
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] AudioSource clickSound;
    [SerializeField] AudioSource writingSound;

    [Header("Cameras")]
    [SerializeField] CinemachineVirtualCamera playerVC;
    [SerializeField] CinemachineVirtualCamera nonPlayerVC;
    [SerializeField] CinemachineBrain cinemachineBrain;
    [SerializeField] Animator cinematicBars;

    [Header("Animation")]
    [SerializeField] Animator character_timur;
    [SerializeField] Animator character_poet;
    [SerializeField] Animator character_guard;

    private DialogeData currentDialogue;

    public int BalanceValue { get; private set; } = 0;


    private DialogueNode currentNode;
    private DialogueNodeUI currentDialogueNodeUI;
    private GameObject prevDialogueNodeInstance;
    private List<GameObject> playerDialogueInstances = new List<GameObject>();
    */


    /*
    private IEnumerator Start() {
        //yield return new WaitUntil(() => Input.anyKeyDown);
        //FadeAudio(null, IntroMusic, 1);
        //introAnimator.SetTrigger("Intro1");
        //yield return new WaitForSeconds(1f);
        //yield return new WaitUntil(() => Input.anyKeyDown);
        //introAnimator.SetTrigger("Intro2");
        //yield return new WaitForSeconds(1f);
        //yield return new WaitUntil(() => Input.anyKeyDown);
        //introAnimator.SetTrigger("IntroEnd");
        //FadeAudio(IntroMusic, backgroundMusic, 1);
        //yield return new WaitForSeconds(1f);
        //StartDialogue(dialogueData_Intro);
        yield break;
    }

    private void FadeAudio(AudioSource origin, AudioSource target, float duration)
    {
        StartCoroutine(FadeAudioRoutine(origin, target, duration));
    }

    private IEnumerator FadeAudioRoutine(AudioSource origin, AudioSource target, float duration)
    {
        float timeElapsed = 0;
        float startVolumeOrigin = 1, targetVolumeTarget = 1;
        if (origin) startVolumeOrigin = origin.volume;
        if (target)
        {
            targetVolumeTarget = target.volume;
            target.Play();
        }
        while (timeElapsed <= duration)
        {
            if(origin) origin.volume = Mathf.Lerp(startVolumeOrigin, 0, timeElapsed / duration);
            if (target) target.volume = Mathf.Lerp(0, targetVolumeTarget, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        if (origin) origin.Stop();
    }
}
    */