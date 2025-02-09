using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameMenuManager : MonoBehaviour
{
    public static GameMenuManager instance;


    // Game Menu UI
    public GameObject gameMenuUI;
    public Color lowlightText;
    public Color highlightText;
    public List<TextMeshProUGUI> menuHeaders = new (); // make new GameMenuHeaderUI class with Text and UI reference

    public List<GameMenuHeaderHolder> menuPanels = new ();


    void Awake () {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HeaderClicked(GameObject text) {
        Debug.Log("boop");
    }
}

public enum GameMenuPanels {
    Journal,
    Map,
    Tinkering
}

[System.Serializable]
public struct GameMenuHeaderHolder {
    public GameMenuPanels panel;
    public GameObject UIObject;
}