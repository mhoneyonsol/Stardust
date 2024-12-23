using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public static bool continueGame;
    private static int amount;
    private static TextMeshProUGUI scoreText;

    void Start()
    {
        // Reset or continue the game based on the flag
        if (!continueGame)
        {
            amount = 0; // Reset score
        }
        else
        {
            continueGame = false; // Disable the flag
        }

        // Initialize the score text component
        scoreText = this.GetComponent<TextMeshProUGUI>();
        DisplayAmount();
    }

    // Get the current score amount
    public static int GetAmount()
    {
        return amount;
    }

    // Set a new score and send it to JavaScript
    public static void SetAmount(int amountToSet)
    {
        // Update the score
        amount = amountToSet;
        DisplayAmount();

        // Check if a new high score is achieved
        if (GetAmount() > Highscore.GetAmount())
        {
            Highscore.SetAmount(GetAmount());
        }

        // Send the score to JavaScript
        SendScoreToJavaScript(amount);
    }

    // Display the score on the screen
    private static void DisplayAmount()
    {
        scoreText.text = amount.ToString();
    }

    // Function to send the score to JavaScript
    private static void SendScoreToJavaScript(int score)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        // Call the JavaScript function "SendScoreToFirestore" defined in the HTML
        Application.ExternalCall("SendScoreToFirestore", score);
        #endif
    }
}
