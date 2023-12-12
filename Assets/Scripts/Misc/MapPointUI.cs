using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MapPointUI : MonoBehaviour, IPointerClickHandler
{
    public LevelData levelData;

    public event System.Action<MapPointUI, PointerEventData> OnClick = delegate { };

    public void AddLevel(LevelData level) {
        levelData = level;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(this, eventData);
    }
}
