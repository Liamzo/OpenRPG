using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            //BeingQuest("Slay the Orcs");
        }
    }

    public void BeingQuest(string name) {
        foreach(Quest quest in questsAll) {
            if (quest.name == name) {
                quest.BeingQuest();
                questsActive.Add(quest);
                return;
            }
        }

        Debug.LogError("No quest found with that name");
    }

    public void QuestProgress(string name, int step) {
        OnQuestProgress(name, step);
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
}

public enum QuestStepType {
    Gather,
    Kill,
    Talk, // Might also need to trigger as a specific dialogue choice
    Vist
}