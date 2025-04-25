using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;


public class SpritePartAnimator : MonoBehaviour
{
    public SpriteParts spritePart;

    public SpriteRenderer spriteRenderer;
    public SpriteRenderer baseSpriteRenderer;
    public SpriteResolver myResolver;
    public SpriteResolver baseResolver;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        myResolver = GetComponent<SpriteResolver>();
        baseResolver = transform.parent.GetComponent<SpriteResolver>();
        baseSpriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        myResolver.SetCategoryAndLabel(baseResolver.GetCategory(), baseResolver.GetLabel());
        spriteRenderer.flipX = baseSpriteRenderer.flipX;
    }
}


public enum SpriteParts {
	Base,
    Hair
}
