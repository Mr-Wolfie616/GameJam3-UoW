using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

// put this on an object with a trigger collider
// it counts how many toys have been dropped inside it
public class ToyCollectionZone : MonoBehaviour
{
    [Header("Score UI")]
    public TMP_Text scoreText;

    [Header("Score")]
    public int currentToys = 0;
    public int totalToys = 0;

    [Header("Extra Events")]
    public UnityEvent onScoreChanged;
    public UnityEvent onAllToysCollected;

    private List<Rigidbody> toysInZone = new List<Rigidbody>();

    private void Start()
    {
        // count every toy in the scene to get the total
        totalToys = GameObject.FindGameObjectsWithTag("Toy").Length;
        Debug.Log("Toy Zone: found " + totalToys + " toys");

        UpdateScore();
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        // only count toys that have been dropped, not ones still in the cats mouth
        if (rb == null || !rb.CompareTag("Toy") || rb.isKinematic)
        {
            return;
        }

        if (!toysInZone.Contains(rb))
        {
            toysInZone.Add(rb);
            Debug.Log("Toy Zone: " + rb.name + " dropped in");
            UpdateScore();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb != null && toysInZone.Contains(rb))
        {
            toysInZone.Remove(rb);
            Debug.Log("Toy Zone: " + rb.name + " left the zone");
            UpdateScore();
        }
    }

    private void Update()
    {
        // picking a toy up turns its collider off, which doesnt call OnTriggerExit
        // so remove any toys that have been picked back up
        for (int i = toysInZone.Count - 1; i >= 0; i--)
        {
            if (toysInZone[i] == null || toysInZone[i].isKinematic)
            {
                toysInZone.RemoveAt(i);
                Debug.Log("Toy Zone: a toy was picked back up");
                UpdateScore();
            }
        }
    }

    private void UpdateScore()
    {
        currentToys = toysInZone.Count;
        Debug.Log("Toy Zone: " + currentToys + " / " + totalToys + " toys collected");

        if (scoreText != null)
        {
            scoreText.text = currentToys + " / " + totalToys;
        }

        onScoreChanged.Invoke();

        if (totalToys > 0 && currentToys == totalToys)
        {
            Debug.Log("Toy Zone: all toys collected");
            onAllToysCollected.Invoke();
        }
    }
}
