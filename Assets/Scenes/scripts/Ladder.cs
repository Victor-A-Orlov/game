using UnityEngine;

namespace SupanthaPaul
{
    public class Ladder : MonoBehaviour
    {
        bool canMoveHorizontally = true; // горизонтальное перемещение
        private bool isPlayerNearLadder = false; // Флаг, указывающий на то, что игрок подошел к лестнице
        private bool isPlayerOnLadder = false; // Флаг, указывающий на то, что игрок зацепился за лестницу
        private GameObject player = null; // Ссылка на игрока

        private void Update()
        {
            
            if (isPlayerOnLadder) // Если игрок находится на лестнице
            {
                float verticalInput = Input.GetAxis("Vertical"); // Получаем вертикальное значение ввода от игрока
                float horizontalInput = 0f; // Отключение горизонтального перемещения

                Debug.Log("Вертикальное значение игрока" + player); 
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>(); 
                rb.velocity = new Vector2(horizontalInput, rb.velocity.y); // Занулить первую компоненту вектора скорости
                rb.Sleep();
                
                
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
                    Debug.Log("Кнопка Е нажата на лестнице: " + Input.GetKeyDown(KeyCode.E));
                    DetachFromLadder(); // Вызываем метод для отсоединения игрока от лестницы
                }
            }
            
            if (isPlayerNearLadder && !isPlayerOnLadder)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    isPlayerOnLadder = true;
                    Debug.Log("Кнопка Е нажата у лестницы, isPlayerOnLadder=" + isPlayerOnLadder);
                }
            }

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) // Если столкновение с объектом, помеченным тегом "Player" (мы у лестницы)
            {
                Debug.Log("Подошли");
                isPlayerNearLadder = true;
                player = collision.gameObject;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                    Debug.Log("Зацепились: " + Input.GetKeyDown(KeyCode.E));
                    rb.Sleep();
                    isPlayerOnLadder = true; // Устанавливаем флаг, что игрок находится на лестнице
                     // Сохраняем ссылку на игрока
                }
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
            //TODO: занулить первый компонент вектора скорости
			rb.velocity = new Vector2(rb.velocity.x, playerController.climbSpeed); // Задаем скорость перемещения игрока по вертикали вверх
            rb.velocity = new Vector2(0f, rb.velocity.y); // Зануляем горизонтальную компоненту вектора скорости
		}

        private void MoveDownLadder()
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>(); // Получаем компонент Rigidbody2D игрока
            PlayerController playerController = player.GetComponent<PlayerController>(); // Получаем компонент PlayerController игрока
			
			rb.Sleep();
            //TODO: занулить первый компонент вектора скорости
            rb.velocity = new Vector2(0f, -playerController.climbSpeed); // Задаем скорость перемещения игрока по вертикали вниз
                rb.velocity = new Vector2(0f, rb.velocity.y); // Зануляем горизонтальную компоненту вектора скорости
        }

        private void DetachFromLadder()
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>(); // Получаем компонент Rigidbody2D игрока
            PlayerController playerController = player.GetComponent<PlayerController>(); // Получаем компонент PlayerController игрока
			
			
            rb.velocity = Vector2.zero; // Сбрасываем скорость перемещения игрока
            rb.gravityScale = 1; // Восстанавливаем коэффициент гравитации игрока до значения по умолчанию
            isPlayerOnLadder = false;
            rb.WakeUp();
            // playerController.canMove = true; // Устанавливаем флаг, что игрок может двигаться
        }
    }
}