using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToyCollectionZone : MonoBehaviour
{
    public int currentToys = 0;
    public int totalToys = 0;

    // hook the UI up to this, then read currentToys and totalToys
    public UnityEvent onScoreChanged;
    public UnityEvent onAllToysCollected;

    private List<Rigidbody> toysInZone = new List<Rigidbody>();

    private void Start()
    {
        // count every toy in the scene to get the total
        totalToys = GameObject.FindGameObjectsWithTag("Toy").Length;
        UpdateScore();
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        // only count toys that have been dropped, not ones still in the mouth
        if (rb == null || !rb.CompareTag("Toy") || rb.isKinematic) { return; }

        if (!toysInZone.Contains(rb))
        {
            toysInZone.Add(rb);
            UpdateScore();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb != null && toysInZone.Contains(rb))
        {
            toysInZone.Remove(rb);
            UpdateScore();
        }
    }

    private void Update()
    {
        // picking a toy up turns its collider off which doesnt call OnTriggerExit
        // so remove any toys that have been picked back up from inside the zone
        for (int i = toysInZone.Count - 1; i >= 0; i--)
        {
            if (toysInZone[i] == null || toysInZone[i].isKinematic)
            {
                toysInZone.RemoveAt(i);
                UpdateScore();
            }
        }
    }

    private void UpdateScore()
    {
        currentToys = toysInZone.Count;
        onScoreChanged.Invoke();

        if (totalToys > 0 && currentToys == totalToys)
        {
            Debug.Log("All toys have been collected.");
            onAllToysCollected.Invoke();
        }
    }
}
