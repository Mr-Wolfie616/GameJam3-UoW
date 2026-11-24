using UnityEngine;
using UnityEngine.SceneManagement;

// resets the level by reloading the scene
// hook RestartGame up to the Start Game button's On Click in the Inspector
public class GameManager : MonoBehaviour
{
    // the main menu, so it can be skipped after a restart
    public GameObject mainMenuPanel;

    // static so it survives the scene reloading (normal variables get reset)
    private static bool skipMenu = false;

    private void Start()
    {
        // if the scene was reloaded from the Start Game button, go straight into the game
        if (skipMenu)
        {
            skipMenu = false;

            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(false);
            }

            Debug.Log("Game Manager: game restarted");
        }
    }

    public void RestartGame()
    {
        skipMenu = true;

        // make sure time is running normally in case anything paused it
        Time.timeScale = 1f;

        // reload the current scene, which puts the cat, toys, water and score back to the start
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
