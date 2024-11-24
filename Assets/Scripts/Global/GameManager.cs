
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] LayerMask playerLayerMask;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] float cameraSpeed;
    [SerializeField] Vector3 spawnOffset;
    [SerializeField] GridData[] grids;

    private Vector3 TILE_RIGHT_UNIT = new Vector2(1.2f, 0.7f);
    private Vector3 TILE_UP_UNIT = new Vector2(-1.2f, 0.7f);

    public Tile[,] Grid { get; private set; }
    
    public static GameManager Instance {
        get { return _instance; }
        private set { }
    }

    private static GameManager _instance;

    private UnitManager unitManager;
    private Camera mainCamera;
    private GameObject gridParent;
    private GameObject player;
    private Tile playerTile;
    
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
        Generate(grids[0]);
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
        player = Instantiate(playerPrefab, Grid[0, 0].gameObject.transform.position + spawnOffset, Quaternion.identity);
        playerTile = Grid[0, 0];

        // Spawn all Units
        unitManager.Generate(gridData.Units,spawnOffset);
        StartCoroutine(UpdateTurn());
    }

    private void Update() {
        UpdatePlayer();
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, player.transform.position + new Vector3(0, 0, -5), Time.deltaTime * cameraSpeed);
    }

    private IEnumerator UpdateTurn() {

        yield return new WaitUntil(() => turn == Turn.NPC);
        turn = Turn.Player;
        unitManager.Update();
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

        Tile tile = playerTile;

        if (tile.Index.x + 1 < Grid.GetLength(0)) {
            if (Grid[(int)tile.Index.x + 1, (int)tile.Index.y].Unit) {
                Grid[(int)tile.Index.x + 1, (int)tile.Index.y].SetColor(Color.red);
            } else {
                Grid[(int)tile.Index.x + 1, (int)tile.Index.y].SetColor(Color.green);
            }
        }
        if (tile.Index.x - 1 >= 0) {
            if (Grid[(int)tile.Index.x - 1, (int)tile.Index.y].Unit) {
                Grid[(int)tile.Index.x - 1, (int)tile.Index.y].SetColor(Color.red);
            } else {
                Grid[(int)tile.Index.x - 1, (int)tile.Index.y].SetColor(Color.green);
            }
        }
        if (tile.Index.y + 1 < Grid.GetLength(1)) {
            if (Grid[(int)tile.Index.x, (int)tile.Index.y + 1].Unit) {
                Grid[(int)tile.Index.x, (int)tile.Index.y + 1].SetColor(Color.red);
            } else {
                Grid[(int)tile.Index.x, (int)tile.Index.y + 1].SetColor(Color.green);
            }
        }
        if (tile.Index.y - 1 >= 0) {
            if (Grid[(int)tile.Index.x, (int)tile.Index.y - 1].Unit) {
                Grid[(int)tile.Index.x, (int)tile.Index.y - 1].SetColor(Color.red);
            } else {
                Grid[(int)tile.Index.x, (int)tile.Index.y - 1].SetColor(Color.green);
            }
        }
    }

    private void UpdatePlayer() {

        if (turn == Turn.NPC) return;

        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, 20, playerLayerMask);

        if (hit.collider) {

            Tile tile = hit.collider.GetComponent<Tile>();

            // TODO : highlight player moves at start of player turn

            if (Input.GetMouseButtonDown(0)) {

                if ((tile.Index.x == playerTile.Index.x + 1 && tile.Index.y == playerTile.Index.y) ||
                    (tile.Index.x == playerTile.Index.x - 1 && tile.Index.y == playerTile.Index.y) ||
                    (tile.Index.x == playerTile.Index.x && tile.Index.y == playerTile.Index.y + 1) ||
                    (tile.Index.x == playerTile.Index.x && tile.Index.y == playerTile.Index.y - 1)) {

                    if (tile.Index.x >= 0 && tile.Index.x < Grid.GetLength(0) &&
                        tile.Index.y >= 0 && tile.Index.y < Grid.GetLength(1)) {
                        // Move the Player
                        if (tile.Unit == null) {
                            player.transform.position = hit.collider.transform.position + spawnOffset;
                            playerTile = tile;
                            turn = Turn.NPC;
                        } else {
                            // Is occupied
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