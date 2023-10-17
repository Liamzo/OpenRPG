using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MuzzleFlash : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Light2D flashLight;

    bool flash = false;
    public List<serializableClass> muzzleFlashAnims;
    List<Sprite> muzzleFlashFrames;
    public float fps = 24;
    float frameTimer = 0f;
    int frame = 0;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (flash) {
            frameTimer += Time.deltaTime;

            if (frameTimer >= 1f / fps) {
                // Next frame
                frame += 1;

                if (frame >= muzzleFlashFrames.Count) {
                    // Flash complete
                    flash = false;
                    spriteRenderer.enabled = false;
                    flashLight.enabled = false;
                } else {
                    spriteRenderer.sprite = muzzleFlashFrames[frame];
                    frameTimer -= 1f / fps;
                }
            }
        }
    }

    public void Flash() {
        flash = true;
        frame = 0;
        frameTimer = 0f;

        // Select random animation
        muzzleFlashFrames = muzzleFlashAnims[Random.Range(0, muzzleFlashAnims.Count-1)].sampleList;

        spriteRenderer.enabled = true;
        spriteRenderer.sprite = muzzleFlashFrames[0];

        flashLight.enabled = true;
    }

    [System.Serializable]
    public class serializableClass
    {
        public List<Sprite> sampleList;
    }
}
