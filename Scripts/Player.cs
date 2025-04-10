using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    // ------------------- SETTINGS -------------------
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float climbSpeed = 3f;

    [Header("Attack Settings")]
    [SerializeField] private float radius = 1f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private LayerMask layerAttack;
    [SerializeField] private GameObject attackArea;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;

    // ------------------- COMPONENTS -------------------
    private Rigidbody2D _rb;
    private Animator _anim;

    // ------------------- STATES -------------------
    private bool isGrounded;
    private bool isClimbing;
    private bool isAttacking = false;
    private string currentState = "Idle";
    private float gravityStart;

    private Health _health;

    // ------------------- UNITY METHODS -------------------
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        gravityStart = _rb.gravityScale;
        _health = GetComponent<Health>();

    }

    private void Update()
    {
        isGrounded = CheckGround();

        HandleMovement();
        HandleJump();
        HandleClimb();
        HandleAttack();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = true;
            _rb.gravityScale = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = false;
            _rb.gravityScale = gravityStart;
        }
    }

    private void OnDrawGizmos()
    {
        if (attackArea != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackArea.transform.position, radius);
        }
    }

    // ------------------- MOVEMENT -------------------
    private void HandleMovement()
    {
        float xAxis = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(xAxis) > 0.1f)
        {
            ChangeAnimation("Walk");
            _rb.linearVelocity = new Vector2(xAxis * moveSpeed, _rb.linearVelocity.y);
            transform.rotation = Quaternion.Euler(0f, xAxis > 0f ? 0f : 180f, 0f);
        }
        else
        {
            ChangeAnimation("Idle");
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            _anim.SetTrigger("Jump");
        }
    }

    private void HandleClimb()
    {
        if (isClimbing)
        {
            float yAxis = Input.GetAxisRaw("Vertical");
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, yAxis * climbSpeed);
            //ChangeAnimation(yAxis != 0 ? "Climb" : "ClimbIdle");
        }
    }

    // ------------------- ATTACK -------------------
    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
        {
            StartCoroutine(DoAttack());
        }
    }

    private IEnumerator DoAttack()
    {
        isAttacking = true;

        _anim.SetTrigger("Atk-1");
        attackArea.SetActive(true);

        // Gây sát thương cho enemy trong vùng
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackArea.transform.position,
            radius,
            layerAttack
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>()?.TakeDamage(damage);

        }

        yield return new WaitForSeconds(0.3f);
        attackArea.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        _health?.TakeDamage(damage);
        // Gọi animation hoặc hiệu ứng trúng đòn
    }

    // ------------------- HELPERS -------------------
    private bool CheckGround()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
        return hit.collider != null;
    }

    private void ChangeAnimation(string newState)
    {
        if (currentState == newState) return;

        _anim.ResetTrigger(currentState);
        currentState = newState;
        _anim.SetTrigger(currentState);
    }
}
