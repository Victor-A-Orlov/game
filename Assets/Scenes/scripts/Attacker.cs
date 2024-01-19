using UnityEngine;

public class Attacker : MonoBehaviour
{
    public Transform waitPoint;
    public Transform attackPoint;
    public float waitTime = 2f;
    public float attackDelay = 1f;
    public float moveSpeed = 2f;

    private bool isWaiting = true;
    private bool isAttacking = false;

    private void Start()
    {
        // Перемещаем моба к точке ожидания
        transform.position = waitPoint.position;
        Invoke("SwitchState", waitTime);
    }

    private void Update()
    {
        if (isWaiting)
        {
            // Моб ожидает
            // Можно добавить анимацию ожидания
        }
        else if (isAttacking)
        {
            // Моб атакует
            transform.position = Vector2.MoveTowards(transform.position, attackPoint.position, moveSpeed * Time.deltaTime);
        }
    }

    private void SwitchState()
    {
        if (isWaiting)
        {
            // Моб начинает атаковать
            isWaiting = false;
            isAttacking = true;
            InvokeRepeating("Attack", 0f, attackDelay);
        }
    }

    private void Attack()
    {
        // Здесь можно добавить код атаки моба
        Debug.Log("Моб атакует!");

        // Прекращаем атаковать после нескольких ударов
        // Например, после трех ударов
        // Если у вас есть анимации атаки, может потребоваться управление анимацией здесь
    }
}