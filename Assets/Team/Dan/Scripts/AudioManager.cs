using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Tracks/Clips")]
    [SerializeField] AudioClip backgroundMusic;
    [SerializeField] AudioClip meowSFX;
    [SerializeField] AudioClip splashSFX;
    [Header("References")]
    [SerializeField] AudioSource musicSource;
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

    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
        musicSource.loop = true;
    }

    private void Update()
    {
        if (meowButton.triggered) sfxSource.PlayOneShot(meowSFX);
    }

    public void SplashSFX()
    {
        sfxSource.PlayOneShot(splashSFX);
    }

    public void test()
    {
        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        audioManager.SplashSFX();
    }
}