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

    private DialogueManager _dialogueManager => DialogueManager.I;

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
        if (playerInRange && !_dialogueManager.dialogueIsPlaying)
        {
            visualCue.SetActive(true);

            // Replace the touch input check with a keyboard input check
            if (Input.GetKeyDown(KeyCode.E)) // Check if the space bar is pressed
            {
                // Pass both typing speed and voice pitch to the DialogueManager
                // Inside the HandlePlayerInput() method where you call EnterDialogueMode
                if (_dialogueManager.ifSetUpStory == false)
                {
                    _dialogueManager.SetUpStory(inkJSON, npcTypingSpeed, npcVoicePitch, npcBeepFrequency);
                    _dialogueManager.ifSetUpStory = true;
                }
                _dialogueManager.StartDialogue(KnotName);
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
