using UnityEngine;

public class Projectile : MonoBehaviour
{
    // скорость снаряда
    public float speed = 10f; 

    // Урон, наносимый снарядом
    public int damage = 1;

    // Направление движения снаряда
    private Vector2 direction;

    // Стартовая функция для самоуничтожения снаряда через определенное время
    void Start()
    {
        // Уничтожает снаряд спустя 3 секунды
        Invoke("DestroyProjectile", 3f);
    }
    
    void Update()
    {
        // Перемещает снаряд в указанном направлении со скоростью speed
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }

    // Функция для установки направления снаряда
    public void Launch(Vector2 direction)
    {
        this.direction = direction;
    }

    // Функция, реагирующая на столкновение с объектами, имеющими определенные теги
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Проверяет, не врезался ли снаряд в персонажа по тегу "Player"
        if (hitInfo.gameObject.CompareTag("Player"))
        {
            // Получаем компонент "герой" объекта
            hero hero = hitInfo.gameObject.GetComponent<hero>();
            
            // Если компонент найден, наносим урон герою
            if (hero != null)
            {
                hero.GetDamage(damage);
				Destroy(gameObject);
            }
        }
        else if (hitInfo.gameObject.CompareTag("Obstacle"))
        {
            // Если снаряд врезается в преграду, уничтожаем его
            Destroy(gameObject);
        }
    }
    
    // Функция уничтожения снаряда
    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}