using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwing : MonoBehaviour
{
    float timer = 0f;
    SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        timer += Time.deltaTime;

        if (timer >= 0.33f) {
            timer = 0f;
            gameObject.SetActive(false);
            transform.SetParent(null);
        }
    }

    public void SetSprite(Sprite sprite) {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = sprite;
    }
}

public enum SwingDir {
    RtL,
    LtR
}