using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Import UI for standard Text
using TMPro;  // Uncomment if using TextMeshPro


public class PlayerController : MonoBehaviour
{
    public float jumpForce = 7f;
    public float moveDuration = 1f;
    private bool isGrounded = true;
    private bool isPlaying = false;
    private Queue<string> movementQueue = new Queue<string>();
    private List<string> movementList = new List<string>();
    public EnemyController enemy;  // Reference to the enemy's controller script
    private int playCount = 0;     // Track how many times play has been pressed
    private float horizontalForce = 2f;

    public Button playButton;
    private Rigidbody2D rb;
    private int maxInputsPerTurn = 3;

    // Reference to the UI Text to display the movements
    //public Text movementText;  // Standard UI Text
    public TextMeshProUGUI movementText; // Uncomment if using TextMeshPro

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playButton.onClick.AddListener(StartMovement);

        // Initialize the text display
        UpdateMovementText();
    }

    void Update()
    {
        CheckIfGrounded();

        PlanMovementInput();

        // Handle input deletion
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            RemoveLastInput();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearAllInputs();
        }
    }

    // Method to check if the player is grounded
    private void CheckIfGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.3f);
        isGrounded = hit.collider != null;
    }

    // Method to plan player movement input, only allows 3 inputs
    void PlanMovementInput()
    {
        // Only allow more inputs if we have less than the max allowed inputs
        if (movementList.Count < maxInputsPerTurn)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                movementQueue.Enqueue("right");
                movementList.Add("right");
                UpdateMovementText();  // Update the UI Text
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                movementQueue.Enqueue("left");
                movementList.Add("left");
                UpdateMovementText();  // Update the UI Text
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                movementQueue.Enqueue("jump");
                movementList.Add("jump");
                UpdateMovementText();  // Update the UI Text
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                movementQueue.Enqueue("wait");
                movementList.Add("wait");
                UpdateMovementText();  // Update the UI Text
            }
        }
    }

    // Method to remove the last planned input
    void RemoveLastInput()
    {
        if (movementList.Count > 0)
        {
            movementList.RemoveAt(movementList.Count - 1);
            RebuildQueue();
            UpdateMovementText();  // Update the UI Text after removal
        }
    }

    // Method to clear all planned inputs
    void ClearAllInputs()
    {
        movementList.Clear();
        RebuildQueue();
        UpdateMovementText();  // Update the UI Text after clearing
    }

    // Rebuild the queue based on the updated list
    void RebuildQueue()
    {
        movementQueue.Clear();
        foreach (string move in movementList)
        {
            movementQueue.Enqueue(move);
        }
    }

    // Method to update the movement text on the UI
    void UpdateMovementText()
    {
        movementText.text = "Planned Movements: ";

        if (movementList.Count == 0)
        {
            movementText.text += "None";  // Display None if no movements are planned
        }
        else
        {
            foreach (string move in movementList)
            {
                movementText.text += move + " ";  // Concatenate the planned movements
            }
        }
    }

    public void StartMovement()
    {
        if (!isPlaying && movementQueue.Count > 0)
        {
            isPlaying = true;
            StartCoroutine(ExecutePlannedMovement());

            // Increase the play count every time the play button is pressed
            playCount++;

            // Trigger the enemy's movement when the player starts moving
            if (enemy != null)
            {
                enemy.StartEnemyMovement(playCount);
            }
        }
    }

    IEnumerator ExecutePlannedMovement()
    {
        while (movementQueue.Count > 0)
        {
            string nextMove = movementQueue.Dequeue();

            switch (nextMove)
            {
                case "right":
                    yield return StartCoroutine(MovePlayer(Vector2.right));
                    break;
                case "left":
                    yield return StartCoroutine(MovePlayer(Vector2.left));
                    break;
                case "jump":
                    yield return StartCoroutine(WaitForGroundAndExecute(() => Jump()));
                    break;
                case "wait":
                    yield return new WaitForSeconds(moveDuration);
                    break;
            }
        }

        isPlaying = false;
    }

    IEnumerator MovePlayer(Vector2 direction)
    {
        float moveDistance = 3f;
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

    void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector2(horizontalForce, jumpForce), ForceMode2D.Impulse);
            isGrounded = false;  // Prevent further jumping in mid-air
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
