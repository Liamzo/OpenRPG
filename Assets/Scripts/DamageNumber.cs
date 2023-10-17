using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    float timer = 0f;
    private void Update() {
        timer += Time.deltaTime;

        if (timer >= 1.0f) {
            timer = 0f;
            transform.parent.gameObject.SetActive(false);
            transform.parent.SetParent(null);
        }
    }
}
