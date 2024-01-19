using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : entity
{
    private float speed = 0.8f;
    private Vector3 direction;
    private SpriteRenderer sprite;

private void Awake() // Метод, который вызывается при создании объекта
{
    sprite = GetComponentInChildren<SpriteRenderer>(); // Присваивает переменной sprite компонент SpriteRenderer, который находится в дочернем объекте
}

private void Start() // Метод, который вызывается при запуске сцены
{
    direction = transform.right; // Присваивает переменной direction значение вектора, указывающего вправо относительно объекта
}

private void Move() // Метод, который отвечает за перемещение врага
{
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.0f + transform.right * direction.x * 0.7f, -0.1f);  // Создает массив colliders и заполняет его всеми коллайдерами, которые пересекаются с кругом заданного радиуса и центра. Центр круга находится немного выше и впереди относительно объекта.

    if (colliders.Length > 0) direction *= -1f; // Если массив colliders не пустой, то меняет направление врага на противоположное
    transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed*Time.deltaTime); // Перемещает объект на заданное расстояние в заданном направлении с учетом скорости и времени кадра
    sprite.flipX = direction.x > 0.1f; // Отражает спрайт по горизонтали в зависимости от направления

}

private void Update() // Метод, который вызывается каждый кадр
{
    Move(); // Вызывает метод Move
}

private void OnCollisionEnter2D(Collision2D collision) // Метод, который вызывается при столкновении с другим коллайдером
{
    if (collision.gameObject == hero.Instance.gameObject) // Если объект столкновения является героем
    {
        hero.Instance.GetDamage(2); // Вызывает метод GetDamage у героя
    }
}
}