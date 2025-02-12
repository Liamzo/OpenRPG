using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameMenuManager : MonoBehaviour
{
    public static GameMenuManager Instance;


    // Game Menu UI
    public GameObject gameMenuUI;
    public Color lowlightText;
    public Color highlightText;
    public List<TextMeshProUGUI> menuHeaders = new (); // make new GameMenuHeaderUI class with Text and UI reference
    public Image leftArrow;
    public Image rightArrow;

    
    public GameMenuPanels? currentPanel = null;
    Coroutine runningScrambleCoroutine = null;


    void Awake () {
        Instance = this;

        leftArrow.alphaHitTestMinimumThreshold = 0.1f;
        rightArrow.alphaHitTestMinimumThreshold = 0.1f;
    }

    // Start is called before the first frame update
    void Start()
    {
        LevelManager.instance.LoadLevelPre += LevelLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        bool panelChanged = false;

        if (InputManager.GetInstance().GetMapPressed()) 
        {
            if (currentPanel == GameMenuPanels.Map) {
                // Close Game Menu
                MapManager.Instance.CloseMap();
                CloseGameMenu();
                return;
            }

            if (currentPanel != null)
                ClosePanelByEnum(currentPanel.Value); // Close currently open Panel first
            MapManager.Instance.OpenMap();
            currentPanel = GameMenuPanels.Map;
            panelChanged = true;
        } 
        else if (InputManager.GetInstance().GetJournalPressed()) 
        {
            if (currentPanel == GameMenuPanels.Journal) {
                // Close Game Menu
                QuestManager.GetInstance().CloseJournal();
                CloseGameMenu();
                return;
            }
            
            if (currentPanel != null)
                ClosePanelByEnum(currentPanel.Value); // Close currently open Panel first
            QuestManager.GetInstance().OpenJournal();
            currentPanel = GameMenuPanels.Journal;
            panelChanged = true;
        } 
        else if (Input.GetKeyDown(KeyCode.K)) 
        {
            if (currentPanel == GameMenuPanels.Tinkering) {
                // Close Game Menu
                ModManager.Instance.CloseModManager();
                CloseGameMenu();
                return;
            }
            
            if (currentPanel != null)
                ClosePanelByEnum(currentPanel.Value); // Close currently open Panel first
            ModManager.Instance.OpenModManager();
            currentPanel = GameMenuPanels.Tinkering;
            panelChanged = true;
        }
    
        if (panelChanged) {
            if (gameMenuUI.activeSelf == false)
                gameMenuUI.SetActive(true);

            SetHeaders();
        }
    }


    void SetHeaders() {
        if (runningScrambleCoroutine != null)
            StopCoroutine(runningScrambleCoroutine);

        runningScrambleCoroutine = StartCoroutine(ScrambleTextTransition());
    }
    IEnumerator ScrambleTextTransition() {
        // List<string> currentWords = new ();
        // foreach (TextMeshProUGUI text in menuHeaders) {
        //     currentWords.Add(text.text);
        // }

        List<string> newWords = new ();
        for (int i = 0; i < 5; i++) {
            int index = (int) currentPanel + i - 2;
            if (index < 0) 
                index = System.Enum.GetValues(typeof(GameMenuPanels)).Length + index;
            index %= System.Enum.GetValues(typeof(GameMenuPanels)).Length;

            newWords.Add(((GameMenuPanels) index).ToString());
        }


        // Get all characters in the current TMP font
        List<char> possibleCharacters = new List<char>();
        foreach (TMP_Character glyph in menuHeaders[0].font.characterTable) {
            possibleCharacters.Add((char)glyph.unicode);
        }


        float elapsedTime = 0f;
        float scramableDuration = 0.4f;
        float scramblePause = 0.05f;

        int numberOfScrambels = (int) (scramableDuration / scramblePause);

        while (elapsedTime < scramableDuration)
        {
            // Scramble letters
            foreach (TextMeshProUGUI text in menuHeaders) {
                //char[] randomChars = new char[text.text.Length];
                char[] randomChars = new char[10];

                for (int i = 0; i < randomChars.Length; i++) {
                    randomChars[i] = possibleCharacters[Random.Range(0, possibleCharacters.Count)];
                }

                text.text = new string(randomChars);
            }
            
            elapsedTime += scramblePause;
            yield return new WaitForSeconds(scramblePause);
        }

        for (int i = 0; i < 5; i++) {
            menuHeaders[i].SetText(newWords[i]);
        }

        runningScrambleCoroutine = null;
    }


    void CloseGameMenu () {
        gameMenuUI.SetActive(false);
        currentPanel = null;

        AudioManager.instance.PlayClipRandom(AudioID.CloseUI);
    }


    void OpenPanelByEnum(GameMenuPanels panel) {
        if (panel == GameMenuPanels.Journal) { 
            ClosePanelByEnum(currentPanel.Value); // Close currently open Panel first
            QuestManager.GetInstance().OpenJournal();
            currentPanel = GameMenuPanels.Journal;
        } else if (panel == GameMenuPanels.Map) {
            ClosePanelByEnum(currentPanel.Value); // Close currently open Panel first
            MapManager.Instance.OpenMap();
            currentPanel = GameMenuPanels.Map;
        } else if (panel == GameMenuPanels.Tinkering) {
            ClosePanelByEnum(currentPanel.Value); // Close currently open Panel first
            ModManager.Instance.OpenModManager();
            currentPanel = GameMenuPanels.Tinkering;
        }

        SetHeaders();
    }

    void ClosePanelByEnum (GameMenuPanels panel) {
        if (panel == GameMenuPanels.Journal) { 
            QuestManager.GetInstance().CloseJournal();
        } else if (panel == GameMenuPanels.Map) {
            MapManager.Instance.CloseMap();
        } else if (panel == GameMenuPanels.Tinkering) {
            ModManager.Instance.CloseModManager();
        }
    }


    public void HeaderClicked(int index) {
        if (index == 0) return; // Maybe reload the current panel

        int newHeader = (int) currentPanel + index;
        if (newHeader < 0) 
            newHeader = System.Enum.GetValues(typeof(GameMenuPanels)).Length + newHeader;
        newHeader %= System.Enum.GetValues(typeof(GameMenuPanels)).Length;

        OpenPanelByEnum((GameMenuPanels) newHeader);
    }




    public void LevelLoaded() {
        if (currentPanel != null)
            ClosePanelByEnum(currentPanel.Value);
        
        CloseGameMenu ();
    }
}

public enum GameMenuPanels {
    Journal,
    Map,
    Tinkering
}

[System.Serializable]
public struct GameMenuHeaderHolder {
    public GameMenuPanels panel;
    public GameObject UIObject;
}