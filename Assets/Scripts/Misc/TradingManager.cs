using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class TradingManager : MonoBehaviour
{
    private static TradingManager instance;

    [SerializeField] private ObjectHandler trader;
    [SerializeField] private ObjectHandler player;


    [SerializeField] private GameObject tradePanel;

    [SerializeField] private TextMeshProUGUI traderNameText;
    [SerializeField] private TextMeshProUGUI traderGoldText;
    [SerializeField] private GameObject traderItemSlotsParent;
    List<InventorySlotUI> traderItemSlots = new List<InventorySlotUI>();


    [SerializeField] private TextMeshProUGUI playerGoldText;
    [SerializeField] private TextMeshProUGUI playerWeightText;
    [SerializeField] private GameObject playerItemSlotsParent;
    List<InventorySlotUI> playerItemSlots = new List<InventorySlotUI>();

    [SerializeField] private TextMeshProUGUI markedValueText;
    private int totalValueMarked = 0;
    [SerializeField] private Color markedValueNegativeColor;
    [SerializeField] private Color markedValuePositiveColor;


    [SerializeField] private Color unmarkedColor;
    [SerializeField] private Color markedColor;
    private List<InventorySlotUI> traderMarkedItems = new List<InventorySlotUI>();
    private List<InventorySlotUI> playerMarkedItems = new List<InventorySlotUI>();

    public static TradingManager GetInstance() 
    {
        return instance;
    }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        tradePanel.SetActive(false);
    }

    private void Start() {
        player = Player.GetInstance().GetComponent<ObjectHandler>();
    }

    private void Update() {
        if (InputManager.GetInstance().GetOfferTradePressed()) {
            // TODO: Stop if player not enough coins, if trader has not enough offer to do it anyway
            // TODO: Consider buying and selling power of Player based on attributes
            InventoryHandler traderInventory = trader.GetComponent<InventoryHandler>();
            InventoryHandler playerInventory = player.GetComponent<InventoryHandler>();

            if (totalValueMarked < 0f && playerInventory.coins < Mathf.Abs(totalValueMarked)) {
                AudioManager.instance.PlayClipRandom(AudioID.TradeWithdrawItem);

                // TODO: Popup box saying not enough coins

                return;
            }

            if (totalValueMarked > 0f && traderInventory.coins < Mathf.Abs(totalValueMarked)) {
                AudioManager.instance.PlayClipRandom(AudioID.TradeWithdrawItem);

                // TODO: Popup box saying not enough coins

                return;
            }

            traderInventory.coins = Mathf.Clamp(traderInventory.coins - totalValueMarked, 0, 999999999);
            playerInventory.coins = Mathf.Clamp(playerInventory.coins + totalValueMarked, 0, 999999999);


            foreach(InventorySlotUI slot in traderMarkedItems) {
                // Transfer to player
                if (slot.item.PickUp(player) == true)
                    traderInventory.Remove(slot.item);
            }

            foreach(InventorySlotUI slot in playerMarkedItems) {
                // Transfer to open contianer
                if (slot.item.PickUp(trader) == true)
                    playerInventory.Remove(slot.item);
            }

            ClearMarkedItems();

            totalValueMarked = 0;

            UpdateTradeUI();

            AudioManager.instance.PlayClipRandom(AudioID.TradeComplete);
        }
    }

    public void EnterTrade(ObjectHandler trader) 
    {
        this.trader = trader;

        // Set all UI elements
        traderNameText.text = trader.objectName + "'s inventory";

        totalValueMarked = 0;

        tradePanel.SetActive(true);

        UpdateTradeUI();
    }

    public void ExitTrade() 
    {
        this.trader = null;

        ClearMarkedItems();
        
        // Set all UI elements
        foreach (InventorySlotUI slot in traderItemSlots) {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }

        traderItemSlots.Clear();

        foreach (InventorySlotUI slot in playerItemSlots) {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }

        playerItemSlots.Clear();

        totalValueMarked = 0;


        tradePanel.SetActive(false);
        AudioManager.instance.PlayClipRandom(AudioID.CloseUI);
    }


    void UpdateTradeUI() {
        foreach (InventorySlotUI slot in traderItemSlots) {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }

        traderItemSlots.Clear();

        foreach(ItemHandler item in trader.GetComponent<InventoryHandler>().inventory) {
            GameObject slotGO = ObjectPoolManager.Instance.GetPooledObject(PoolIdentifiers.ItemSlotUI);
            slotGO.transform.SetParent(traderItemSlotsParent.transform, false);

            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
            traderItemSlots.Add(slotUI);

            slotUI.AddItem(item, 2.0f - GetValueModifier(item));
            slotUI.OnClick += OnPointerClick;

            slotGO.SetActive(true);
        }

        foreach (InventorySlotUI slot in playerItemSlots) {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }

        playerItemSlots.Clear();

        foreach(ItemHandler item in player.GetComponent<InventoryHandler>().inventory) {
            GameObject slotGO = ObjectPoolManager.Instance.GetPooledObject(PoolIdentifiers.ItemSlotUI);
            slotGO.transform.SetParent(playerItemSlotsParent.transform, false);

            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
            playerItemSlots.Add(slotUI);

            slotUI.AddItem(item, GetValueModifier(item));
            slotUI.OnClick += OnPointerClick;

            slotGO.SetActive(true);
        }

        markedValueText.text = totalValueMarked.ToString();
        if (totalValueMarked < 0) {
            markedValueText.color = markedValueNegativeColor;
        } else if (totalValueMarked > 0) {
            markedValueText.color = markedValuePositiveColor;
        } else {
            markedValueText.color = Color.white;
        }

        traderGoldText.text  = trader.GetComponent<InventoryHandler>().coins.ToString();

        playerGoldText.text  = player.GetComponent<InventoryHandler>().coins.ToString();
        if (player.GetComponent<InventoryHandler>().coins + totalValueMarked < 0f) {
            playerGoldText.color = markedValueNegativeColor;
        } else {
            playerGoldText.color = Color.white;
        }


        playerWeightText.text = player.GetComponent<InventoryHandler>().carryWeightCurrent.ToString() + " / " + player.GetComponent<InventoryHandler>().carryWeightMax.ToString();
    }

    public void OnPointerClick(InventorySlotUI slot, PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) {
            MarkItem(slot);
        }
        else if (eventData.button == PointerEventData.InputButton.Right) {
            
        }
    }


    void MarkItem(InventorySlotUI slot) {
        if (traderItemSlots.Contains(slot)) {
            if (traderMarkedItems.Contains(slot) == false) {
                traderMarkedItems.Add(slot);
                totalValueMarked -= Mathf.RoundToInt(slot.item.value * (2.0f - GetValueModifier(slot.item)));
                slot.background.color = markedColor;
                AudioManager.instance.PlayClipRandom(AudioID.TradeOfferItem);
            } else {
                traderMarkedItems.Remove(slot);
                totalValueMarked += Mathf.RoundToInt(slot.item.value * (2.0f - GetValueModifier(slot.item)));
                slot.background.color = unmarkedColor;
                AudioManager.instance.PlayClipRandom(AudioID.TradeWithdrawItem);
            }
        } else if (playerItemSlots.Contains(slot)) {
            if (playerMarkedItems.Contains(slot) == false) {
                playerMarkedItems.Add(slot);
                totalValueMarked += Mathf.RoundToInt(slot.item.value * GetValueModifier(slot.item));
                slot.background.color = markedColor;
                AudioManager.instance.PlayClipRandom(AudioID.TradeOfferItem);
            } else {
                playerMarkedItems.Remove(slot);
                totalValueMarked -= Mathf.RoundToInt(slot.item.value * GetValueModifier(slot.item));
                slot.background.color = unmarkedColor;
                AudioManager.instance.PlayClipRandom(AudioID.TradeWithdrawItem);
            }
        }

        markedValueText.text = totalValueMarked.ToString();

        if (totalValueMarked < 0) {
            markedValueText.color = markedValueNegativeColor;
        } else if (totalValueMarked > 0) {
            markedValueText.color = markedValuePositiveColor;
        } else {
            markedValueText.color = Color.white;
        }

        if (player.GetComponent<InventoryHandler>().coins + totalValueMarked < 0f) {
            playerGoldText.color = markedValueNegativeColor;
        } else {
            playerGoldText.color = Color.white;
        }
        if (trader.GetComponent<InventoryHandler>().coins - totalValueMarked < 0f) {
            traderGoldText.color = markedValueNegativeColor;
        } else {
            traderGoldText.color = Color.white;
        }

    }

    void ClearMarkedItems() {
        foreach(InventorySlotUI slot in traderItemSlots) {
            traderMarkedItems.Remove(slot);
            slot.background.color = unmarkedColor;
        }
        foreach(InventorySlotUI slot in playerItemSlots) {
            playerMarkedItems.Remove(slot);
            slot.background.color = unmarkedColor;
        }

        totalValueMarked = 0;
        markedValueText.text = totalValueMarked.ToString();
    }


    float GetValueModifier(ItemHandler item) {
        float modifier = 0.2f + (0.1f * Player.instance.character.Attributes.GetAttribute(AttributeNames.Ego)); // Add something with reputation

        modifier = Mathf.Clamp(modifier, 0f, 0.9f);

        return modifier;
    }
}
