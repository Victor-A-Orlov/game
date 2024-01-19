using UnityEngine;

namespace SupanthaPaul
{
    public class Ladder : MonoBehaviour
    {
        private bool isPlayerOnLadder = false; // Флаг, указывающий на присутствие игрока на лестнице
        private GameObject player = null; // Ссылка на игрока

        private void Update()
        {
            if (isPlayerOnLadder) // Если игрок находится на лестнице
            {
                float verticalInput = Input.GetAxis("Vertical"); // Получаем вертикальное значение ввода от игрока
                if (verticalInput > 0f) // Если игрок двигается вверх
                {
                    MoveUpLadder(); // Вызываем метод для перемещения игрока вверх по лестнице
                }
                else if (verticalInput < 0f) // Если игрок двигается вниз
                {
                    MoveDownLadder(); // Вызываем метод для перемещения игрока вниз по лестнице
                }

                if (Input.GetKeyDown(KeyCode.E)) // Если игрок нажимает клавишу "E"
                {
                    DetachFromLadder(); // Вызываем метод для отсоединения игрока от лестницы
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) // Если столкновение с объектом, помеченным тегом "Player"
            {
                isPlayerOnLadder = true; // Устанавливаем флаг, что игрок находится на лестнице
                player = collision.gameObject; // Сохраняем ссылку на игрока
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) // Если объект, помеченный тегом "Player", покидает столкновение
            {
                isPlayerOnLadder = false; // Сбрасываем флаг, что игрок находится на лестнице
                player = null; // Сбрасываем ссылку на игрока
            }
        }

		private void MoveUpLadder()
		{
			Rigidbody2D rb = player.GetComponent<Rigidbody2D>(); // Получаем компонент Rigidbody2D игрока
			PlayerController playerController = player.GetComponent<PlayerController>(); // Получаем компонент PlayerController игрока

			rb.Sleep(); // Отключаем физику объекта rb
			rb.velocity = new Vector2(rb.velocity.x, playerController.climbSpeed); // Задаем скорость перемещения игрока по вертикали вверх
		}

        private void MoveDownLadder()
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>(); // Получаем компонент Rigidbody2D игрока
            PlayerController playerController = player.GetComponent<PlayerController>(); // Получаем компонент PlayerController игрока
			
			rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x, -playerController.climbSpeed); // Задаем скорость перемещения игрока по вертикали вниз
        }

        private void DetachFromLadder()
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>(); // Получаем компонент Rigidbody2D игрока
            PlayerController playerController = player.GetComponent<PlayerController>(); // Получаем компонент PlayerController игрока
			
			rb.gravityScale = playerController.defaultGravityScale;
            rb.velocity = Vector2.zero; // Сбрасываем скорость перемещения игрока
            rb.gravityScale = playerController.defaultGravityScale; // Восстанавливаем коэффициент гравитации игрока до значения по умолчанию
            playerController.canMove = true; // Устанавливаем флаг, что игрок может двигаться
        }
    }
}