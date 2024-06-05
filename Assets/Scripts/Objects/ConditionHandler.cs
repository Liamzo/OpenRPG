using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionHandler
{
    public List<BaseCondition> conditions { get; private set; } = new List<BaseCondition>();

    List<BaseCondition> conditionsToRemove = new List<BaseCondition>();

    public void Tick() 
    {
        foreach (BaseCondition condition in conditionsToRemove)
        {
            if (conditions.Remove(condition) == false) {
                Debug.LogWarning("Condition not found: " + condition.ToString());
            }
        }
        conditionsToRemove.Clear();

        foreach (BaseCondition condition in conditions) {
            condition.Tick();
        }
    }

    public void AddCondition(BaseCondition condition)
    {
        foreach (BaseCondition activeCondition in conditions) {
            if (condition.GetType() == activeCondition.GetType()) return; // Can't have 2 of same condition
        }

        conditions.Add(condition);

        condition.Start();
    }

    public void RemoveCondition(BaseCondition condition)
    {
        conditionsToRemove.Add(condition);
    }
}
