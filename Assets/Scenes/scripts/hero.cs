using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Определение класса игрового героя.
public class hero : MonoBehaviour
{
    [SerializeField] private float speed = 3f;    // Скорость перемещения героя.
    [SerializeField] private int lives = 5;       // Количество жизней героя.
    [SerializeField] private float jumpForce = 15f; // Сила прыжка героя.
    [SerializeField] private LayerMask groundLayer; // Слой, представляющий землю.
    [SerializeField] private Transform groundCheck; // Точка для проверки заземления.

    private const float GroundCheckRadius = 0.2f; // Радиус проверки заземления.

    public int currentHealth; // Текущее здоровье героя.
    public bool OnLadder { get; set; } // Находится ли персонаж на лестнице.
    private bool isGrounded; // Находится ли персонаж на земле.

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    public static hero Instance { get; private set; }

    private enum States
    {
        idle,
        run,
        jump
    }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        Instance = this;
        currentHealth = lives; // Инициализация текущего здоровья героя.
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        if (isGrounded && State != States.jump) State = States.idle;

        if (Input.GetButton("Horizontal")) Run();
        if (isGrounded && Input.GetButtonDown("Jump")) Jump();

        if (OnLadder)
        {
            State = States.jump; // Временно используем анимацию прыжка.
            Crawl();
        }
    }

    private void Run()
    {
        Vector3 dir = transform.right * Input.GetAxis("Horizontal");

        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

        if (dir.x > 0.0f) sprite.flipX = false;
        else if (dir.x < 0.0f) sprite.flipX = true;

        if (isGrounded) State = States.run;
    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, GroundCheckRadius, groundLayer);
        if (!isGrounded && State != States.jump && !OnLadder) State = States.jump;
    }

    private void Crawl()
    {
        float inputVertical = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(rb.velocity.x, inputVertical * speed);

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnLadder = false;
            rb.gravityScale = 1; // Восстановление гравитации.
            State = isGrounded ? States.idle : States.jump; // Возвращаем состояние ходьбы/стояния.
        }
        else
        {
            rb.gravityScale = 0; // Отключаем гравитацию для лазания.
        }
    }

    public void GetDamage(int damage)
    {
        lives -= damage;
        Debug.Log($"У героя осталось {lives} жизней");

        if (lives <= 0) Die();
    }

    private void Die()
    {
        Destroy(gameObject); // Уничтожение персонажа.
    }
}
