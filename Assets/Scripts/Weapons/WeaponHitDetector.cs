using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitDetector : MonoBehaviour
{
    public event System.Action<Collider2D> TriggerEntered = delegate { };

    private void OnTriggerEnter2D(Collider2D other) {
        TriggerEntered(other);
    }
}
