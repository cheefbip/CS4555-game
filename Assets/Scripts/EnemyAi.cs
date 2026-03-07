using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;

    void Update()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        float moveX = Mathf.Sign(direction.x);

        transform.position += new Vector3(moveX, 0f, 0f) * speed * Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            KillPlayer();
        }
    }

    void KillPlayer()
    {
        // Placeholder: GameController will handle player death later
    }
}
