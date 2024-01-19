using UnityEngine;
using System.Collections;

public class ShootingEnemy : MonoBehaviour
{
    public Transform player; // Ссылка на игрока
    public GameObject projectilePrefab; // Префаб снаряда
    public Transform projectileSpawnPoint; // Место спавна снаряда
    public float shootingRange = 5f; // Расстояние, на котором NPC начнет стрелять
    public float shootingCooldown = 2f; // Время между выстрелами
    private bool canShoot = true; // Флаг, разрешающий выстрелы

    private void Update()
    {
        // Рассчитываем расстояние до игрока
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Если игрок находится в пределах дальности стрельбы и NPC может стрелять, выполняем выстрел
        if (distanceToPlayer <= shootingRange && canShoot)
        {
            Shoot();
            StartCoroutine(ShootingCooldown());
        }
    }

    private void Shoot()
    {
        // Создаем снаряд
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        // Направляем снаряд к игроку
        projectile.GetComponent<Projectile>().Launch(player.position - transform.position);
    }

    private IEnumerator ShootingCooldown()
    {
        // Запретить выстрелы на заданное время
        canShoot = false;
        yield return new WaitForSeconds(shootingCooldown);
        // Разрешить выстрелы
        canShoot = true;
    }
}
