using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryExtraOptionUI : MonoBehaviour
{
    public ItemAction action;
    public TextMeshProUGUI menuText;
    public InventoryExtraOptionsMenuUI menu;

    public void Action() {
        action.Action();
        menu.OptionChosen();
    }
}
