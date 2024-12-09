using UnityEngine;
using TMPro; 
using System.Collections.Generic;


public class HintSystem : MonoBehaviour
{
    public string sentence = "Tree footprints from leaving walks deer trail away."; // The full sentence
    public TMP_Text hintText; // Reference to the TextMeshPro UI Text

    private List<string> words; // Split words from the sentence
    private int revealedWordCount = 0; // Track how many words have been revealed
    private float revealInterval = 5f; // Time interval in seconds
    private float timer = 0f; // Timer to track time

    void Start()
    {
        // Initialize the word list
        words = new List<string>(sentence.Split(' '));
        hintText.text = ""; // Start with no words shown
    }

    void Update()
    {
        // Update the timer
        timer += Time.deltaTime;

        // Check if it's time to reveal more words
        if (timer >= revealInterval)
        {
            RevealWords(2); // Reveal x words
            timer = 0f; 
        }
    }

    void RevealWords(int count)
    {
        // Check if there are more words to reveal
        if (revealedWordCount < words.Count)
        {
            // Calculate how many words to reveal
            int wordsToShow = Mathf.Min(revealedWordCount + count, words.Count);

            // Build the hint text
            hintText.text = string.Join(" ", words.GetRange(0, wordsToShow));

            // Update the revealed word count
            revealedWordCount = wordsToShow;
        }
    }
}
