using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReactionBlock : BaseReaction
{
    public float coolDown = 5f;
    bool onCoolDown = false;
    public float meleeChance = 66f;
    public float rangedChance = 33f;
    public bool canParry;

    bool blocking = false;

    WeaponHandler attackingWeapon;
    GameObject projectile;


    public override float Evaluate(WeaponHandler weapon, GameObject projectile)
    {
        if (onCoolDown) return 0f;

        float value = 0f;

        float chance = projectile == null ? meleeChance : rangedChance;

        float rand = Random.Range(0f, 99f);

        if (rand > chance) return 0f;

        
        this.attackingWeapon = weapon;
        this.projectile = projectile;


        Vector3 targetPosition = projectile == null ? attackingWeapon.item.owner.transform.position : projectile.transform.position;

        float attackDistance = (targetPosition - brain.transform.position).magnitude;
        if (projectile == null && attackDistance > attackingWeapon.GetStatValue(WeaponStatNames.Range)) return 0f;

        Vector3 targetDirection = (brain.transform.position - targetPosition).normalized;
        Vector3 attackDirection = projectile == null ? attackingWeapon.item.owner.GetComponent<BaseBrain>().targetLookingDirection : projectile.GetComponent<BasicBullet>().direction;
        float attackAngle = Vector2.Angle(targetDirection, attackDirection);

        float blockAngle = projectile == null ? 90f : 30f;

        if (attackAngle <= blockAngle) {
            value += 100f;
        }
        

        return value;
    }

    public override void Execute()
    {
        if (blocking) return;

        Vector3 lookingDirection = projectile == null ? brain.threatHandler.TargetLastSeen.Value : projectile.transform.position;
        brain.SetTargetLookingDirection(lookingDirection);

        StartCoroutine(HoldBlock(1f));
    }

    IEnumerator HoldBlock(float duration) {
        blocking = true;
        brain.equipmentHandler.rightMeleeSpot.weapon.Unholster(); // Temp, do better
        brain.equipmentHandler.ToggleMeleeRanged(true);
        WeaponHandler weapon = brain.equipmentHandler.rightMeleeSpot.weapon;
        bool defaultCanParry = weapon.item.owner.GetComponent<CharacterHandler>().canParry;
        weapon.item.owner.GetComponent<CharacterHandler>().canParry = canParry;

        weapon.AttackHold(1);

        yield return new WaitForSeconds(duration);

        blocking = false;
        brain.thoughtLocked = null;
        weapon.item.owner.GetComponent<CharacterHandler>().canParry = defaultCanParry;
        
        weapon.AttackRelease(1);

        StartCoroutine(CoolDown());
    }

    public IEnumerator CoolDown() {
        onCoolDown = true;

        yield return new WaitForSeconds(coolDown);

        onCoolDown = false;
    }
}
