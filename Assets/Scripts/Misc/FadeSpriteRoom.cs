using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FadeSpriteRoom : MonoBehaviour
{
    public Collider2D fadeCollider;  // Collider area for detecting player
    private float fadeDuration = 0.5f;     // Time taken to fade in/out
    private List<SpriteRenderer> spriteRenderers;
    private bool isFading = false;
    private bool isFaded = false;
    private float fadeInPauseDuration = 0.5f;
    private float fadeInPauseTimer = 0.5f;

    public List<ObjectHandler> objectsBehind = new();
    List<Collider2D> collidersInsideHidden = new();
    List<SpriteRenderer> spritesInsideHidden = new();



    public Room room;
    

    // Start is called before the first frame update
    void Start()
    {
        fadeCollider = GetComponent<Collider2D>();

        spriteRenderers = transform.parent.GetComponentsInChildren<SpriteRenderer>().ToList();
        

        List<Room> allRooms = transform.parent.GetComponentsInChildren<Room>().ToList();

        Room biggestRoom = allRooms[0];
        float biggestSize = biggestRoom.zoneCollider.bounds.size.magnitude;

        for (int i = 1; i < allRooms.Count; i++) {
            Room testRoom = allRooms[i];

            if (testRoom.zoneCollider.bounds.size.magnitude > biggestSize) {
                biggestRoom = testRoom;
                biggestSize = biggestRoom.zoneCollider.bounds.size.magnitude;
            }
        }

        room = biggestRoom;

        room.OnEnter += ColliderEnteredRoom;
        room.OnExit += ColliderLeftRoom;
    }

    // Update is called once per frame
    void Update()
    {
        bool shouldFadeOut = objectsBehind.Contains(Player.instance.character);

        if (!shouldFadeOut) {
            foreach (ObjectHandler objectHandler in objectsBehind) {
                if (Player.instance.threatHandler.CheckInLineOfSight(objectHandler)) {
                    shouldFadeOut = true;
                    break;
                }
            }
        }

        // Check if the player is within the fadeCollider
        if (shouldFadeOut)
        {
            if (!isFading && !isFaded)
                StartCoroutine(FadeOut());
        }
        else
        {
            if (!isFading && isFaded) {
                fadeInPauseTimer -= Time.deltaTime;

                if (fadeInPauseTimer <= 0f)
                    StartCoroutine(FadeIn());
            }
        }
    }

    IEnumerator FadeOut()
    {
        isFading = true;
        float elapsedTime = 0f;

        List<Collider2D> colliders = new ();

        room.zoneCollider.OverlapCollider(new ContactFilter2D(), colliders);

        collidersInsideHidden.Clear();
        spritesInsideHidden.Clear();

        foreach (Collider2D collider in colliders) {
            if (collider.GetComponent<ObjectHandler>() != null || collider.GetComponent<Thing>() != null) {
                List<SpriteRenderer> sprites = collider.GetComponentsInChildren<SpriteRenderer>().ToList();
                sprites.ForEach(sprite => sprite.enabled = false);
                collidersInsideHidden.Add(collider);
                spritesInsideHidden.AddRange(sprites);
            }
        }
        

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
        fadeInPauseTimer = fadeInPauseDuration;

        foreach (SpriteRenderer spriteRenderer in spritesInsideHidden) {
            spriteRenderer.enabled = true;
        }
        spritesInsideHidden.Clear();
        collidersInsideHidden.Clear();
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

    private void OnTriggerEnter2D(Collider2D other) {
        CharacterHandler otherCharacter;
        if (other.transform.root.TryGetComponent<CharacterHandler>( out otherCharacter)) {
            objectsBehind.Add(otherCharacter);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        CharacterHandler otherCharacter;
        if (other.transform.root.TryGetComponent<CharacterHandler>( out otherCharacter)) {
            objectsBehind.Remove(otherCharacter);
        }
    }

    void ColliderEnteredRoom (Collider2D collider) {
        if (!collidersInsideHidden.Contains(collider) && (isFading || isFaded)) {
            List<SpriteRenderer> sprites = collider.GetComponentsInChildren<SpriteRenderer>().ToList();
            sprites.ForEach(sprite => sprite.enabled = false);
            spritesInsideHidden.AddRange(sprites);
            collidersInsideHidden.Add(collider);
        }
    }
    void ColliderLeftRoom (Collider2D collider) {
        if (collidersInsideHidden.Contains(collider)) {
            List<SpriteRenderer> sprites = collider.GetComponentsInChildren<SpriteRenderer>().ToList();
            sprites.ForEach(sprite => sprite.enabled = true);
            sprites.ForEach(sprite => spritesInsideHidden.Remove(sprite));
            collidersInsideHidden.Remove(collider);
        }
    }
}
