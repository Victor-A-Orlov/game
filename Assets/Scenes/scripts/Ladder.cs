using UnityEngine;

namespace SupanthaPaul
{
    public class Ladder : MonoBehaviour
    {
        bool canMoveHorizontally = true; // горизонтальное перемещение
        private bool isPlayerNearLadder = false; // Флаг, указывающий на то, что игрок подошел к лестнице
        private bool isPlayerOnLadder = false; // Флаг, указывающий на то, что игрок зацепился за лестницу
        private bool justAttachedToLadder = false;
        private GameObject player = null; // Ссылка на игрока
    
        private void Update()
        {
            if (isPlayerNearLadder && !isPlayerOnLadder)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    AttachToLadder();
                    Debug.Log("Кнопка Е нажата у лестницы, isPlayerOnLadder=" + isPlayerOnLadder);
                    justAttachedToLadder = true;
                }
            }
            
            if (isPlayerOnLadder) // Если игрок находится на лестнице
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                // playerController.canMove = false;
                float verticalInput = Input.GetAxis("Vertical"); // Получаем вертикальное значение ввода от игрока
                immobilizePlayer();

                if (verticalInput > 0f) // Если игрок двигается вверх
                {
                    MoveUpLadder(); // Вызываем метод для перемещения игрока вверх по лестнице
                    Debug.Log("Двигаемся вверх");
                }
                else if (verticalInput < 0f) // Если игрок двигается вниз
                {
                    MoveDownLadder(); // Вызываем метод для перемещения игрока вниз по лестнице
                    Debug.Log("Двигаемся вниз");
                }
                if (Input.GetKeyDown(KeyCode.E)) // Если игрок нажимает клавишу "E"
                {
                    if (!justAttachedToLadder)
                    {
                        Debug.Log("Кнопка Е нажата на лестнице: " + Input.GetKeyDown(KeyCode.E));
                        DetachFromLadder(); // Вызываем метод для отсоединения игрока от лестницы
                    }
                }
            }

            justAttachedToLadder = false;
            


        }

        private void immobilizePlayer()
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(0, rb.velocity.y); // Занулить первую компоненту вектора скорости
            rb.Sleep();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) // Если столкновение с объектом, помеченным тегом "Player" (мы у лестницы)
            {
                Debug.Log("Подошли");
                isPlayerNearLadder = true;
                player = collision.gameObject;
            }
        }

        private void AttachToLadder()
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.canMove = false;
            Debug.Log("Зацепились: " + Input.GetKeyDown(KeyCode.E));
            rb.Sleep();
            isPlayerOnLadder = true; // Устанавливаем флаг, что игрок находится на лестнице
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
            //TODO: занулить первый компонент вектора скорости
			rb.velocity = new Vector2(0, playerController.climbSpeed); // Задаем скорость перемещения игрока по вертикали вверх
		}

        private void MoveDownLadder()
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>(); // Получаем компонент Rigidbody2D игрока
            PlayerController playerController = player.GetComponent<PlayerController>(); // Получаем компонент PlayerController игрока
			
			rb.Sleep();
            //TODO: занулить первый компонент вектора скорости
            rb.velocity = new Vector2(0, -playerController.climbSpeed); // Задаем скорость перемещения игрока по вертикали вниз
        }

        private void DetachFromLadder()
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>(); // Получаем компонент Rigidbody2D игрока
            PlayerController playerController = player.GetComponent<PlayerController>(); // Получаем компонент PlayerController игрока
			
			
            rb.velocity = Vector2.zero; // Сбрасываем скорость перемещения игрока
            rb.gravityScale = 1; // Восстанавливаем коэффициент гравитации игрока до значения по умолчанию
            isPlayerOnLadder = false;
            playerController.canMove = true;
            rb.WakeUp();
            // playerController.canMove = true; // Устанавливаем флаг, что игрок может двигаться
        }
    }
}