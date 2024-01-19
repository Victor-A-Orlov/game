using UnityEngine;

public class Patroler : MonoBehaviour
{
    public float movementSpeed = 3f;
    public float patrolDistance = 0.1f;
    public float playerDetectionRadius = 5f;
    public float attackRange = 1f;
    public int attackDamage = 10;
    public Transform patrolStartPosition;

    private Transform hero;
    private Vector2 direction = Vector2.left;
    private Vector2 startPatrolPosition;
    private bool isPatrolling = true;
    private bool isChasingPlayer = false;

    private void Start()
    {
        hero = GameObject.FindGameObjectWithTag("Player").transform;
        startPatrolPosition = patrolStartPosition.position;
        Flip();  // make sure we're looking the correct way at the beginning
    }

    private void Update()
    {
        if (isPatrolling)
        {
            Patrol();
        }

        if (isChasingPlayer)
        {
            ChasePlayer();
        }

        if (!isChasingPlayer && !isPatrolling)
        {
            GoBackToPatrol();
        }
    }

    private void Patrol()
    {
        transform.Translate(direction * movementSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPatrolPosition.x) >= patrolDistance)
        {
            direction *= -1;
            Flip();
            startPatrolPosition = transform.position;
        }

        if (IsPlayerDetected())
        {
            StartChasingPlayer();
        }
    }

    private bool IsPlayerDetected()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, playerDetectionRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    private void StartChasingPlayer()
    {
        isPatrolling = false;
        isChasingPlayer = true;
    }

    private void ChasePlayer()
    {
        if (transform.position.x > hero.position.x)
        {
            direction = Vector2.left;
        }
        else
        {
            direction = Vector2.right;
        }

        Flip();

        transform.position = Vector2.MoveTowards(transform.position, hero.position, movementSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, hero.position) <= attackRange)
        {
            Attack();
        }

        if (!IsPlayerDetected())
        {
            StopChasingPlayer();
        }
    }

    private void StopChasingPlayer()
    {
        isChasingPlayer = false;
    }

    private void GoBackToPatrol()
    {
        direction = Vector2.right;
        Flip();
        transform.position = Vector2.MoveTowards(transform.position, patrolStartPosition.position, movementSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, patrolStartPosition.position) <= 0.01f) // используем маленькую дельту вместо прямого сравнения
        {
            isPatrolling = true;
            startPatrolPosition = transform.position; // обновляем начальную позицию патрулирования
        }

        if (IsPlayerDetected()) // если игрок снова обнаружен, начинаем преследовать
        {
            StartChasingPlayer();
        }
    }


    private void Attack()
    {
        Debug.Log("Mob is attacking the player.");
    }

    private void Flip()
    {
        if (direction == Vector2.left)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (direction == Vector2.right)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
