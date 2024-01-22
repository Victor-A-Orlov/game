using UnityEngine;


namespace SupanthaPaul
{
  public class PlayerController : MonoBehaviour
  {
    // Сериализованные переменные, доступные для редактирования через инспектор Unity
    [SerializeField] private float speed; // Скорость движения игрока
    [Header("Jumping")]
    [SerializeField] private float jumpForce; // Сила прыжка
    [SerializeField] private float fallMultiplier; // Коэффициент ускорения падения
    [SerializeField] private Transform groundCheck; // Точка, откуда выпускается луч для проверки земли
    [SerializeField] private float groundCheckRadius; // Радиус луча для проверки земли
    [SerializeField] private LayerMask whatIsGround; // Слои, считающиеся землей
    [SerializeField] private int extraJumpCount = 1; // Количество дополнительных прыжков
    [SerializeField] private GameObject jumpEffect; // Префаб эффекта прыжка
    [Header("Dashing")]
    [SerializeField] private float dashSpeed = 30f; // Скорость рывка
    [Tooltip("Amount of time (in seconds) the player will be in the dashing speed")]
    [SerializeField] private float startDashTime = 0.1f; // Время, в течение которого игрок будет находиться со скоростью рывка
    [Tooltip("Time (in seconds) between dashes")]
    [SerializeField] private float dashCooldown = 0.2f; // Время между рывками
    [SerializeField] private GameObject dashEffect; // Префаб эффекта рывка

    // Переменные для контроля состояния игрока
    [HideInInspector] public bool isGrounded; // Находится ли игрок на земле
    [HideInInspector] public float moveInput; // Ввод от игрока для горизонтального перемещения
    [HideInInspector] public bool canMove = true; // Может ли игрок перемещаться
    [HideInInspector] public bool isDashing = false; // Выполняется ли рывок
    [HideInInspector] public bool actuallyWallGrabbing = false; // Определяет, действительно ли игрок удерживается на стене
    [HideInInspector] public bool isCurrentlyPlayable = false; // Определяет, является ли этот экземпляр в данный момент играбельным

    [Header("Wall grab & jump")]
    [Tooltip("Right offset of the wall detection sphere")]
    public Vector2 grabRightOffset = new Vector2(0.16f, 0f); // Смещение для определения стены справа от игрока
    public Vector2 grabLeftOffset = new Vector2(-0.16f, 0f); // Смещение для определения стены слева от игрока
    public float grabCheckRadius = 0.24f; // Радиус сферы для определения стены
    public float slideSpeed = 2.5f; // Скорость скольжения по стене
    public Vector2 wallJumpForce = new Vector2(10.5f, 18f); // Сила прыжка от стены
    public Vector2 wallClimbForce = new Vector2(4f, 14f); // Сила вертикального перемещения по стене

    private Rigidbody2D m_rb; // Компонент Rigidbody2D игрока
    private ParticleSystem m_dustParticle; // Компонент ParticleSystem для пыли
    private bool m_facingRight = true; // Определяет, смотрит ли игрок вправо
    private readonly float m_groundedRememberTime = 0.25f; // Время, в течение которого игрок считается на земле после падения
    private float m_groundedRemember = 0f; // Таймер для определения, находится ли игрок на земле
    private int m_extraJumps; // Количество доступных дополнительных прыжков
    private float m_extraJumpForce; // Сила дополнительного прыжка
    private float m_dashTime; // Таймер рывка
    private bool m_hasDashedInAir = false; // Определяет, выполнил ли игрок рывок в воздухе
    private bool m_onWall = false; // Находится ли игрок на стене
    private bool m_onRightWall = false; // Находится ли игрок на правой стене
    private bool m_onLeftWall = false; // Находится ли игрок на левой стене
    private bool m_wallGrabbing = false; // Определяет, удерживается ли игрок на стене
    private readonly float m_wallStickTime = 0.25f; // Время удерживания игрока на стене
    private float m_wallStick = 0f; // Таймер удерживания игрока на стене
    private bool m_wallJumping = false; // Определяет, выполняет ли игрок прыжок от стены
    private float m_dashCooldown; // Таймер между рывками
    public float climbSpeed; // Определяет скорость движения по лестнице
    public float defaultGravityScale;


