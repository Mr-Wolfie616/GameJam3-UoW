using UnityEngine;

public class WaterKillVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //other.GetComponent<PlayerHe
            //alth>().Kill();
            Debug.Log("Player has entered the water kill volume.");
        }
    }
}
