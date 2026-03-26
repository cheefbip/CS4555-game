using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // === Movement / Behavior Settings ===
    public Transform player;
    public float speed = 2f;
    public float chaseRange = 4f;
    public float patrolDistance = 2f;

    // === Ground & Wall Detection ===
    [Header("Ground / Wall Detection")]
    public LayerMask groundLayer;
    public float checkDistance = 0.4f;
    public Vector2 groundCheckOffset = new Vector2(0.35f, -0.6f);
    public Vector2 wallCheckOffset = new Vector2(0.35f, 0f);

    // === Combat Settings ===
    [Header("Stomp Settings")]
    public float stompBounceForce = 8f;     // How high player bounces after stomping
    public float stompKillHeight = 0.2f;    // How much higher player must be to count as stomp

    // === Internal State ===
    private Rigidbody2D rb;
    private Vector2 patrolCenter;
    private int patrolDirection = 1;
    private bool wasChasing = false;
    private bool isDead = false;

    void Awake()
    {
        // Cache Rigidbody and store starting position for patrol
        rb = GetComponent<Rigidbody2D>();
        patrolCenter = transform.position;
    }

    void FixedUpdate()
    {
        // Stop logic if missing references or enemy already dead
        if (player == null || rb == null || isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if player is close enough to chase
        bool isChasing = distanceToPlayer <= chaseRange;

        if (isChasing)
        {
            wasChasing = true;

            // Determine direction toward player
            int chaseDirection = player.position.x > transform.position.x ? 1 : -1;

            // Move toward player (ignore ledges while chasing)
            rb.linearVelocity = new Vector2(chaseDirection * speed, rb.linearVelocity.y);
        }
        else
        {
            // If we just stopped chasing, reset patrol center
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
        // Flip direction if about to fall or hit wall
        if (!CanMove(patrolDirection))
        {
            patrolDirection *= -1;
        }

        // Keep enemy within patrol range
        if (transform.position.x > patrolCenter.x + patrolDistance)
        {
            patrolDirection = -1;
        }
        else if (transform.position.x < patrolCenter.x - patrolDistance)
        {
            patrolDirection = 1;
        }

        // Move if path is clear
        if (CanMove(patrolDirection))
        {
            rb.linearVelocity = new Vector2(patrolDirection * speed, rb.linearVelocity.y);
        }
        else
        {
            // Stop if blocked
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    bool CanMove(int direction)
    {
        // Check ground ahead (to prevent falling)
        Vector2 groundOrigin = (Vector2)transform.position + new Vector2(groundCheckOffset.x * direction, groundCheckOffset.y);

        // Check wall ahead (to prevent walking into walls)
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
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();

            // === DASH KILL ===
            // If player is dashing, enemy dies instantly
            if (playerMovement != null && playerMovement.IsDashing)
            {
                Die();

                // Small upward boost after dash hit
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 2f);
                }

                return;
            }

            // === STOMP KILL ===
            // Check if player is above enemy and falling downward
            bool playerAbove = collision.transform.position.y > transform.position.y + stompKillHeight;
            bool playerFalling = playerRb != null && playerRb.linearVelocity.y < 0f;

            if (playerAbove && playerFalling)
            {
                Die();

                // Bounce player upward after stomp
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompBounceForce);
                }
            }
            else
            {
                // Otherwise player dies
                KillPlayer();
            }
        }
    }

    void KillPlayer()
    {
        // Simple respawn (can replace with proper system later)
        player.position = new Vector3(0, 0, 0);
        Debug.Log("Player died");
    }

    void Die()
    {
        isDead = true;

        // Stop movement before destroying
        rb.linearVelocity = Vector2.zero;

        Debug.Log("Enemy defeated");

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        // Visualize ground and wall checks in editor
        int dir = patrolDirection == 0 ? 1 : patrolDirection;

        Vector2 groundOrigin = (Vector2)transform.position + new Vector2(groundCheckOffset.x * dir, groundCheckOffset.y);
        Vector2 wallOrigin = (Vector2)transform.position + new Vector2(wallCheckOffset.x * dir, wallCheckOffset.y);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundOrigin, groundOrigin + Vector2.down * checkDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(wallOrigin, wallOrigin + Vector2.right * dir * checkDistance);
    }
}