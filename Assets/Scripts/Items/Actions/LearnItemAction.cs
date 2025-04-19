using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Learn Action", menuName = "Inventory/Actions/Learn Action")]
public class LearnItemAction : BaseItemAction
{
    public string researchId;


    public override void Action(ItemAction action)
    {
        bool used = ResearchManager.Instance.SetResearchState(researchId, ResearchState.Researched);

        if (used) {
            action.item.owner.GetComponent<InventoryHandler>().Remove(action.item);
            Destroy(action.item.gameObject);
        }
    }

    public override string MakeMenuName(ItemAction action)
    {
        return menuName;// + " " + action.item.objectHandler.baseStats.objectName;
    }
}
