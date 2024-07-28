using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Mod", menuName = "Mods/Weapon Mod")]
public class WeaponMod : ScriptableObject
{
    public string modId;
    public string modName;
    public string modDescription;
    public Sprite modIcon;
    public WeaponModSlot modSlot;
    public List<BaseStrategy> strategies;
    [SerializeField] List<BaseStrategy> startingStrategies;
    public List<WeaponModBonus> weaponModBonuses;

    protected WeaponHandler weapon;

    public virtual void Create(WeaponHandler weapon)
    {
        this.weapon = weapon;

        strategies = new List<BaseStrategy>();

        foreach (BaseStrategy startingStrategy in startingStrategies) {
            BaseStrategy strategy = Instantiate(startingStrategy);

            strategy.Create(weapon);

            strategies.Add(strategy);
        }

        foreach (WeaponModBonus weaponModBonus in weaponModBonuses)
        {
            weapon.statsWeapon[weaponModBonus.weaponStatName].AddModifier(new Modifier(weaponModBonus.modifierType, weaponModBonus.value));
        }
    }

    public virtual void Delete() {
        foreach (BaseStrategy strategy in strategies)
        {
            Destroy(strategy);
        }

        foreach (WeaponModBonus weaponModBonus in weaponModBonuses)
        {
            weapon.statsWeapon[weaponModBonus.weaponStatName].RemoveModifier(new Modifier(weaponModBonus.modifierType, weaponModBonus.value));
        }

        Destroy(this);
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
    public Sprite icon;
}