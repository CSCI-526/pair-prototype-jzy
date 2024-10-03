using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  
public class WinLoseController : MonoBehaviour
{
    public Transform player;         // Reference to the player object
    public Camera mainCamera;        // Reference to the main camera
    public TextMeshProUGUI winLoseText;
    public Transform enemy;          // Reference to the enemy object

    void Start()
    {
        winLoseText.text = "";
    }

    void Update()
    {
        CheckIfPlayerWon();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision Detected with: " + collision.gameObject.name);
        // Check if the player collides with the specific enemy object
        if (collision.gameObject == enemy.gameObject)
        {
            PlayerLoses();
        }
    }

    // Method to check if the player has moved out of the camera's right bounds
    void CheckIfPlayerWon()
    {
        // Get the player's position in screen space
        Vector3 playerPositionInView = mainCamera.WorldToViewportPoint(player.position);

        // Check if the player's x-position is greater than 1 (out of the right side of the camera view)
        if (playerPositionInView.x > 1)
        {
            // Display "Win!!" message
            winLoseText.text = "Player Wins!!";
            player.GetComponent<PlayerController>().enabled = false;
        }
    }

    // Method to handle player losing
    void PlayerLoses()
    {
        winLoseText.text = "Enemy Wins...";
        winLoseText.color = Color.yellow;
        player.GetComponent<PlayerController>().enabled = false;  // Stop player movement
    }
}
