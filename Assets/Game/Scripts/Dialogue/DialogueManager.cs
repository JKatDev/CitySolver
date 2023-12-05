using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils.Singleton;

public class DialogueManager : Singleton<DialogueManager>
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;
    private Animator layoutAnimator;

    [Header("Choices UI")]
    [SerializeField] private Button[] choiceButtons;
    private TextMeshProUGUI[] choicesText;

    [Header("Typing Effect")]
    [SerializeField] private float typingSpeed = 0.05f;
    private Coroutine typingCoroutine;

    [Header("Voice")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] voiceClips;
    [SerializeField] private float voicePitch = 1.0f;
    [SerializeField] private float beepFrequency = 1.0f;
    [SerializeField] private bool stopAudioSource;

    public bool ifSetUpStory = false;  // To determine if the story is set up or not

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";

    private Story currentStory;
    public bool dialogueIsPlaying { get; private set; }

    private bool isTyping; 

    private bool isWaitingForChoiceToBeMade = false;

    private int selectedChoiceIndex = 0;

    private PlayerController playerController => PlayerController.I;

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        // get the layout animator
        layoutAnimator = dialoguePanel.GetComponent<Animator>();

        if (choiceButtons == null)
        {
            Debug.LogError("Choice buttons array is not initialized.");
            return;
        }

        choicesText = new TextMeshProUGUI[choiceButtons.Length];

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int localIndex = i;
            choiceButtons[i].onClick.AddListener(() => MakeChoice(localIndex));
            choicesText[i] = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        }

    }


    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }

        // Pressing space should only continue the story if a choice is not being waited on.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                CompleteSentence();
            }
            else if (!isWaitingForChoiceToBeMade)
            {
                ContinueStory();
            }
            // Do not include an else statement here, as we don't want Space to select a choice
        }

        // Use arrow keys to navigate choices
        if (isWaitingForChoiceToBeMade)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                selectedChoiceIndex = Mathf.Min(selectedChoiceIndex - 1, currentStory.currentChoices.Count + 1);
                UpdateChoiceSelection();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                selectedChoiceIndex = Mathf.Max(selectedChoiceIndex + 1, 0);
                UpdateChoiceSelection();
            }

            // Use the Enter key to select a choice when choices are being waited on.
            if (Input.GetKeyDown(KeyCode.Return))
            {
                MakeChoice(selectedChoiceIndex);
            }
        }
    }


    private void UpdateChoiceSelection()
    {
        if (choiceButtons.Length > 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(choiceButtons[selectedChoiceIndex].gameObject);
        }
    }


    public void SetUpStory(TextAsset inkJSON, float npcTypingSpeed, float npcVoicePitch, float npcBeepFrequency)
    {
        currentStory = new Story(inkJSON.text);

        // Set the typing speed and voice pitch from the parameters
        typingSpeed = npcTypingSpeed;
        audioSource.pitch = npcVoicePitch;

        beepFrequency = npcBeepFrequency;

    }

    public void StartDialogue(string KnotName)
    {
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        // reset portrait, layout, and speaker
        displayNameText.text = "???";
        portraitAnimator.Play("default");
        layoutAnimator.Play("left");

        // Freeze the player movement
        if (playerController != null)
        {
            playerController.SetIsFrozen(false);
        }

        currentStory.ChoosePathString(KnotName);

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        Debug.Log("Exiting Dialogue Mode...");
        yield return new WaitForSeconds(0.2f);

        // Unfreeze the player movement
        if (playerController != null)
        {
            playerController.SetIsFrozen(true);
        }

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        Debug.Log("Dialogue Mode Exited");
    }


    private void ContinueStory()
    {
        // If the story can continue
        if (currentStory.canContinue)
        {
            // Stop any ongoing typing coroutine
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            // Start typing out the next line of dialogue
            typingCoroutine = StartCoroutine(TypeSentence(currentStory.Continue()));

            // Display the choices if there are any
            DisplayChoices();
            //handle tags
            HandleTags(currentStory.currentTags);
        }
        // If the story can't continue but there are choices available
        else if (currentStory.currentChoices.Count > 0)
        {
            dialogueIsPlaying = true;
            DisplayChoices();
        }
        // If the story can't continue and there are no choices available, exit dialogue mode
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        // loop through each tag and handle it accordingly
        foreach (string tag in currentTags)
        {
            // parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            // handle the tag
            switch (tagKey)
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    break;
                case LAYOUT_TAG:
                    layoutAnimator.Play(tagValue);
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        float nextBeepTime = 0f; // Time when next beep should play

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            if (Time.time >= nextBeepTime)
            {
                if (voiceClips != null && voiceClips.Length > 0 && audioSource != null && !audioSource.isPlaying)
                {
                    AudioClip randomBeep = voiceClips[Random.Range(0, voiceClips.Length)];
                    audioSource.PlayOneShot(randomBeep);
                    nextBeepTime = Time.time + beepFrequency;
                }
            }
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }




    private void CompleteSentence()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        dialogueText.text = currentStory.currentText; // Display full text immediately
        isTyping = false; // Update typing status
    }
    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        // Check if there are any choices to display.
        isWaitingForChoiceToBeMade = currentChoices.Count > 0;

        if (isWaitingForChoiceToBeMade)
        {
            Debug.Log("Choices available: " + currentChoices.Count);

            // If there are choices, enable the choice UI and set the text for each one.
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i < currentChoices.Count)
                {
                    choiceButtons[i].gameObject.SetActive(true); // Activate the button gameobject
                    choicesText[i].text = currentChoices[i].text;
                }
                else
                {
                    // Disable any choice UI that is not needed.
                    choiceButtons[i].gameObject.SetActive(false); // Deactivate the button gameobject
                }
            }

            // Automatically select the first choice for better UX.
            StartCoroutine(SelectFirstChoice());
            selectedChoiceIndex = 0; // Reset to the first choice
            UpdateChoiceSelection(); // Highlight the first choice
        }
        else
        {
            // If no choices are present, make sure they are all disabled.
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                choiceButtons[i].gameObject.SetActive(false); // Deactivate the button gameobject
            }
        }
    }


    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        Debug.Log("Choice made: " + choiceIndex);

        currentStory.ChooseChoiceIndex(choiceIndex);
        isWaitingForChoiceToBeMade = false;

        ContinueStory();
    }
}