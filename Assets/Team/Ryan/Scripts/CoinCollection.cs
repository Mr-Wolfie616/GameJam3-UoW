using UnityEngine;

public class CoinCollection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            CollectCoin(other.gameObject);
        }
    }

    void CollectCoin(GameObject coin)
    {
        // add vfx / sound here later

        Destroy(coin);
    }
}
