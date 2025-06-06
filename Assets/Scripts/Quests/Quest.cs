using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

[System.Serializable]
public class Quest
{
    public int id {get; private set;}
    JSONNode json;

    public string name;
    public string description;
    public ObjectHandler giver;

    public int stepOn {get; private set;} = -1;
    private int finalStage = 0;

    [SerializeField] public List<QuestStep> questSteps = new List<QuestStep>();
    public List<QuestStep> questStepsActive = new List<QuestStep>();

    public Quest(JSONNode json) {
        this.json = json;
        InitializeFromJson(json);
        stepOn = -1;
    }

    public void InitializeFromJson(JSONNode json) {
        name = json["name"];
        description = json["description"];

        foreach(JSONNode stepData in json["steps"]) {
            if (stepData["stepNum"] > finalStage) {
                finalStage = stepData["stepNum"];
            }

            string type = stepData["type"];
            switch(type){
                case "Kill":
                    QuestStepKill kill = new QuestStepKill(stepData, this);
                    questSteps.Add(kill);
                    break;
                case "Talk":
                    QuestStepTalk talk = new QuestStepTalk(stepData, this);
                    questSteps.Add(talk);
                    break;
                case "Gather":
                    QuestStepGather gather = new QuestStepGather(stepData, this);
                    questSteps.Add(gather);
                    break;
            }
        }
    }

    public void BeginQuest() {
        Debug.Log("Beginning Quest: " + name);




        NextStep();
    }

    void NextStep() {
        stepOn += 1;

        if (stepOn > finalStage) {
            // Quest Complete
            CompleteQuest();
            return;
        }

        QuestManager.GetInstance().QuestProgress(name, stepOn);

        foreach (QuestStep step in questSteps) {
            if (step.stepNum == stepOn) {
                step.Begin();
                questStepsActive.Add(step);
            }
        }
    }

    void BackStep() {
        // Probably need to add a Cancel function to steps
        questStepsActive.Clear();

        stepOn -= 1;

        QuestManager.GetInstance().QuestProgress(name, stepOn);

        foreach (QuestStep step in questSteps) {
            if (step.stepNum == stepOn) {
                step.Begin(); // Probably will cause issues with some steps
                questStepsActive.Add(step);
            }
        }
    }

    public void CompleteStep(QuestStep step) {
        if (!questStepsActive.Contains(step)) {
            Debug.LogWarning("Step completed was not Active");
            return;
        }

        questStepsActive.Remove(step);

        if (questStepsActive.Count == 0) {
            NextStep();
        }
    }

    void CompleteQuest() {
        Debug.Log("Complete Quest: " + name);

        foreach (JSONNode completeEvent in json["onComplete"]) {
            if (completeEvent.HasKey("removeLevel")) {
                MapManager.Instance.RemoveLevel(completeEvent["removeLevel"]);
            }
            if (completeEvent.HasKey("exp")) {
                
            }
            if (completeEvent.HasKey("item")) {
                (BaseStats baseStats, GameObject go) = PrefabManager.Instance.SpawnPrefab(completeEvent["item"]);
                ItemHandler item = GameObject.Instantiate(go).GetComponent<ItemHandler>();
                item.GetComponent<ObjectHandler>().CreateBaseObject(baseStats);
                item.PickUp(Player.Instance.character);
            }
        }

        QuestManager.GetInstance().QuestComplete(name);
    }
}