    // 0 -> none, 1 -> right, -1 -> left
    private int m_onWallSide = 0; // Определяет сторону стены, на которой находится игрок (0 - не на стене, 1 - правая стена, -1 - левая стена)
    private int m_playerSide = 1; // Определяет сторону, куда смотрит игрок (1 - вправо, -1 - влево)


	void Start()
	{
		// create pools for particles
		PoolManager.instance.CreatePool(dashEffect, 2);
		PoolManager.instance.CreatePool(jumpEffect, 2);

		// if it's the player, make this instance currently playable
		if (transform.CompareTag("Player"))
			isCurrentlyPlayable = true;

		m_extraJumps = extraJumpCount;
		m_dashTime = startDashTime;
		m_dashCooldown = dashCooldown;
		m_extraJumpForce = jumpForce * 0.7f;

		m_rb = GetComponent<Rigidbody2D>();
		m_dustParticle = GetComponentInChildren<ParticleSystem>();
	}

	private void FixedUpdate()
	{
		// check if grounded
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
		var position = transform.position;
		// check if on wall
		m_onWall = Physics2D.OverlapCircle((Vector2)position + grabRightOffset, grabCheckRadius, whatIsGround)
		          || Physics2D.OverlapCircle((Vector2)position + grabLeftOffset, grabCheckRadius, whatIsGround);
		m_onRightWall = Physics2D.OverlapCircle((Vector2)position + grabRightOffset, grabCheckRadius, whatIsGround);
		m_onLeftWall = Physics2D.OverlapCircle((Vector2)position + grabLeftOffset, grabCheckRadius, whatIsGround);

		// calculate player and wall sides as integers
		CalculateSides();

		if((m_wallGrabbing || isGrounded) && m_wallJumping)
		{
			m_wallJumping = false;
		}
		// if this instance is currently playable
		if (isCurrentlyPlayable)
		{
			// horizontal movement
			if(m_wallJumping)
			{
				m_rb.velocity = Vector2.Lerp(m_rb.velocity, (new Vector2(moveInput * speed, m_rb.velocity.y)), 1.5f * Time.fixedDeltaTime);
			}
			else
			{
				if(canMove && !m_wallGrabbing)
					m_rb.velocity = new Vector2(moveInput * speed, m_rb.velocity.y);
				else if(!canMove)
					m_rb.velocity = new Vector2(0f, m_rb.velocity.y);
			}
			// better jump physics
			if (m_rb.velocity.y < 0f)
			{
				m_rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
			}

			// Flipping
			if (!m_facingRight && moveInput > 0f)
				Flip();
			else if (m_facingRight && moveInput < 0f)
				Flip();

			// Dashing logic
			if (isDashing)
			{
				if (m_dashTime <= 0f)
				{
					isDashing = false;
					m_dashCooldown = dashCooldown;
					m_dashTime = startDashTime;
					m_rb.velocity = Vector2.zero;
				}
				else
				{
					m_dashTime -= Time.deltaTime;
					if(m_facingRight)
						m_rb.velocity = Vector2.right * dashSpeed;
					else
						m_rb.velocity = Vector2.left * dashSpeed;
				}
			}

			// wall grab
			if(m_onWall && !isGrounded && m_rb.velocity.y <= 0f && m_playerSide == m_onWallSide)
			{
				actuallyWallGrabbing = true;    // for animation
				m_wallGrabbing = true;
				m_rb.velocity = new Vector2(moveInput * speed, -slideSpeed);
				m_wallStick = m_wallStickTime;
			} else
			{
				m_wallStick -= Time.deltaTime;
				actuallyWallGrabbing = false;
				if (m_wallStick <= 0f)
					m_wallGrabbing = false;
			}
			if (m_wallGrabbing && isGrounded)
				m_wallGrabbing = false;

			// enable/disable dust particles
			float playerVelocityMag = m_rb.velocity.sqrMagnitude;
			if(m_dustParticle.isPlaying && playerVelocityMag == 0f)
			{
				m_dustParticle.Stop();
			}
			else if(!m_dustParticle.isPlaying && playerVelocityMag > 0f)
			{
				m_dustParticle.Play();
			}

		}
	}

