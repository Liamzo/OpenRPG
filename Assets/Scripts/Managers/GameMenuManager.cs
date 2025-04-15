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
    public List<TextMeshProUGUI> menuHeaders = new (); // make new GameMenuHeaderUI class with Text and UI reference
    public Image leftArrow;
    public Image rightArrow;


    public Color lowlightText;
    public Color highlightText;
    public Material lowlightMaterial;
    public Material highlightMaterial;

    
    public GameMenuPanels? currentPanel = null;
    public GameObject currentPanelGO = null;
    Coroutine runningScrambleCoroutine = null;
    Coroutine runningTransitionCoroutine = null;


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

            if (runningTransitionCoroutine != null)
                return;

            runningTransitionCoroutine = StartCoroutine(PanelTransition(GameMenuPanels.Map));
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

            if (runningTransitionCoroutine != null)
                return;

            runningTransitionCoroutine = StartCoroutine(PanelTransition(GameMenuPanels.Journal));
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

            if (runningTransitionCoroutine != null)
                return;

            runningTransitionCoroutine = StartCoroutine(PanelTransition(GameMenuPanels.Tinkering));
            panelChanged = true;
        }
        else if (Input.GetKeyDown(KeyCode.H)) 
        {
            if (currentPanel == GameMenuPanels.Research) {
                // Close Game Menu
                ResearchManager.Instance.CloseResearchManager();
                CloseGameMenu();
                return;
            }

            if (runningTransitionCoroutine != null)
                return;

            runningTransitionCoroutine = StartCoroutine(PanelTransition(GameMenuPanels.Research));
            panelChanged = true;
        }
    
        if (panelChanged) {
            if (gameMenuUI.activeSelf == false)
                gameMenuUI.SetActive(true);
        }
    }

    IEnumerator PanelTransition(GameMenuPanels openPanel) {
        float fadeOutDuration = 0.1f;
        float pauseDuration = 0.2f;
        float fadeInDuration = 0.1f;

        float timer = 0f;

        SetHeaders(openPanel);

        AudioManager.instance.PlayClipRandom(AudioID.PanelChange);

        GameMenuPanels? oldPanel = currentPanel;
        currentPanel = openPanel;

        if (oldPanel != null) {
            while (timer < fadeOutDuration) {
                timer += Time.deltaTime;

                currentPanelGO.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);

                yield return null;
            }

            ClosePanelByEnum(oldPanel.Value); // Close currently open Panel first
            timer = 0f;
            
            yield return new WaitForSeconds(pauseDuration);
        }


        OpenPanelByEnum(openPanel);

        while (timer < fadeInDuration) {
            timer += Time.deltaTime;

            currentPanelGO.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0f, 1f, timer / fadeInDuration);

            yield return null;
        }

        currentPanelGO.GetComponent<CanvasGroup>().alpha = 1f;

        runningTransitionCoroutine = null;
    }


    void SetHeaders(GameMenuPanels newPanel) {
        if (runningScrambleCoroutine != null)
            StopCoroutine(runningScrambleCoroutine);

        runningScrambleCoroutine = StartCoroutine(ScrambleTextTransition(newPanel));
    }
    IEnumerator ScrambleTextTransition(GameMenuPanels newPanel) {
        float elapsedTime = 0f;
        float scramableDuration = 0.4f;
        float scramblePause = 0.05f;

        // List<string> currentWords = new ();
        // foreach (TextMeshProUGUI text in menuHeaders) {
        //     currentWords.Add(text.text);
        // }

        List<string> newWords = new ();
        for (int i = 0; i < 5; i++) {
            int index = (int) newPanel + i - 2;
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
        if (runningTransitionCoroutine != null) {
            StopCoroutine(runningTransitionCoroutine);
            runningTransitionCoroutine = null;
        }

        gameMenuUI.SetActive(false);
        currentPanel = null;
        currentPanelGO = null;

        AudioManager.instance.PlayClipRandom(AudioID.CloseUI);
    }


    void OpenPanelByEnum(GameMenuPanels panel) {
        currentPanel = panel;

        if (panel == GameMenuPanels.Journal) {
            QuestManager.GetInstance().OpenJournal();
            currentPanelGO = QuestManager.GetInstance().jounralUI;
        } else if (panel == GameMenuPanels.Map) {
            MapManager.Instance.OpenMap();
            currentPanelGO = MapManager.Instance.mapUI;
        } else if (panel == GameMenuPanels.Tinkering) {
            ModManager.Instance.OpenModManager();
            currentPanelGO = ModManager.Instance.modManagerUI;
        } else if (panel == GameMenuPanels.Research) {
            ResearchManager.Instance.OpenResearchManager();
            currentPanelGO = ResearchManager.Instance.researchManagerUI;
        }
    }

    void ClosePanelByEnum (GameMenuPanels panel) {
        if (panel == GameMenuPanels.Journal) { 
            QuestManager.GetInstance().CloseJournal();
        } else if (panel == GameMenuPanels.Map) {
            MapManager.Instance.CloseMap();
        } else if (panel == GameMenuPanels.Tinkering) {
            ModManager.Instance.CloseModManager();
        } else if (panel == GameMenuPanels.Research) {
            ResearchManager.Instance.CloseResearchManager();
        }
    }


    public void ArrowClicked(int index) {
        HeaderClicked(index);

        AudioManager.instance.PlayClipRandom(AudioID.UI_Click);
    }

    public void HeaderClicked(int index) {
        if (index == 0) return; // Maybe reload the current panel

        if (runningTransitionCoroutine != null) return;

        int newPanel = (int) currentPanel + index;
        if (newPanel < 0) 
            newPanel = System.Enum.GetValues(typeof(GameMenuPanels)).Length + newPanel;
        newPanel %= System.Enum.GetValues(typeof(GameMenuPanels)).Length;

        runningTransitionCoroutine = StartCoroutine(PanelTransition((GameMenuPanels) newPanel));
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
    Tinkering,
    Research
}

[System.Serializable]
public struct GameMenuHeaderHolder {
    public GameMenuPanels panel;
    public GameObject UIObject;
}