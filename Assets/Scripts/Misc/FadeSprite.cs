using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeSprite : MonoBehaviour
{
    // public BoxCollider2D fadeCollider;  // Collider area for detecting player
    // public float fadeDuration = 0.2f;     // Time taken to fade in/out
    // private List<SpriteRenderer> spritesToHide;
    // private List<Texture2D> textures;
    // private List<Color[]> originalPixels;
    // private bool isFading = false;

    public BoxCollider2D fadeCollider;  // Collider area for detecting player
    public float fadeDuration = 0.5f;     // Time taken to fade in/out
    private SpriteRenderer spriteRenderer;
    private Texture2D texture;
    private Color[] originalPixels;
    private bool isFading = false;
    private bool isFaded = false;
    

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Copy the texture of the sprite
        texture = spriteRenderer.sprite.texture;
        originalPixels = texture.GetPixels(); // Store the original texture colors
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
        Rect pixelRect = WorldBoundsToPixelRect(bounds);

        for (int y = (int)pixelRect.yMin; y < (int)pixelRect.yMax; y++)
        {
            for (int x = (int)pixelRect.xMin; x < (int)pixelRect.xMax; x++)
            {
                // Modify the alpha of the pixels in the specified region
                Color pixelColor = texture.GetPixel(x, y);
                pixelColor.a = Mathf.Lerp(Mathf.Min(originalPixels[x + y * texture.width].a, 0.4f), originalPixels[x + y * texture.width].a, alpha);
                texture.SetPixel(x, y, pixelColor);
            }
        }

        // Apply the changes to the texture
        texture.Apply();
    }

    // Convert world space bounds to pixel space rect in the texture
    Rect WorldBoundsToPixelRect(Bounds worldBounds)
    {
        Vector2 min = WorldToTextureCoord(worldBounds.min);
        Vector2 max = WorldToTextureCoord(worldBounds.max);

        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }

    // Convert world position to texture pixel coordinates
    Vector2 WorldToTextureCoord(Vector3 worldPos)
    {
        // Vector2 localPos = transform.InverseTransformPoint(worldPos);
        // float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;

        // return new Vector2(localPos.x * pixelsPerUnit, localPos.y * pixelsPerUnit);

        // Get the local position of the world point relative to the object's origin
        Vector2 localPos = transform.InverseTransformPoint(worldPos);

        // Get the sprite's rect in the texture
        Rect spriteRect = spriteRenderer.sprite.rect;

        // Get the pivot point of the sprite (normalized from 0 to 1)
        Vector2 pivot = spriteRenderer.sprite.pivot;

        // Convert local position to pixel space, taking into account the sprite's pivot
        float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;
        Vector2 pixelPos = localPos * pixelsPerUnit;

        // Adjust for the sprite pivot (so the coordinates align with the texture correctly)
        pixelPos += pivot;

        // Clip the coordinates to the sprite's rect
        pixelPos.x = Mathf.Clamp(pixelPos.x, 0, spriteRect.width);
        pixelPos.y = Mathf.Clamp(pixelPos.y, 0, spriteRect.height);

        return pixelPos;
    }
}
