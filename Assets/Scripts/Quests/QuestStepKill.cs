using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

[System.Serializable]
public class QuestStepKill : QuestStep
{
    ObjectType target;
    public int neededAmount;
    public int currentAmount;
    
    public QuestStepKill(JSONNode json, Quest parent) : base(json, parent) {
        target = System.Enum.Parse<ObjectType>(json["target"]);
        neededAmount = (int)json["amount"];
        currentAmount = 0;
    }

    public override void Begin()
    {
        Debug.Log("Beginning Step: " + name);

        QuestManager.GetInstance().OnPlayerKill += CheckKill;
    }

    void CheckKill(ObjectHandler obj) {
        if (obj.objectType == target) {
            currentAmount += 1;
        }

        if (currentAmount >= neededAmount) {
            Complete();
        }
    }

}