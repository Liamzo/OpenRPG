using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MapPointUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Elements")]
    public TextMeshProUGUI levelNameText;
    public Image levelIcon;


    
    public LevelData levelData {get; private set;}

    public event System.Action<MapPointUI, PointerEventData> OnClick = delegate { };

    public void AddLevel(LevelData level) {
        levelData = level;

        levelNameText.text = levelData.levelName;
        levelIcon.sprite = levelData.icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(this, eventData);
    }
}
