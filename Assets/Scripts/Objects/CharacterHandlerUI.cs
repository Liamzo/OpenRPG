using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterHandlerUI : CharacterHandler
{
    public static CharacterHandlerUI instance;

    public Slider healthBar;
    public Slider staminaBar;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI staminaText;


    protected override void Setup(BaseStats baseStats) {
        base.Setup(baseStats);

        instance = this;

        GameObject playerStatsUI = GameObject.FindWithTag("PlayerStatsUI");

        healthBar = playerStatsUI.transform.Find("Health").GetComponent<Slider>();
        healthText = playerStatsUI.transform.Find("Health").Find("Fill Area").Find("HealthText").GetComponent<TextMeshProUGUI>();
        staminaBar = playerStatsUI.transform.Find("Stamina").GetComponent<Slider>();
        staminaText = playerStatsUI.transform.Find("Stamina").Find("Fill Area").Find("StaminaText").GetComponent<TextMeshProUGUI>();

        healthBar.maxValue = statsObject[ObjectStatNames.Health].GetValue();
        healthBar.value = currentHealth;
        staminaBar.maxValue = currentStamina = statsCharacter[CharacterStatNames.Stamina].GetValue();
        staminaBar.value = currentStamina;

        healthText.text = currentHealth + " / " + currentHealth;
        staminaText.text = currentStamina + " / " + currentStamina;


        statsObject[ObjectStatNames.Health].OnChange += UpdateHealthUI;

        if (statsObject[ObjectStatNames.Health].BaseAttribute != null)
            Attributes.attributes[statsObject[ObjectStatNames.Health].BaseAttribute.Value.attributeName].OnChange += UpdateHealthUI;

        UpdateHealthUI();
    }


    public override void Heal(float amount)
    {
        base.Heal(amount);

        UpdateHealthUI();
    }


    public override void TakeDamge (float damage, WeaponHandler weapon, CharacterHandler damageDealer) {
        base.TakeDamge(damage, weapon, damageDealer);

        UpdateHealthUI();
    }

    void UpdateHealthUI() {
        healthBar.maxValue = statsObject[ObjectStatNames.Health].GetValue();
        healthBar.value = currentHealth;
        healthText.text = healthBar.value + " / " + healthBar.maxValue;
    }


    public override void ChangeStamina(float changeAmount)
    {
        base.ChangeStamina(changeAmount);

        UpdateStaminaUI();
    }

    void UpdateStaminaUI() {
        staminaBar.maxValue = statsCharacter[CharacterStatNames.Stamina].GetValue();
        staminaBar.value = currentStamina;
        staminaText.text = staminaBar.value + " / " + staminaBar.maxValue;
    }
}
