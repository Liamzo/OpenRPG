using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseBrain))]
public class TakeDamageSound : MonoBehaviour
{
    BaseBrain brain;


    // Start is called before the first frame update
    void Start()
    {
        brain  = GetComponent<BaseBrain>();

        brain.character.OnTakeDamage += OnTakeDamage;
    }

    // Update is called once per frame
    void OnTakeDamage(float damage, WeaponHandler weapon, CharacterHandler damageDealer, BasicBullet projectile)
    {
        if (brain.character.currentHealth < brain.character.statsObject[ObjectStatNames.Health].GetValue() / 2f && brain.character.currentHealth + damage >= brain.character.statsObject[ObjectStatNames.Health].GetValue() / 2f) {
            // Only trigger when health drops below half
            AudioManager.instance.PlayClipRandom(AudioID.TakeDamage, brain.character.audioSource);
        }

    }
}
