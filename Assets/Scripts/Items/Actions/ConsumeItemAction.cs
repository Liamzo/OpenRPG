using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Consume Action", menuName = "Inventory/Actions/Consume Action")]
public class ConsumeItemAction : BaseItemAction
{
    public int currentHealthGain;
    public int currentStaminaGain;


    public override void Action(ItemAction action)
    {
        action.item.owner.Heal(currentHealthGain);
        
        if (action.item.owner is CharacterHandler) {
            CharacterHandler character = (CharacterHandler) action.item.owner;

            character.ChangeStamina(currentStaminaGain);
        }
    }

    public override string MakeMenuName(ItemAction action)
    {
        return menuName;// + " " + action.item.objectHandler.baseStats.objectName;
    }
}
