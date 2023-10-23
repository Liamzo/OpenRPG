using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Building : MonoBehaviour
{
    public SpriteRenderer spriteExt;
    public SpriteRenderer spriteTop;

    float switchingTimer = 1.0f;
    float startingAlpha = 1.0f;
    float targetAlpha = 1.0f;


    public ShadowCaster2D shadowCaster2D;

    private void Update() {
        if (targetAlpha != spriteExt.color.a) {
            Color newColorAlpha = spriteExt.color;

            if (startingAlpha > targetAlpha) {
                switchingTimer -= Time.deltaTime * 3f;

                if (switchingTimer < 0.0f)
                    switchingTimer = 0.0f;

                newColorAlpha.a = Mathf.Lerp(startingAlpha, targetAlpha, 1 - switchingTimer);
            } else {
                switchingTimer += Time.deltaTime * 4f;

                if (switchingTimer > 1.0f)
                    switchingTimer = 1.0f;

                newColorAlpha.a = Mathf.Lerp(startingAlpha, targetAlpha, switchingTimer);
            }

            spriteExt.color = newColorAlpha;
            spriteTop.color = newColorAlpha;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.tag == "Player") {
            targetAlpha = 0.0f;
            startingAlpha = 1.0f;
            shadowCaster2D.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.transform.tag == "Player") {
            targetAlpha = 1.0f;
            startingAlpha = 0.0f;
            shadowCaster2D.enabled = false;
        }
    }
}
