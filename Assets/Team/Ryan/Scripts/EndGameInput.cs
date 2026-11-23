using UnityEngine;
using UnityEngine.Events;

public class EndGameInput : MonoBehaviour
{
    public ToyCollectionZone toyZone;

    // hook the end game method up to this
    public UnityEvent onEndGame;

    private bool gameEnded = false;

    private void Update()
    {
        if (gameEnded) { return; }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        gameEnded = true;
        Debug.Log("Game ended with " + toyZone.currentToys + " / " + toyZone.totalToys + " toys.");
        onEndGame.Invoke();
    }
}
