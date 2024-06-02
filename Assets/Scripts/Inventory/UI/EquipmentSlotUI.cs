using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public Sprite baseIcon;

    ItemHandler item;

    public void AddItem (ItemHandler newEquipment) {
        item = newEquipment;

        icon.sprite = item.baseItemStats.icon;
    }

    public void ClearSlot () {
        item = null;

        icon.sprite = baseIcon;
    }

    public void UnequipItem () {
        if (item != null) {
            AudioManager.instance.PlayClipRandom(AudioID.Unequip, item.owner.audioSource);
            item.owner.GetComponent<EquipmentHandler>().Unequip(item);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            UnequipItem ();
        else if (eventData.button == PointerEventData.InputButton.Middle)
            Debug.Log("Middle");
        else if (eventData.button == PointerEventData.InputButton.Right)
            Debug.Log("Right");
    }
}
