using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

[System.Serializable]
public class QuestStepTalk : QuestStep
{
    public QuestStepTalk(JSONNode json, Quest parent) : base(json, parent) {
        
    }

    public override void Begin()
    {
        base.Begin();

        QuestManager.GetInstance().OnDialogueChoiceMade += OnDialogueChoiceMade;
    }

    void OnDialogueChoiceMade(string questName, string stepName, int choiceId) {
        // TODO: Make good

        if (this.name == stepName) {
            Complete();
        }
    }
}
