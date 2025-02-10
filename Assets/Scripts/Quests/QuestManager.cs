using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class QuestManager : MonoBehaviour
{
    static QuestManager instance;

    public TextAsset questsJson;
    [SerializeField] private List<Quest> questsAll = new List<Quest>();
    [SerializeField] public List<Quest> questsActive = new List<Quest>();
    [SerializeField] public List<Quest> questsComplete = new List<Quest>();


    public event System.Action<ObjectHandler> OnPlayerKill = delegate { };
    public event System.Action<string, string, int> OnDialogueChoiceMade = delegate { };
    public event System.Action<string, int> OnQuestProgress = delegate { };


    [Header("UI")]
    public GameObject jounralUI;
    public TextMeshProUGUI activeHeader;
    public TextMeshProUGUI completedHeader;
    public GameObject questEntriesActiveParent;
    public GameObject questEntriesCompletedParent;
    public GameObject questEntryPrefab;
    public GameObject questDetailsUI;
    List<QuestEntryUI> questsActiveUI = new List<QuestEntryUI>();
    List<QuestEntryUI> questsCompleteUI = new List<QuestEntryUI>();
    QuestEntryUI selectQuestEntry;
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI questDescriptionText;
    public GameObject questStepsParent;
    List<QuestStepUI> questSteps = new List<QuestStepUI>();
    [SerializeField] private Color unmarkedColor;
    [SerializeField] private Color markedColor;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        // Create Quests
        JSONNode data = JSON.Parse(questsJson.text);

        foreach (JSONNode questData in data["Quests"]) {
            Quest quest = new Quest(questData);
            questsAll.Add(quest);
        }

        UpdateQuestDetails();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Quest FindQuest(string name) {
        foreach(Quest quest in questsAll) {
            if (quest.name == name) {
                return quest;
            }
        }

        Debug.LogError("No quest found with that name");

        return null;
    }

    // UI
    QuestEntryUI FindQuestEntry(Quest quest) {
        foreach(QuestEntryUI questEntryUI in questsActiveUI) {
            if (questEntryUI.quest == quest) {
                return questEntryUI;
            }
        }

        Debug.LogError("No quest entry found with that quest");

        return null;
    }

    public void BeingQuest(string name) {
        Quest quest = FindQuest(name);
        quest.BeingQuest();
        questsActive.Add(quest);


        // UI
        QuestEntryUI questEntry = Instantiate(questEntryPrefab, questEntriesActiveParent.transform).GetComponent<QuestEntryUI>();
        questEntry.AddItem(quest);
        questsActiveUI.Add(questEntry);
        questEntry.OnClick += OnPointerClick;
    }

    public void QuestProgress(string name, int step) {
        OnQuestProgress(name, step);
    }

    public void QuestComplete(string name) {
        Quest quest = FindQuest(name);

        questsActive.Remove(quest);
        questsComplete.Add(quest);


        // UI
        QuestEntryUI questEntryUI = FindQuestEntry(quest);
        questEntryUI.transform.SetParent(questEntriesCompletedParent.transform);
    }


    // Kill Quests

    public void RegisterOnDeath(ObjectHandler obj) {
        obj.OnDeath += CollectDeath;
    }

    void CollectDeath(ObjectHandler obj) {
        // TODO: Check if it was the player that killed the object
        OnPlayerKill(obj);
    }


    // Talk Quests

    public void DialogueChoiceListener(string questName, string questStep, int choiceId) {
        OnDialogueChoiceMade(questName, questStep, choiceId);
    }


    public static QuestManager GetInstance() {
        return instance;
    }



    // UI
    public void OpenJournal() {
        if (jounralUI.activeSelf == false) {
            jounralUI.SetActive(true);
        }

        UpdateJournalUI();
    }
    public void CloseJournal() {
        if (jounralUI.activeSelf == true) {
            jounralUI.SetActive(false);
        }
    }

    void UpdateJournalUI() {
        UpdateQuestDetails();
    }

    public void OnPointerClick(QuestEntryUI slot, PointerEventData eventData)
    {
        if (slot == selectQuestEntry) {
            selectQuestEntry.background.color = unmarkedColor;
            selectQuestEntry = null;
            UpdateQuestDetails();
            return;
        }

        if (selectQuestEntry != null)
            selectQuestEntry.background.color = unmarkedColor;

        selectQuestEntry = slot;
        selectQuestEntry.background.color = markedColor;

        UpdateQuestDetails();
    }

    void UpdateQuestDetails() {
        foreach (QuestStepUI questStepUI in questSteps) {
            questStepUI.ClearQuestStep();
            questStepUI.gameObject.SetActive(false);
        }

        questSteps.Clear();


        if (selectQuestEntry == null) {
            questNameText.text = "";
            questDescriptionText.text = "";
            return;
        }

        questNameText.text = selectQuestEntry.quest.name;
        questDescriptionText.text = selectQuestEntry.quest.description;


        foreach (QuestStep questStep in selectQuestEntry.quest.questSteps) {
            if (questStep.stepNum < selectQuestEntry.quest.stepOn) {
                QuestStepUI questStepUI = ObjectPoolManager.Instance.GetPooledObject(PoolIdentifiers.QuestStepUI).GetComponent<QuestStepUI>();
                questStepUI.transform.SetParent(questStepsParent.transform, false);
                questStepUI.AddItem(questStep, true);
                questSteps.Add(questStepUI);
                questStepUI.gameObject.SetActive(true);
            } else if (questStep.stepNum == selectQuestEntry.quest.stepOn) {
                QuestStepUI questStepUI = ObjectPoolManager.Instance.GetPooledObject(PoolIdentifiers.QuestStepUI).GetComponent<QuestStepUI>();
                questStepUI.transform.SetParent(questStepsParent.transform, false);
                questStepUI.AddItem(questStep, false);
                questSteps.Add(questStepUI);
                questStepUI.gameObject.SetActive(true);
            } else {
                // Step not started yet so don't show
                break;
            }
        }
    }

    public void ShowActiveQuests(bool showActive) {
        if (showActive && questEntriesActiveParent.activeSelf == false) {
            questEntriesActiveParent.SetActive(true);
            questEntriesCompletedParent.SetActive(false);

            activeHeader.color = GameMenuManager.Instance.highlightText;
            completedHeader.color = GameMenuManager.Instance.lowlightText;

            // if (selectQuestEntry != null){
            //     selectQuestEntry.background.color = unmarkedColor;
            //     selectQuestEntry = null;
            //     UpdateQuestDetails();
            // }
        }
        
        if (!showActive && questEntriesActiveParent.activeSelf == true) {
            questEntriesActiveParent.SetActive(false);
            questEntriesCompletedParent.SetActive(true);

            activeHeader.color = GameMenuManager.Instance.lowlightText;
            completedHeader.color = GameMenuManager.Instance.highlightText;

            // if (selectQuestEntry != null){
            //     selectQuestEntry.background.color = unmarkedColor;
            //     selectQuestEntry = null;
            //     UpdateQuestDetails();
            // }
        }

    }
}

public enum QuestStepType {
    Gather,
    Kill,
    Talk, // Might also need to trigger as a specific dialogue choice
    Vist
}