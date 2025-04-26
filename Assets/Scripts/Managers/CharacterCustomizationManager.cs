using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;


public class CharacterCustomizationManager : MonoBehaviour
{
    public static CharacterCustomizationManager Instance { get; private set; }
    public List<SpriteLibraryHolder> allSpriteParts;


    void Awake() {
        Instance = this;
    }


    public SpriteLibraryAsset FindSpriteLibraryById(string spritePartId) {
        foreach (SpriteLibraryHolder spritePartHolder in allSpriteParts)
        {
            if (spritePartHolder.spritePartId == spritePartId) {
                return spritePartHolder.spriteLibrary;
            }
        }

        Debug.LogWarning("No sprite library found with that ID: " + spritePartId);
        return null;
    }
    public string FindIdBySpriteLibrary(SpriteLibraryAsset spriteLibrary) {
        foreach (SpriteLibraryHolder spritePartHolder in allSpriteParts)
        {
            if (spritePartHolder.spriteLibrary == spriteLibrary) {
                return spritePartHolder.spritePartId;
            }
        }

        Debug.LogWarning("No sprite library found");
        return null;
    }
}


[System.Serializable]
public struct SpriteLibraryHolder {
    public string spritePartId;
    public SpriteParts spritePart;
    public SpriteLibraryAsset spriteLibrary;
}

public enum SpriteParts {
	Base,
    Hair,
    Body
}
