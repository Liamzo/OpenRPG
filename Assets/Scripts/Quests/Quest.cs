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
            }
        }
    }

    public void BeingQuest() {
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

        QuestManager.GetInstance().QuestComplete(name);
    }
}