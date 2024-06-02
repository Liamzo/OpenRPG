using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

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

            foreach(InventorySlotUI slot in traderMarkedItems) {
                // Money
                traderInventory.coins += slot.item.value;
                playerInventory.coins -= slot.item.value;

                // Transfer to player
                if (slot.item.PickUp(player) == true)
                    traderInventory.Remove(slot.item);
            }

            foreach(InventorySlotUI slot in playerMarkedItems) {
                // Money
                traderInventory.coins -= slot.item.value;
                playerInventory.coins += slot.item.value;

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
            GameObject slotGO = ObjectPoolManager.instance.GetPooledObject(PoolIdentifiers.ItemSlotUI);
            slotGO.transform.SetParent(traderItemSlotsParent.transform, false);

            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
            traderItemSlots.Add(slotUI);

            slotUI.AddItem(item);
            slotUI.OnClick += OnPointerClick;

            slotGO.SetActive(true);
        }

        foreach (InventorySlotUI slot in playerItemSlots) {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }

        playerItemSlots.Clear();

        foreach(ItemHandler item in player.GetComponent<InventoryHandler>().inventory) {
            GameObject slotGO = ObjectPoolManager.instance.GetPooledObject(PoolIdentifiers.ItemSlotUI);
            slotGO.transform.SetParent(playerItemSlotsParent.transform, false);

            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
            playerItemSlots.Add(slotUI);

            slotUI.AddItem(item);
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
                totalValueMarked -= slot.item.value;
                slot.background.color = markedColor;
                AudioManager.instance.PlayClipRandom(AudioID.TradeOfferItem);
            } else {
                traderMarkedItems.Remove(slot);
                totalValueMarked += slot.item.value;
                slot.background.color = unmarkedColor;
                AudioManager.instance.PlayClipRandom(AudioID.TradeWithdrawItem);
            }
        } else if (playerItemSlots.Contains(slot)) {
            if (playerMarkedItems.Contains(slot) == false) {
                playerMarkedItems.Add(slot);
                totalValueMarked += slot.item.value;
                slot.background.color = markedColor;
                AudioManager.instance.PlayClipRandom(AudioID.TradeOfferItem);
            } else {
                playerMarkedItems.Remove(slot);
                totalValueMarked -= slot.item.value;
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
}
