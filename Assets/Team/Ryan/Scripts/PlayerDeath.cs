using UnityEngine;

// shows the death screen when the cat has used up both lives
// hook ShowDeathScreen up to PlayerHealth's onPlayerFullyKilled event in the Inspector
public class PlayerDeath : MonoBehaviour
{
    [Header("Death Screen")]
    public GameObject deathPanel;

    [Header("Things To Stop")]
    public PlayerController playerController;
    public ThirdPersonCameraRig cameraRig;
    public WaterRise waterRise;
    public EndGameInput endGameInput;

    private bool isDead = false;

    private void Start()
    {
        // hide the death screen until the player dies
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
    }

    public void ShowDeathScreen()
    {
        // stop the death screen showing more than once
        if (isDead)
        {
            return;
        }

        isDead = true;
        Debug.Log("Player Death: out of lives, showing death screen");

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }

        // stop the cat moving
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // stop the camera turning with the mouse
        if (cameraRig != null)
        {
            cameraRig.enabled = false;
        }

        // stop the water rising any higher
        if (waterRise != null)
        {
            waterRise.StopRising();
        }

        // stop enter bringing up the end screen on top of the death screen
        if (endGameInput != null)
        {
            endGameInput.enabled = false;
        }

        // unlock the mouse so the player can click the death screen buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
