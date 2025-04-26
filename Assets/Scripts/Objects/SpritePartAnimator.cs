using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;


public class SpritePartAnimator : MonoBehaviour
{
    public SpriteParts spritePart;

    SpriteLibrary spriteLibrary;

    public SpriteRenderer spriteRenderer;
    public SpriteRenderer baseSpriteRenderer;
    public SpriteResolver resolver;
    public SpriteResolver baseResolver;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteLibrary = GetComponent<SpriteLibrary>();
        resolver = GetComponent<SpriteResolver>();
        baseResolver = transform.parent.GetComponent<SpriteResolver>();
        baseSpriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
    }

    void Start() {
        spriteLibrary.spriteLibraryAsset = transform.root.GetComponent<CharacterHandler>().baseSpriteParts[spritePart];

        EquipmentHandler equipmentHandler = transform.root.GetComponent<EquipmentHandler>();

        for (int i = 0; i < equipmentHandler.currentEquipment.Length; i++) {
            if (equipmentHandler.currentEquipment[i] != null) {
                if (equipmentHandler.currentEquipment[i].baseItemStats.spritePart == spritePart) {
                    spriteLibrary.spriteLibraryAsset = equipmentHandler.currentEquipment[i].baseItemStats.spriteLibrary;
                    break;
                }
            }
        }

        equipmentHandler.onEquipmentChanged += OnEquipmentChanged;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (spriteLibrary.spriteLibraryAsset == null) {
            spriteRenderer.sprite = null;
            return;
        }

        resolver.SetCategoryAndLabel(baseResolver.GetCategory(), baseResolver.GetLabel());
        spriteRenderer.flipX = baseSpriteRenderer.flipX;
    }


    void OnEquipmentChanged(ItemHandler newItem, ItemHandler oldItem, EquipmentSlot equipmentSlot) {
        if (oldItem != null) {
            if (oldItem.baseItemStats.spritePart == spritePart) {
                spriteLibrary.spriteLibraryAsset = transform.root.GetComponent<CharacterHandler>().baseSpriteParts[spritePart];
            }
        }
        
        if (newItem != null) {
            if (newItem.baseItemStats.spritePart == spritePart) {
                spriteLibrary.spriteLibraryAsset = newItem.baseItemStats.spriteLibrary;
            }
        }

    }
}