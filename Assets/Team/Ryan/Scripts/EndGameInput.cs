using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class EndGameInput : MonoBehaviour
{
    public ToyCollectionZone toyZone;

    [Header("End Screen")]
    public GameObject endScreen;
    public TMP_Text finalScoreText;

    [Header("Extra Events")]
    public UnityEvent onEndGame;

    private bool gameEnded = false;

    private void Start()
    {
        // hide the end screen until the game is over
        if (endScreen != null)
        {
            endScreen.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameEnded)
        {
            return;
        }

        // end the game straight away once every toy is in the zone
        if (toyZone.totalToys > 0 && toyZone.currentToys == toyZone.totalToys)
        {
            Debug.Log("End Game: all toys collected");
            EndGame();
        }

        // pressing enter lets the player end early with only some of the toys
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Debug.Log("End Game: enter pressed");
            EndGame();
        }
    }

    public void EndGame()
    {
        // stop the game ending more than once
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;
        Debug.Log("End Game: game ended with " + toyZone.currentToys + " / " + toyZone.totalToys + " toys");

        if (endScreen != null)
        {
            endScreen.SetActive(true);
        }

        // unlock the mouse so the player can click on the end screen
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (finalScoreText != null)
        {
            finalScoreText.text = "You collected " + toyZone.currentToys + " / " + toyZone.totalToys + " toys!";
        }

        onEndGame.Invoke();
    }
}
