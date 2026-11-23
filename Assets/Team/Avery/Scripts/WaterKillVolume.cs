using UnityEngine;

public class WaterKillVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //other.GetComponent<PlayerHealth>().Kill();
            Debug.Log("Player has entered the water kill volume.");
        }
    }
}
