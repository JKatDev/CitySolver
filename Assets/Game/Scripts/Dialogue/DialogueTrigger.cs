using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;
    [SerializeField] private string KnotName;

    [Header("NPC Voice Settings")]
    [SerializeField] private float npcTypingSpeed = 0.05f; // Default typing speed for the NPC
    [SerializeField] private float npcVoicePitch = 1.0f;   // Default voice pitch for the NPC
    [SerializeField] private float npcBeepFrequency = 1.0f;   // Default beep frequency for the NPC

    


    private bool playerInRange;

    private void Awake()
    {
        playerInRange = false;
        visualCue.SetActive(false);
    }

    private void Update()
    {
        HandlePlayerInput();
    }

    private void HandlePlayerInput()
    {
        if (playerInRange && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            visualCue.SetActive(true);

            // Replace the touch input check with a keyboard input check
            if (Input.GetKeyDown(KeyCode.E)) // Check if the space bar is pressed
            {
                // Pass both typing speed and voice pitch to the DialogueManager
                // Inside the HandlePlayerInput() method where you call EnterDialogueMode
                if (DialogueManager.GetInstance().ifSetUpStory == false)
                {
                    DialogueManager.GetInstance().SetUpStory(inkJSON, npcTypingSpeed, npcVoicePitch, npcBeepFrequency);
                    DialogueManager.GetInstance().ifSetUpStory = true;
                }
                DialogueManager.GetInstance().StartDialogue(KnotName);
            }
        }
        else
        {
            visualCue.SetActive(false);
        }
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
