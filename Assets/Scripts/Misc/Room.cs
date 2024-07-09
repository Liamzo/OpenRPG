using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Room : MonoBehaviour
{
    public List<SpriteRenderer> spritesToHide;

    float switchingTimer = 1.0f;
    float startingAlpha = 1.0f;
    float targetAlpha = 1.0f;


    public ShadowCaster2D shadowCaster2Dext;
    public ShadowCaster2D shadowCaster2Dint;

    private void Update() {
        if (spritesToHide.Count == 0) { return ; }

        Color targetColour = spritesToHide[0].color;

        if (targetAlpha != targetColour.a) {
            if (startingAlpha > targetAlpha) {
                switchingTimer -= Time.deltaTime * 3f;

                if (switchingTimer < 0.0f)
                    switchingTimer = 0.0f;

                targetColour.a = Mathf.Lerp(startingAlpha, targetAlpha, 1 - switchingTimer);
            } else {
                switchingTimer += Time.deltaTime * 4f;

                if (switchingTimer > 1.0f)
                    switchingTimer = 1.0f;

                targetColour.a = Mathf.Lerp(startingAlpha, targetAlpha, switchingTimer);
            }

            foreach (SpriteRenderer spriteToHide in spritesToHide) {
                spriteToHide.color = targetColour;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.tag == "Player") {
            targetAlpha = 0.0f;
            startingAlpha = 1.0f;
            
            if (shadowCaster2Dext != null)
                shadowCaster2Dext.enabled = false;

            if (shadowCaster2Dint != null)
                shadowCaster2Dint.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.transform.tag == "Player") {
            targetAlpha = 1.0f;
            startingAlpha = 0.0f;
            
            if (shadowCaster2Dext != null)
                shadowCaster2Dext.enabled = true;

            if (shadowCaster2Dint != null)
                shadowCaster2Dint.enabled = false;
        }
    }
}
