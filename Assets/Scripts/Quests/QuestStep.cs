using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

[System.Serializable]
public abstract class QuestStep
{
    JSONNode json;
    Quest parentQuest;
    public string name;
    public int stepNum;

    public QuestStep(JSONNode json, Quest parent) {
        this.json = json;

        name = json["name"];
        stepNum = json["stepNum"];

        parentQuest = parent;
    }

    public virtual void Begin() {
        Debug.Log("Beginning Step: " + name);

        foreach (JSONNode beginEvent in json["onBegin"]) {
            if (beginEvent.HasKey("addLevel")) {
                MapManager.instance.AddLevel(beginEvent["addLevel"]);
            }
        }
    }
    public virtual void Complete() {
        parentQuest.CompleteStep(this);
    }
    
    public virtual (string, string) GetText() {
        return (name, null);
    }
}