	private void Update()
	{
		// horizontal input
		moveInput = InputSystem.HorizontalRaw();

		if (isGrounded)
		{
			m_extraJumps = extraJumpCount;
		}

		// grounded remember offset (for more responsive jump)
		m_groundedRemember -= Time.deltaTime;
		if (isGrounded)
			m_groundedRemember = m_groundedRememberTime;

		if (!isCurrentlyPlayable) return;
		// if not currently dashing and hasn't already dashed in air once
		if (!isDashing && !m_hasDashedInAir && m_dashCooldown <= 0f)
		{
			// dash input (left shift)
			if (InputSystem.Dash())
			{
				isDashing = true;
				// dash effect
				PoolManager.instance.ReuseObject(dashEffect, transform.position, Quaternion.identity);
				// if player in air while dashing
				if(!isGrounded)
				{
					m_hasDashedInAir = true;
				}
				// dash logic is in FixedUpdate
			}
		}
		m_dashCooldown -= Time.deltaTime;
		
		// if has dashed in air once but now grounded
		if (m_hasDashedInAir && isGrounded)
			m_hasDashedInAir = false;
		
		// Jumping
		if(InputSystem.Jump() && m_extraJumps > 0 && !isGrounded && !m_wallGrabbing)	// extra jumping
		{
			m_rb.velocity = new Vector2(m_rb.velocity.x, m_extraJumpForce); ;
			m_extraJumps--;
			// jumpEffect
			PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
		}
		else if(InputSystem.Jump() && (isGrounded || m_groundedRemember > 0f))	// normal single jumping
		{
			m_rb.velocity = new Vector2(m_rb.velocity.x, jumpForce);
			// jumpEffect
			PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
		}
		else if(InputSystem.Jump() && m_wallGrabbing && moveInput!=m_onWallSide )		// wall jumping off the wall
		{
			m_wallGrabbing = false;
			m_wallJumping = true;
			Debug.Log("Wall jumped");
			if (m_playerSide == m_onWallSide)
				Flip();
			m_rb.AddForce(new Vector2(-m_onWallSide * wallJumpForce.x, wallJumpForce.y), ForceMode2D.Impulse);
		}
		else if(InputSystem.Jump() && m_wallGrabbing && moveInput != 0 && (moveInput == m_onWallSide))      // wall climbing jump
		{
			m_wallGrabbing = false;
			m_wallJumping = true;
			Debug.Log("Wall climbed");
			if (m_playerSide == m_onWallSide)
				Flip();
			m_rb.AddForce(new Vector2(-m_onWallSide * wallClimbForce.x, wallClimbForce.y), ForceMode2D.Impulse);
		}

	}

	void Flip()
	{
		m_facingRight = !m_facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	void CalculateSides()
	{
		if (m_onRightWall)
			m_onWallSide = 1;
		else if (m_onLeftWall)
			m_onWallSide = -1;
		else
			m_onWallSide = 0;

		if (m_facingRight)
			m_playerSide = 1;
		else
			m_playerSide = -1;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
		Gizmos.DrawWireSphere((Vector2)transform.position + grabRightOffset, grabCheckRadius);
		Gizmos.DrawWireSphere((Vector2)transform.position + grabLeftOffset, grabCheckRadius);
	}
}
}