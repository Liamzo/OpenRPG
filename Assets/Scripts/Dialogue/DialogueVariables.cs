using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueVariables
{
    public VariablesState globalState;
    //public Dictionary<string, Ink.Runtime.Object> globalVariables { get; private set; }
    public Dictionary<string, Ink.Runtime.Object> currentLocalVariables { get; private set; }
    public Dictionary<ObjectHandler, Dictionary<string, Ink.Runtime.Object>> localVariables { get; private set; }

    private Story globalVariablesStory;
    private const string saveVariablesKey = "INK_VARIABLES";

    public DialogueVariables(TextAsset loadGlobalsJSON) 
    {
        // create the story
        globalVariablesStory = new Story(loadGlobalsJSON.text);
        // if we have saved data, load it
        // if (PlayerPrefs.HasKey(saveVariablesKey))
        // {
        //     string jsonState = PlayerPrefs.GetString(saveVariablesKey);
        //     globalVariablesStory.state.LoadJson(jsonState);
        // }

        // initialize the dictionary
        globalState = globalVariablesStory.variablesState;

        // globalVariables = new Dictionary<string, Ink.Runtime.Object>();
        // foreach (string name in globalVariablesStory.variablesState)
        // {
        //     Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
        //     globalVariables.Add(name, value);
        //     //Debug.Log("Initialized global dialogue variable: " + name + " = " + value);
        // }

        localVariables = new Dictionary<ObjectHandler, Dictionary<string, Ink.Runtime.Object>>();

        QuestManager.GetInstance().OnQuestProgress += QuestStageUpdated;
    }

    public void SaveVariables() 
    {
        if (globalVariablesStory != null) 
        {
            // Load the current state of all of our variables to the globals story
            VariablesToStory(globalVariablesStory);
            // NOTE: eventually, you'd want to replace this with an actual save/load method
            // rather than using PlayerPrefs.
            PlayerPrefs.SetString(saveVariablesKey, globalVariablesStory.state.ToJson());
        }
    }

    public void StartListening(Story story, ObjectHandler talker) 
    {
        if (localVariables.ContainsKey(talker)) {
            currentLocalVariables = localVariables[talker];
        } else {
            Dictionary<string, Ink.Runtime.Object> localVars = new Dictionary<string, Ink.Runtime.Object>();

            foreach (string name in story.variablesState)
            {
                // if (!globalVariables.ContainsKey(name)) {
                //     Ink.Runtime.Object value = story.variablesState.GetVariableWithName(name);
                //     localVars.Add(name, value);
                //     //Debug.Log("Initialized local dialogue variable: " + name + " = " + value);
                // }
                if (globalState[name] == null) {
                    Ink.Runtime.Object value = story.variablesState.GetVariableWithName(name);
                    localVars.Add(name, value);
                }
            }
            localVariables.Add(talker, localVars);
            currentLocalVariables = localVars;
        }
        // it's important that VariablesToStory is before assigning the listener!
        VariablesToStory(story);
        story.variablesState.variableChangedEvent += VariableChanged;

        Ink.Runtime.Object ink = new Ink.Runtime.Object();
    }

    public void StopListening(Story story, ObjectHandler talker) 
    {
        story.variablesState.variableChangedEvent -= VariableChanged;

        localVariables[talker] = currentLocalVariables;
        currentLocalVariables = null;

        // foreach (KeyValuePair<string, Ink.Runtime.Object> variable in globalVariables) {
        //     if (globalState[variable.Key] != null) {
        //         globalState[variable.Key] = (object) variable.Value;
        //     }
        // }
    }

    private void VariableChanged(string name, Ink.Runtime.Object value) 
    {
        if (globalState[name] != null) {
            globalState.SetGlobal(name, value);
        }

        // only maintain variables that were initialized from the globals ink file
        // if (globalVariables.ContainsKey(name)) 
        // {
        //     globalVariables.Remove(name);
        //     globalVariables.Add(name, value);
        // }
        if (currentLocalVariables != null) {
            if (currentLocalVariables.ContainsKey(name)) {
                currentLocalVariables.Remove(name);
                currentLocalVariables.Add(name, value);
            }
        }
    }
    public void QuestStageUpdated(string questName, int step) {
        string result = questName.Replace(" ", "_").ToLower() + "_stage";
        if (globalState[result] != null) {
            globalState[result] = step;
        }
    }

    private void VariablesToStory(Story story) 
    {
        // foreach(KeyValuePair<string, Ink.Runtime.Object> variable in globalVariables) 
        // {
        //     story.variablesState.SetGlobal(variable.Key, variable.Value);
        //     Debug.Log(variable.Key + ": " + variable.Value);
        // }
        foreach (string key in globalState) {
            story.variablesState[key] = globalState[key];
        }
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in currentLocalVariables) 
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }

    }

}