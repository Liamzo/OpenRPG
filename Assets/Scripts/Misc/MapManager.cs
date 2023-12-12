using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            MapPointUI mapPointUI = Instantiate(mapPointPrefab).GetComponent<MapPointUI>();
            mapPointUI.transform.SetParent(mapPointParent.transform, false);
            mapPointUI.AddLevel(level);

            knownLevels.Add(level);
            mapPointUIs.Add(mapPointUI);
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
        } else {
            OpenMap();
        }
    }

    public void OpenMap() {
        if (mapUI.activeSelf == false) {
            mapUI.SetActive(true);
        }
    }
    public void CloseMap() {
        if (mapUI.activeSelf == true) {
            mapUI.SetActive(false);
        }
    }

    public void AddLevel(LevelData level) {
        MapPointUI mapPointUI = Instantiate(mapPointPrefab).GetComponent<MapPointUI>();
        mapPointUI.AddLevel(level);

        knownLevels.Add(level);
        mapPointUIs.Add(mapPointUI);
    }

    public void Removelevel(LevelData level) {
        foreach (MapPointUI mapPoint in mapPointUIs) {
            if (mapPoint.levelData == level) {
                mapPointUIs.Remove(mapPoint);
                Destroy(mapPoint.gameObject);
                break;
            }
        }

        knownLevels.Remove(level);
    }
}
