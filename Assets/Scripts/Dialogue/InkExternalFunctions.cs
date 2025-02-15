using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class InkExternalFunctions
{
    public void Bind(Story story, ObjectHandler currentTalker)
    {
        story.BindExternalFunction("startQuest", (string questName) => StartQuest(questName));
        story.BindExternalFunction("dialogueChoiceMade", (string questName, string questStep, int choiceId) => DialogueChoiceMade(questName, questStep, choiceId));
    }

    public void Unbind(Story story) 
    {
        story.UnbindExternalFunction("startQuest");
        story.UnbindExternalFunction("dialogueChoiceMade");
    }


    public void StartQuest(string questName) {
        QuestManager.GetInstance().BeginQuest(questName);
    }
    public void DialogueChoiceMade(string questName, string questStep, int choiceId) {
        QuestManager.GetInstance().DialogueChoiceListener(questName, questStep, choiceId);
    }
    
}