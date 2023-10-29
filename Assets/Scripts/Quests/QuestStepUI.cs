using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class QuestStepUI : MonoBehaviour
{
    public Image background;
    public Image icon;
    public Sprite activeIcon;
    public Sprite completedIcon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI progressText;

    public QuestStep questStep {get; private set;}


    public void AddItem (QuestStep newQuestStep, bool completed) {
        questStep = newQuestStep;

        icon.sprite = completed ? completedIcon : activeIcon;
        icon.enabled = true;

        // List<string> questText = newQuestStep.GetText();
        (string name, string progress) questText = newQuestStep.GetText();

        nameText.SetText(questText.name);
        nameText.enabled = true;

        if (questText.progress != null) {
            progressText.SetText(questText.progress);
            progressText.enabled = true;
        } else {
            progressText.SetText("");
            progressText.enabled = false;
        }
    }

    public void ClearQuestStep () {
        questStep = null;

        icon.sprite = null;
        icon.enabled = false;

        nameText.SetText("");
        nameText.enabled = false;

        progressText.SetText("");
        progressText.enabled = false;
    }
}
