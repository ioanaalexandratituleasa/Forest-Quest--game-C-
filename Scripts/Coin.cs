using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
        if (playerInventory != null)
        {
            playerInventory.CoinCollected();
            Vector2Int gridPos = MazeGenerator.Instance.WorldToGrid(transform.position);
            MazeGenerator.Instance.maze[gridPos.y, gridPos.x] = 0;
            gameObject.SetActive(false);
        }
    }

}
