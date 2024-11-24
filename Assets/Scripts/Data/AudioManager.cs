using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource unitSpawnSound;
    [SerializeField] private AudioSource unitMoveSound;       // For Move method
    [SerializeField] private AudioSource unitPathSound;       // For ExecutePath
    [SerializeField] private AudioSource unitClearSound;     //       // For Clear method

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayUnitSpawnSound()
    {
        if (unitSpawnSound != null)
            //sfxSource.PlayOneShot();
            unitSpawnSound.Play();
    }

    public void PlayUnitMoveSound()
    {
        //if (unitMoveSound != null)
        unitMoveSound.Play();

    }

    public void PlayUnitPathSound()
    {
        if (unitPathSound != null)
        unitPathSound.Play();
    }

    public void PlayUnitClearSound()
    {
        if (unitClearSound != null)
            unitClearSound.Play();
    }
}