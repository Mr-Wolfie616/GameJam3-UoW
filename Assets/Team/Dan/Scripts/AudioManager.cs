using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Tracks/Clips")]
    [SerializeField] AudioClip[] meowSFX;
    [SerializeField] AudioClip[] splashSFX;
    [SerializeField] AudioClip coinSFX;
    [Header("References")]
    [SerializeField] AudioSource sfxSource;
    [SerializeField] InputAction meowButton;

    private void OnEnable()
    {
        meowButton.Enable();
    }

    private void OnDisable()
    {
        meowButton.Disable();
    }

    private void Update()
    {
        if (meowButton.triggered)
        {
            int i = Random.Range(0, meowSFX.Length);
            sfxSource.PlayOneShot(meowSFX[i]);
        }
    }

    public void SplashSFX()
    {
        int i = Random.Range(0, splashSFX.Length);
        sfxSource.PlayOneShot(splashSFX[i]);
    }

    public void CoinSFX()
    {
        sfxSource.PlayOneShot(coinSFX);
    }
}