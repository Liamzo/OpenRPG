using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Mod", menuName = "Mods/Weapon Mod")]
public class WeaponMod : ScriptableObject
{
    public string modId;
    public string modName;
    public WeaponModSlot modSlot;
    public List<BaseStrategy> strategies;
    public List<WeaponModBonus> weaponModBonuses;

    protected WeaponHandler weapon;

    public virtual void Create(WeaponHandler weapon)
    {
        this.weapon = weapon;

        List<BaseStrategy> instantiatedStrategies = new List<BaseStrategy>();

        foreach (BaseStrategy startingStrategy in strategies) {
            BaseStrategy strategy = Instantiate(startingStrategy);

            strategy.Create(weapon);

            instantiatedStrategies.Add(strategy);
        }

        strategies = instantiatedStrategies; // Swap the prefab strategies with new versions

        foreach (WeaponModBonus weaponModBonus in weaponModBonuses)
        {
            weapon.statsWeapon[weaponModBonus.weaponStatName].AddModifier(new Modifier(weaponModBonus.modifierType, weaponModBonus.value));
        }
    }
}

public enum WeaponModSlot {
    PistolTrigger,
    PistolBarrel,
    PistolMagazine,
    PistolInternals,
    PistolAmmo,
    PistolAlternate,
    PistolExtra,
    SwordHandle,
    SwordBlade,
    SwordCross,
    SwordExtra
}

[System.Serializable]
public class WeaponModBonus {
    public WeaponStatNames weaponStatName;
    public ModifierTypes modifierType;
    public float value;
}