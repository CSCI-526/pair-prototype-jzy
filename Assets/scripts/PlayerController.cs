using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameObject[] commandBoxes;  // Drag the command boxes in the Inspector
    public float jumpForce = 5f;       // Force applied when jumping
    public float horizontalForce = 2f; // Force applied when moving horizontally
    public float moveDuration = 1f;    // Duration for waiting between moves
    private bool isGrounded = true;    // Check if player is on the ground
    private bool isPlaying = false;    // Prevent multiple executions
    private Queue<string> movementQueue = new Queue<string>(); // Store planned movements
    public Button playButton;  // Reference to the Play button in the UI
    private Rigidbody2D rb;    // Reference to the player's Rigidbody2D component

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playButton.onClick.AddListener(StartMovement);  // Set up Play button event
    }

    void Update()
    {
        // Check if the player is grounded using raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.3f);
        isGrounded = hit.collider != null;
    }

    // Queue the movements based on arrows placed in the boxes
    public void QueueMovements()
    {
        ClearMovementQueue();  // Clear previous movements

        foreach (GameObject box in commandBoxes)
        {
            if (box.transform.childCount > 0)  // Check if a box has an arrow dropped into it
            {
                string arrowName = box.transform.GetChild(0).name.ToLower();  // Get the arrow name
                Debug.Log("Enqueuing arrow: " + arrowName);  // Log the arrow name
                AddToMovementQueue(arrowName);  // Add the movement to the queue
            }
        }
    }

    // Clear the movement queue
    public void ClearMovementQueue()
    {
        movementQueue.Clear();
    }

    // Add to the movement queue
    public void AddToMovementQueue(string moveCommand)
    {
        movementQueue.Enqueue(moveCommand);
    }

    // Start executing the movements when the Play button is pressed
    public void StartMovement()
    {
        if (!isPlaying && movementQueue.Count > 0)  // Prevent multiple executions
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
                case "upright":
                    yield return StartCoroutine(WaitForGroundAndExecute(() => JumpAndMove(Vector2.right)));
                    break;
                case "upleft":
                    yield return StartCoroutine(WaitForGroundAndExecute(() => JumpAndMove(Vector2.left)));
                    break;
                case "right":
                    yield return StartCoroutine(MovePlayer(Vector2.right));
                    break;
                case "left":
                    yield return StartCoroutine(MovePlayer(Vector2.left));
                    break;
                case "up":
                    // Ensure the player is grounded before jumping again
                    yield return StartCoroutine(WaitForGroundAndExecute(() => Jump()));
                    break;
                case "wait":
                    yield return new WaitForSeconds(moveDuration);  // Simply wait
                    break;
            }
        }

        isPlaying = false;  // Reset isPlaying after all movements are executed
    }

    // Method to move the player left or right
    IEnumerator MovePlayer(Vector2 direction)
    {
        float moveDistance = Camera.main.orthographicSize * 2 * Camera.main.aspect / 10f;  // Calculate movement distance
        Vector2 targetPosition = rb.position + direction * moveDistance;

        float elapsedTime = 0f;
        Vector2 startingPosition = rb.position;

        while (elapsedTime < moveDuration)
        {
            Vector2 newPosition = Vector2.Lerp(startingPosition, targetPosition, elapsedTime / moveDuration);
            rb.MovePosition(newPosition);
            elapsedTime += Time.deltaTime * horizontalForce;
            yield return null;
        }

        rb.MovePosition(targetPosition);  // Ensure exact final position after movement
    }

    // Method to apply simultaneous jump and horizontal movement
    void JumpAndMove(Vector2 direction)
    {
        if (isGrounded)
        {
            Debug.Log("Jumping and moving in direction: " + direction);
            rb.AddForce(new Vector2(direction.x * horizontalForce, jumpForce), ForceMode2D.Impulse);
            isGrounded = false;  // Prevent further jumping in mid-air
        }
    }

    // Method to make the player jump
    void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    // Wait until the player is grounded, then execute the jump and move
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

