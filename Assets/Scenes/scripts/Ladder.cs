using UnityEngine;
using SupanthaPaul; // Добавлено пространство имен, где находится PlayerController

public class Ladder : MonoBehaviour
{
    [SerializeField] private BoxCollider2D topCollider; // Коллайдер вверху лестницы

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetCanClimbLadder(true, topCollider);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetCanClimbLadder(false);
            }
        }
    }
}
