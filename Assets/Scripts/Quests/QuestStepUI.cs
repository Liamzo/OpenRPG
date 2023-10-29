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

    public QuestStep questStep {get; private set;}


    public void AddItem (QuestStep newQuestStep, bool completed) {
        questStep = newQuestStep;

        icon.sprite = completed ? completedIcon : activeIcon;
        icon.enabled = true;

        nameText.SetText(newQuestStep.name);
        nameText.enabled = true;
    }

    public void ClearQuestStep () {
        questStep = null;

        icon.sprite = null;
        icon.enabled = false;

        nameText.SetText("");
        nameText.enabled = false;
    }
}
