using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Grid Grid { get; private set; }

    public static GameManager Instance {
        get { return _instance; }
        private set { }
    }

    private static GameManager _instance;

    private void Awake() {
        if (_instance == null) {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        } else {
            Destroy(gameObject);
        }
        Application.runInBackground = true;
        Cursor.visible = true;
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