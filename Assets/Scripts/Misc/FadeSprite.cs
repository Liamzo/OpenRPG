using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FadeSprite : MonoBehaviour
{
    public BoxCollider2D fadeCollider;  // Collider area for detecting player
    private float fadeDuration = 0.5f;     // Time taken to fade in/out
    private List<SpriteRenderer> spriteRenderers;
    private bool isFading = false;
    private bool isFaded = false;
    

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is within the fadeCollider
        if (fadeCollider.bounds.Contains(Player.instance.transform.position))
        {
            if (!isFading && !isFaded)
                StartCoroutine(FadeOut());
        }
        else
        {
            if (!isFading && isFaded)
                StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeOut()
    {
        isFading = true;
        float elapsedTime = 0f;

        // Gradually fade out the section of the texture
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // Modify the alpha of pixels within the collider bounds
            FadeSection(fadeCollider.bounds, 1 - (elapsedTime / fadeDuration));
            yield return null;
        }

        FadeSection(fadeCollider.bounds, 0f); // Ensure full fade out at the end
        isFading = false;
        isFaded = true;
    }

    IEnumerator FadeIn()
    {
        isFading = true;
        float elapsedTime = 0f;

        // Gradually fade in the section of the texture
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // Restore the alpha of pixels within the collider bounds
            FadeSection(fadeCollider.bounds, elapsedTime / fadeDuration);
            yield return null;
        }

        FadeSection(fadeCollider.bounds, 1f); // Ensure full fade in at the end
        isFading = false;
        isFaded = false;
    }

    // Modify the alpha of pixels within a specified section of the texture
    void FadeSection(Bounds bounds, float alpha)
    {
        // Convert world space bounds to texture pixel space
        Vector4 pixelRect = WorldBoundsToPixelRect(bounds);

        foreach(SpriteRenderer spriteRenderer in spriteRenderers) {
            spriteRenderer.material.SetFloat("_Alpha", alpha);
            spriteRenderer.material.SetVector("_Bounds", pixelRect);
        }

    }

    // Convert world space bounds to pixel space rect in the texture
    Vector4 WorldBoundsToPixelRect(Bounds worldBounds)
    {
        Vector2 min = worldBounds.min;
        Vector2 max = worldBounds.max;

        return new Vector4(min.x, min.y, max.x, max.y);
    }
}
