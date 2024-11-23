
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] LayerMask playerLayerMask;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] float cameraSpeed;
    [SerializeField] Vector3 spawnOffset;
    [SerializeField] GridData [] grids;

    private Vector3 TILE_RIGHT_UNIT = new Vector2(1.2f, 0.7f);
    private Vector3 TILE_UP_UNIT = new Vector2(-1.2f, 0.7f);

    public Grid Grid { get; private set; }

    public static GameManager Instance {
        get { return _instance; }
        private set { }
    }

    private static GameManager _instance;

    private UnitManager unitManager;
    private Camera mainCamera;
    private GameObject gridParent;
    private GameObject player;

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
        Grid = new(gridData);
        for (int i = 0; i < Grid.Points.GetLength(0); i++) {
            for (int j = 0; j < Grid.Points.GetLength(1); j++) {
                Grid.Points[i,j].gameObject = Instantiate(gridData.TilePrefab, transform.position + i * TILE_RIGHT_UNIT + j * TILE_UP_UNIT, Quaternion.identity, gridParent.transform);
                Grid.Points[i, j].gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = Grid.Points.GetLength(0) - i + Grid.Points.GetLength(1) - j;
            }
        }

        // Spawn Player
        player = Instantiate(playerPrefab, Grid.Points[0, 0].gameObject.transform.position + spawnOffset, Quaternion.identity);
        // TODO : marke Grid.Points[0, 0]. as taken by player

        // TODO : place all characaters and obstacles
        unitManager.Generate(gridData.Units);
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)) {
            RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, 20, playerLayerMask);
            if (hit) {
                player.transform.position = hit.collider.transform.position + spawnOffset;
            }
        }
        // Update Camera
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, player.transform.position + new Vector3 (0,0,-5), Time.deltaTime * cameraSpeed);
        unitManager.Move();
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

    private void Update() {
        if (currentDialogue == null) return;
        if(Input.anyKeyDown) {
            if(playerDialogueInstances.Count <= 1 && !currentNode.IsPoetry) {
                currentDialogueNodeUI.OnNodeSelected();
            }
        }
    }
    */
}