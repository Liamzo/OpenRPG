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

        transform.root.GetComponent<EquipmentHandler>().onEquipmentChanged += OnEquipmentChanged; // Doesn't trigger when equipping Starting Equipment
        Debug.Log(transform.root.GetComponent<EquipmentHandler>().currentEquipment[(int)EquipmentSlot.RightHand]); // Does find the starting equipment, so will need to do a quick check here for libraries
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (spriteLibrary.spriteLibraryAsset == null) return;

        resolver.SetCategoryAndLabel(baseResolver.GetCategory(), baseResolver.GetLabel());
        spriteRenderer.flipX = baseSpriteRenderer.flipX;
    }


    void OnEquipmentChanged(ItemHandler newItem, ItemHandler oldItem, EquipmentSlot equipmentSlot) {
        Debug.Log("new equipment");
    }
}