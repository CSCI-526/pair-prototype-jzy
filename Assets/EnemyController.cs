using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveDuration = 1f;  // Duration for each movement
    public float jumpForce = 5f;     // Jump force for the enemy
    private bool isGrounded = true;  // Check if enemy is on the ground
    private Rigidbody2D rb;

    private Queue<string> movementQueue = new Queue<string>();  // Stores planned movements
    private bool isMoving = false;   // Is the enemy currently moving?

    private float horizontalForce = -2f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //PlanEnemyMovements(1);
    }

    // Start the enemy movement based on the number of turns
    public void StartEnemyMovement(int playCount)
    {
        if (!isMoving)
        {
            // Plan movements based on the play count (1st or 2nd time)
            PlanEnemyMovements(playCount);

            if (movementQueue.Count > 0)
            {
                isMoving = true;
                StartCoroutine(ExecuteEnemyMovements());
            }
        }
    }

    // Plan different movements for each play turn
    public void PlanEnemyMovements(int playCount)
    {
        movementQueue.Clear();

        if (playCount == 1)  // First play turn
        {
            movementQueue.Enqueue("left");
            movementQueue.Enqueue("left");
            movementQueue.Enqueue("wait");
        }
        else if (playCount == 2)  // Second play turn
        {
            movementQueue.Enqueue("jump");
            movementQueue.Enqueue("left");
            movementQueue.Enqueue("left");
        }
    }


    // Coroutine to execute the planned movement from the queue
    IEnumerator ExecuteEnemyMovements()
    {
        while (movementQueue.Count > 0)
        {
            string nextMove = movementQueue.Dequeue();  // Get the next planned movement

            switch (nextMove)
            {
                case "right":
                    yield return StartCoroutine(MoveEnemy(Vector2.right));
                    break;
                case "left":
                    yield return StartCoroutine(MoveEnemy(Vector2.left));
                    break;
                case "jump":
                    yield return StartCoroutine(WaitForGroundAndExecute(() => Jump()));
                    break;
                default:
                    yield return null;
                    break;
            }
        }

        // When all movements are executed, reset isMoving
        isMoving = false;
    }

    // Method to move the enemy in the specified direction
    IEnumerator MoveEnemy(Vector2 direction)
    {
        float moveDistance = Camera.main.orthographicSize * 2 * Camera.main.aspect / 10f;
        Vector2 targetPosition = rb.position + new Vector2(direction.x * moveDistance, 0);

        float elapsedTime = 0f;
        Vector2 startingPosition = rb.position;
        float moveSpeed = 2f;

        while (elapsedTime < moveDuration)
        {
            Vector2 newPosition = Vector2.Lerp(startingPosition, targetPosition, elapsedTime / moveDuration);
            rb.MovePosition(newPosition);
            elapsedTime += Time.deltaTime * moveSpeed;
            yield return null;
        }

        rb.MovePosition(targetPosition);
    }

    // Method to make the enemy jump
    void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector2(horizontalForce, jumpForce), ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    // Optional: Raycast for ground check
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        isGrounded = hit.collider != null;
    }

    IEnumerator WaitForGroundAndExecute(System.Action action)
    {
        while (!isGrounded)
        {
            yield return new WaitForSeconds(0.5f);
        }

        action();  // Execute the action (jump and move)
        yield return new WaitForSeconds(1.5f);  // Wait for jump completion
    }
}
