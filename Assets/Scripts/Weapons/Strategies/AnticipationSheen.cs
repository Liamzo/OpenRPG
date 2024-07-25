using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Anticipation Sheen", menuName = "Strategies/Anticipation Sheen")]
public class AnticipationSheen : IAnticipation
{
    public override void AttackAnticipation() {
        weapon.StartCoroutine(DoSheen());
    }

    public IEnumerator DoSheen() {
        float timer = 0f;
        while (timer <= 1f) {
            timer += Time.deltaTime * 5f; // 0.2 secs
            weapon.item.objectHandler.spriteRenderer.material.SetFloat("_Sheen", Mathf.Lerp(0f, -2f, timer));
            yield return null;
        }

        weapon.item.objectHandler.spriteRenderer.material.SetFloat("_Sheen", 0f);
    }
}
