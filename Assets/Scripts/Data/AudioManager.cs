using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource ladderSound;
    [SerializeField] private AudioSource playerWonSound;
    [SerializeField] private AudioSource playerDiedSound;
    [SerializeField] private AudioSource playerMoveSound;
    [SerializeField] private AudioSource unitMoveSound;

    public void PlayLadder()
    {
        ladderSound.Play();
    }

    public void PlayPlayerMove() {
        playerMoveSound.Play();
    }

    public void PlayUnitMove()
    {
        unitMoveSound.Play();
    }

    public void PlayPlayerDied()
    {
        playerDiedSound.Play();
    }

    public void PlayPlayerWon() {
        playerWonSound.Play();
    }
}