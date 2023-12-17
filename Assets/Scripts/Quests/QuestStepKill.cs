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
        base.Begin();
        
        QuestManager.GetInstance().OnPlayerKill += CheckKill;
    }

    public override void Complete()
    {
        base.Complete();

        QuestManager.GetInstance().OnPlayerKill -= CheckKill;
    }

    public override (string, string) GetText()
    {
        return (name, $"{currentAmount}/{neededAmount}");
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
