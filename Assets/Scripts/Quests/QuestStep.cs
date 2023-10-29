using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

[System.Serializable]
public abstract class QuestStep
{
    Quest parentQuest;
    public string name;
    public int stepNum;

    public QuestStep(JSONNode json, Quest parent) {
        name = json["name"];
        stepNum = json["stepNum"];

        parentQuest = parent;
    }

    public abstract void Begin();
    public virtual void Complete() {
        parentQuest.CompleteStep(this);
    }
    
    public virtual (string, string) GetText() {
        return (name, null);
    }
}
