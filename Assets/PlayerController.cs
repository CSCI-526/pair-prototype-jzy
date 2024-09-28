using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 5f;       // Force applied when jumping
    public float moveDuration = 1f;    // Duration for waiting between moves
    private bool isGrounded = true;    // Check if player is on the ground
    private bool isPlaying = false;    // To prevent multiple executions
    private Queue<string> movementQueue = new Queue<string>(); // Store planned movements

    public Button playButton;  // Reference to the Play button in the UI
    void Start()
    {
        // Set up the Play button's onClick event to trigger the movement plan
        playButton.onClick.AddListener(StartMovement);
    }

    // Update is called once per frame
    void Update()
    {
        // isGrounded check
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        isGrounded = hit.collider != null;

        // Allow user to plan movements using keys
        PlanMovementInput();
    }

    // Method to check player input and queue the movements
    void PlanMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.D))  // Plan Right move
        {
            movementQueue.Enqueue("right");
            Debug.Log("Move Right planned");
        }

        if (Input.GetKeyDown(KeyCode.A))  // Plan Left move
        {
            movementQueue.Enqueue("left");
            Debug.Log("Move Left planned");
        }

        if (Input.GetKeyDown(KeyCode.W))  // Plan Jump
        {
            movementQueue.Enqueue("jump");
            Debug.Log("Jump planned");
        }

        if (Input.GetKeyDown(KeyCode.S))  // Plan Wait
        {
            movementQueue.Enqueue("wait");
            Debug.Log("Wait planned");
        }
    }

    // This method starts executing the planned movements when Play button is pressed
    public void StartMovement()
    {
        if (!isPlaying && movementQueue.Count > 0)  // Prevent multiple executions and ensure there are planned moves
        {
            isPlaying = true;
            StartCoroutine(ExecutePlannedMovement());
        }
    }

    // Coroutine to execute the planned movement from the queue
    IEnumerator ExecutePlannedMovement()
    {
        while (movementQueue.Count > 0)
        {
            string nextMove = movementQueue.Dequeue();  // Get the next planned movement

            switch (nextMove)
            {
                case "right":
                    yield return StartCoroutine(MovePlayer(Vector2.right));
                    break;
                case "left":
                    yield return StartCoroutine(MovePlayer(Vector2.left));
                    break;
                case "jump":
                    if (isGrounded)
                    {
                        Jump();
                        yield return new WaitForSeconds(1f);  // Wait for jump completion
                    }
                    break;
                case "wait":
                    yield return new WaitForSeconds(moveDuration);  // Simply wait
                    break;
            }
        }

        // When all movements are executed, reset isPlaying to allow future executions
        isPlaying = false;
    }

    // Method to move the player left or right
    IEnumerator MovePlayer(Vector2 direction)
    {
        // Calculate movement based on the screen size (camera view size)
        float moveDistance = Camera.main.orthographicSize * 2 * Camera.main.aspect / 10f; // Adjusting by 10 for 1/10th screen unit move
        Vector3 targetPosition = transform.position + new Vector3(direction.x * moveDistance, 0, 0);

        // Move over time to simulate smooth movement
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;
        float moveSpeed = 2f;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime * moveSpeed;
            yield return null;
        }

        // Ensure exact final position after movement
        transform.position = targetPosition;
    }

    // Method to make the player jump
    void Jump()
    {
        if (isGrounded)
        {
            // Apply upward force to the player to make them jump
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
        }
    }
}
