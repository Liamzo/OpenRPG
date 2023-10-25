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



    protected override void Awake()
    {
        base.Awake();

        instance = this;

        healthBar.maxValue = statsObject[ObjectStatNames.Health].GetValue();
        healthBar.value = currentHealth;
        staminaBar.maxValue = currentStamina = statsCharacter[CharacterStatNames.Stamina].GetValue();
        staminaBar.value = currentStamina;

        healthText.text = currentHealth + " / " + currentHealth;
        staminaText.text = currentStamina + " / " + currentStamina;
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
