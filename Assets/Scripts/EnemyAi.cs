using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float chaseRange = 4f;
    public float patrolDistance = 2f;

    [Header("Ground / Wall Detection")]
    public LayerMask groundLayer;
    public float checkDistance = 0.4f;
    public Vector2 groundCheckOffset = new Vector2(0.35f, -0.6f);
    public Vector2 wallCheckOffset = new Vector2(0.35f, 0f);

    [Header("Stomp Settings")]
    public float stompBounceForce = 8f;
    public float stompKillHeight = 0.2f;

    private Rigidbody2D rb;
    private Vector2 patrolCenter;
    private int patrolDirection = 1;
    private bool wasChasing = false;
    private bool isDead = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        patrolCenter = transform.position;
    }

    void FixedUpdate()
    {
        if (player == null || rb == null || isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool isChasing = distanceToPlayer <= chaseRange;

        if (isChasing)
        {
            wasChasing = true;

            int chaseDirection = player.position.x > transform.position.x ? 1 : -1;

            // During chase, ignore ledges so enemy can drop down after player
            rb.linearVelocity = new Vector2(chaseDirection * speed, rb.linearVelocity.y);
        }
        else
        {
            if (wasChasing)
            {
                patrolCenter = transform.position;
                wasChasing = false;
            }

            Patrol();
        }
    }

    void Patrol()
    {
        if (!CanMove(patrolDirection))
        {
            patrolDirection *= -1;
        }

        if (transform.position.x > patrolCenter.x + patrolDistance)
        {
            patrolDirection = -1;
        }
        else if (transform.position.x < patrolCenter.x - patrolDistance)
        {
            patrolDirection = 1;
        }

        if (CanMove(patrolDirection))
        {
            rb.linearVelocity = new Vector2(patrolDirection * speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    bool CanMove(int direction)
    {
        Vector2 groundOrigin = (Vector2)transform.position + new Vector2(groundCheckOffset.x * direction, groundCheckOffset.y);
        Vector2 wallOrigin = (Vector2)transform.position + new Vector2(wallCheckOffset.x * direction, wallCheckOffset.y);

        bool groundAhead = Physics2D.Raycast(groundOrigin, Vector2.down, checkDistance, groundLayer);
        bool wallAhead = Physics2D.Raycast(wallOrigin, Vector2.right * direction, checkDistance, groundLayer);

        return groundAhead && !wallAhead;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            bool playerAbove = collision.transform.position.y > transform.position.y + stompKillHeight;
            bool playerFalling = playerRb != null && playerRb.linearVelocity.y < 0f;

            if (playerAbove && playerFalling)
            {
                Die();

                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompBounceForce);
                }
            }
            else
            {
                KillPlayer();
            }
        }
    }

    void KillPlayer()
    {
        player.position = new Vector3(0, 0, 0);
        Debug.Log("Death");
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Enemy defeated");

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        int dir = patrolDirection == 0 ? 1 : patrolDirection;

        Vector2 groundOrigin = (Vector2)transform.position + new Vector2(groundCheckOffset.x * dir, groundCheckOffset.y);
        Vector2 wallOrigin = (Vector2)transform.position + new Vector2(wallCheckOffset.x * dir, wallCheckOffset.y);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundOrigin, groundOrigin + Vector2.down * checkDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(wallOrigin, wallOrigin + Vector2.right * dir * checkDistance);
    }
}