using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionDodge : BaseReaction
{
    public float coolDown = 5f;
    bool onCoolDown = false;
    public float meleeChance = 66f;
    public float rangedChance = 33f;

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

        Vector3 targetPosition = projectile == null ? weapon.item.owner.transform.position : projectile.transform.position;

        float attackDistance = (targetPosition - brain.transform.position).magnitude;
        if (projectile == null && attackDistance > weapon.GetStatValue(WeaponStatNames.Range)) return 0f;

        Vector3 targetDirection = (brain.transform.position - targetPosition).normalized;
        Vector3 attackDirection = projectile == null ? weapon.item.owner.GetComponent<BaseBrain>().targetLookingDirection : projectile.GetComponent<BasicBullet>().direction;
        float attackAngle = Vector2.Angle(targetDirection, attackDirection);

        float blockAngle = projectile == null ? 90f : 30f;

        if (attackAngle <= blockAngle) {
            value += 90f;
        }
        
        return value;
    }

    public override void Execute()
    {
        brain.wasDodging = true;

        Vector3 targetPosition = projectile == null ? attackingWeapon.item.owner.transform.position : projectile.transform.position;
        Vector3 dirFromTarget = (brain.transform.position - targetPosition).normalized;
        float dodgeAngle = Random.Range(-110f, 110f);
        Vector3 dodgeDirection = Quaternion.AngleAxis(dodgeAngle, Vector3.forward) * dirFromTarget;

        brain.dodgeMovement = dodgeDirection;

        if (brain.dodgeMovement.x < 0) {
            brain.character.spriteRenderer.flipX = true;
        } else if (brain.dodgeMovement.x > 0) {
            brain.character.spriteRenderer.flipX = false;
        }

        brain.dodgeMovement *= brain.character.statsCharacter[CharacterStatNames.MovementSpeed].BaseValue * brain.dodgeSpeedMulti;

        brain._animator.SetBool("Rolling", true);
        brain._animator.SetTrigger("Roll");

        brain.character.objectStatusHandler.BlockMovementControls();
        brain.character.objectStatusHandler.isDodging = true;
        brain.character.ChangeStamina(-brain.dodgeStaminaCost);
        brain.character.objectStatusHandler.BlockRegainStamina(2f);

        brain.footSteps.Emit(25);

        AudioManager.instance.PlayClipRandom(AudioID.Roll, brain.character.audioSource);

        StartCoroutine(CoolDown());
        brain.thoughtLocked = null;
    }


    public IEnumerator CoolDown() {
        onCoolDown = true;

        yield return new WaitForSeconds(coolDown);

        onCoolDown = false;
    }
}
