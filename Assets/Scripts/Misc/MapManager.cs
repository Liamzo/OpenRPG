using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    public GameObject mapUI;
    public GameObject mapPointParent;
    public GameObject mapPointPrefab;

    public List<LevelData> startingLevels; // Very temp, do something much better
    public List<LevelData> knownLevels = new List<LevelData>();
    List<MapPointUI> mapPointUIs = new List<MapPointUI>();


    private void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (LevelData level in startingLevels) {
            AddLevel(level);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.GetInstance().GetMapPressed()) {
            OnMap();
        }
    }


    public void OnMap() {
        if (mapUI.activeSelf == true) {
            CloseMap();
            AudioManager.instance.PlayClipRandom(AudioID.CloseUI);
        } else {
            OpenMap();
            AudioManager.instance.PlayClipRandom(AudioID.OpenMap);
        }
    }

    public void OpenMap() {
        if (mapUI.activeSelf == false) {
            mapUI.SetActive(true);
        }

        foreach (MapPointUI mapPoint in mapPointUIs) {
            //Debug.Log(mapPoint.transform.position);
        }
    }
    public void CloseMap() {
        if (mapUI.activeSelf == true) {
            mapUI.SetActive(false);
        }
    }

    public void AddLevel(LevelData level) {
        MapPointUI mapPointUI = Instantiate(mapPointPrefab).GetComponent<MapPointUI>();
        mapPointUI.transform.SetParent(mapPointParent.transform, false);
        mapPointUI.AddLevel(level);

        mapPointUI.OnClick += LevelSelected;

        knownLevels.Add(level);
        mapPointUIs.Add(mapPointUI);
    }
    public void AddLevel(string level) {
        LevelData levelData = LevelManager.instance.FindLevelWithName(level);

        AddLevel(levelData);
    }

    public void RemoveLevel(LevelData level) {
        foreach (MapPointUI mapPoint in mapPointUIs) {
            if (mapPoint.levelData == level) {
                mapPointUIs.Remove(mapPoint);
                mapPoint.OnClick -= LevelSelected;
                Destroy(mapPoint.gameObject);
                break;
            }
        }

        knownLevels.Remove(level);
    }
    public void RemoveLevel(string level) {
        LevelData levelData = LevelManager.instance.FindLevelWithName(level);

        RemoveLevel(levelData);
    }

    void LevelSelected(MapPointUI mapPoint, PointerEventData eventData) {
        // Check if we can travel. In combat, in a dungeon, etc.

        LevelManager.instance.LoadLevel(mapPoint.levelData);
        AudioManager.instance.PlayClipRandom(AudioID.Travel);
    }
}
