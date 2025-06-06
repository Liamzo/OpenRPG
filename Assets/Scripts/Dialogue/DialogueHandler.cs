using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueHandler : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Load Globals JSON")]
    [SerializeField] private TextAsset loadGlobalsJSON;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Image displayTalkerSprite;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    private Story currentStory;
    private ObjectHandler currentTalker;
    public bool dialogueIsPlaying { get; private set; }

    private bool canContinueToNextLine = false;

    private Coroutine displayLineCoroutine;
    private Coroutine talkingAudioCoroutine;

    private static DialogueHandler instance;

    private DialogueVariables dialogueVariables;
    private InkExternalFunctions inkExternalFunctions;

    private bool trading = false;

    private void Awake() 
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;

        inkExternalFunctions = new InkExternalFunctions();
    }

    public static DialogueHandler GetInstance() 
    {
        return instance;
    }

    private void Start() 
    {
        dialogueVariables = new DialogueVariables(loadGlobalsJSON);
        
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        // get all of the choices text 
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices) 
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update() 
    {
        // return right away if dialogue isn't playing
        if (!dialogueIsPlaying) 
        {
            return;
        }

        // handle continuing to the next line in the dialogue when submit is pressed
        // NOTE: The 'currentStory.currentChoiecs.Count == 0' part was to fix a bug after the Youtube video was made
        if (canContinueToNextLine && currentStory.currentChoices.Count == 0 && InputManager.GetInstance().GetSubmitPressed()) 
        {
            InputManager.GetInstance().RegisterSubmitPressed();
            ContinueStory();
        }


        // Check for Trader input
        if (InputManager.GetInstance().GetTradePressed()) {
            // Check we have an inventory
            if (currentTalker.GetComponent<InventoryHandler>() != null) {
                // Trade
                StartCoroutine(ExitDialogueMode());
                TradingManager.GetInstance().EnterTrade(currentTalker);
                trading = true;
            }
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON, ObjectHandler talker) 
    {
        currentStory = new Story(inkJSON.text);
        currentTalker = talker;
        dialogueIsPlaying = true;


        displayNameText.text = talker.baseStats.objectName;
        displayTalkerSprite.sprite = talker.baseStats.GetStatBlock<BaseCharacterStats>().profileSprite;
        dialoguePanel.SetActive(true);

        dialogueVariables.StartListening(currentStory, currentTalker.objectHandlerId);
        inkExternalFunctions.Bind(currentStory, currentTalker);

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode() 
    {
        yield return new WaitForSeconds(0.2f);

        dialogueVariables.StopListening(currentStory, currentTalker.objectHandlerId);
        inkExternalFunctions.Unbind(currentStory);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        displayTalkerSprite.sprite = null;
    }
    public void ExitDialogue() {
        if (dialogueIsPlaying)
            StartCoroutine(ExitDialogueMode());

        if (trading) {
            TradingManager.GetInstance().ExitTrade();
            trading = false;
        }
    }

    private void ContinueStory() 
    {
        if (currentStory.canContinue) 
        {
            // set text for the current dialogue line
            if (displayLineCoroutine != null) 
            {
                StopCoroutine(displayLineCoroutine);
            }
            if (talkingAudioCoroutine != null) 
            {
                StopCoroutine(talkingAudioCoroutine);
            }
            string nextLine = currentStory.Continue();
            // handle case where the last line is an external function
            if (nextLine.Equals("") && !currentStory.canContinue)
            {
                Player.Instance.CancelInteraction();
            }
            // otherwise, handle the normal case for continuing the story
            else 
            {
                // handle tags
                //HandleTags(currentStory.currentTags);
                displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
                talkingAudioCoroutine = StartCoroutine(PlayTalkingAudio());
            }
        }
        else 
        {
            Player.Instance.CancelInteraction();
        }
    }

    private IEnumerator DisplayLine(string line) 
    {
        // set the text to the full line, but set the visible characters to 0
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;
        // hide items while text is typing
        continueIcon.SetActive(false);
        HideChoices();

        canContinueToNextLine = false;

        bool isAddingRichTextTag = false;

        // display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {
            // if the submit button is pressed, finish up displaying the line right away
            if (InputManager.GetInstance().GetSubmitPressed()) 
            {
                dialogueText.maxVisibleCharacters = line.Length;
                break;
            }

            // check for rich text tag, if found, add it without waiting
            if (letter == '<' || isAddingRichTextTag) 
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            // if not rich text, add the next letter and wait a small time
            else 
            {
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        // actions to take after the entire line has finished displaying
        continueIcon.SetActive(true);
        DisplayChoices();

        canContinueToNextLine = true;

        if (talkingAudioCoroutine != null) 
        {
            StopCoroutine(talkingAudioCoroutine);
        }
    }

    private IEnumerator PlayTalkingAudio() {
        float soundWaitTime = 0.2f;
        float wordWaitTime = 0.5f;
        float wordWaitRange = 0.1f;
        int minSounds = 2;
        int maxSounds = 5;
        int wordSounds = Random.Range(minSounds, maxSounds);
        int curSounds = 0;

        float maxPlayTime = 5f; // Stop self after 5 seconds
        float curPlayTime = 0f;

        while (curPlayTime < maxPlayTime)
        {
            AudioManager.instance.PlayClipRandom(AudioID.Dialogue01, currentTalker.audioSource);
            curSounds++;
            yield return new WaitForSeconds(soundWaitTime);

            if (curSounds >= wordSounds) 
            {
                wordSounds = Random.Range(minSounds, maxSounds);
                curSounds = 0;
                yield return new WaitForSeconds(Random.Range(wordWaitTime-wordWaitRange, wordWaitTime+wordWaitRange));
            }
        }


        yield return null;
    }

    private void HideChoices() 
    {
        foreach (GameObject choiceButton in choices) 
        {
            choiceButton.SetActive(false);
        }
    }

    // private void HandleTags(List<string> currentTags)
    // {
    //     // loop through each tag and handle it accordingly
    //     foreach (string tag in currentTags) 
    //     {
    //         // parse the tag
    //         string[] splitTag = tag.Split(':');
    //         if (splitTag.Length != 2) 
    //         {
    //             Debug.LogError("Tag could not be appropriately parsed: " + tag);
    //         }
    //         string tagKey = splitTag[0].Trim();
    //         string tagValue = splitTag[1].Trim();
            
    //         // handle the tag
    //         switch (tagKey) 
    //         {
    //             case SPEAKER_TAG:
    //                 displayNameText.text = tagValue;
    //                 break;
    //             case PORTRAIT_TAG:
    //                 portraitAnimator.Play(tagValue);
    //                 break;
    //             case LAYOUT_TAG:
    //                 layoutAnimator.Play(tagValue);
    //                 break;
    //             case AUDIO_TAG: 
    //                 SetCurrentAudioInfo(tagValue);
    //                 break;
    //             default:
    //                 Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
    //                 break;
    //         }
    //     }
    // }

    private void DisplayChoices() 
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        // defensive check to make sure our UI can support the number of choices coming in
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: " 
                + currentChoices.Count);
        }

        int index = 0;
        // enable and initialize the choices up to the amount of choices for this line of dialogue
        foreach(Choice choice in currentChoices) 
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        // go through the remaining choices the UI supports and make sure they're hidden
        for (int i = index; i < choices.Length; i++) 
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice() 
    {
        // Event System requires we clear it first, then wait
        // for at least one frame before we set the current selected object.
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine) 
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            // NOTE: The below two lines were added to fix a bug after the Youtube video was made
            InputManager.GetInstance().RegisterSubmitPressed(); // this is specific to my InputManager script
            ContinueStory();
            AudioManager.instance.PlayClipRandom(AudioID.UI_Click);
        }
    }

    // public Ink.Runtime.Object GetVariableState(string variableName) 
    // {
    //     Ink.Runtime.Object variableValue = null;
    //     dialogueVariables.globalVariables.TryGetValue(variableName, out variableValue);
    //     if (variableValue == null) 
    //     {
    //         Debug.LogWarning("Ink Variable was found to be null: " + variableName);
    //     }
    //     return variableValue;
    // }

    // This method will get called anytime the application exits.
    // Depending on your game, you may want to save variable state in other places.
    // public void OnApplicationQuit() 
    // {
    //     dialogueVariables.SaveVariables();
    // }
}
