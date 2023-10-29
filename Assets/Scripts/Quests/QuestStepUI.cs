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
    public TextMeshProUGUI nameText;

    public QuestStep questStep {get; private set;}


    public void AddItem (QuestStep newQuestStep) {
        questStep = newQuestStep;

        icon.enabled = true;

        nameText.SetText(newQuestStep.name);
        nameText.enabled = true;
    }
}
