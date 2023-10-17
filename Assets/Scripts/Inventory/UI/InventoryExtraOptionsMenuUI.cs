using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class InventoryExtraOptionsMenuUI : MonoBehaviour
{
    private static InventoryExtraOptionsMenuUI instance;
    public static InventoryExtraOptionsMenuUI GetInstance() {
        return instance;
    }

    public GameObject optionPrefab;
    public List<GameObject> optionList = new List<GameObject>();

    void Awake() {
        instance = this;
        gameObject.SetActive(false);
    }

    void Update() {
        if (Input.GetMouseButtonUp(0) && gameObject.activeSelf == true) {
            if (!EventSystem.current.IsPointerOverGameObject()) {
                ClearOptions();
                gameObject.SetActive(false);
            }
        }
    }

    public void OptionChosen() {
        ClearOptions();
        gameObject.SetActive(false);
    }

    public void Move(Vector3 pos) {
        transform.position = pos;
    }

    public void AddOption(ItemAction action) {
        InventoryExtraOptionUI go = Instantiate(optionPrefab, transform).GetComponent<InventoryExtraOptionUI>();
        go.action = action;
        go.menuText.text = action.menuName;
        go.menu = this;
        optionList.Add(go.gameObject);
    }

    public void ClearOptions() {
        foreach(GameObject option in optionList) {
            Destroy(option.gameObject);
        }
        optionList.Clear();
    }
}
