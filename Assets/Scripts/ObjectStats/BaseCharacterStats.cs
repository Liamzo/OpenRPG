using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;


[System.Serializable]
public class BaseCharacterStats : StatBlock {

	public List<BodyPartHolder> spriteParts;

	public List<CharacterStatValue> stats;

	public List<AttributeValue> attributes;

	[Header("Dialogue")]
	public Sprite profileSprite;
	public TextAsset dialogue;



	public BaseCharacterStats() {
		stats = new List<CharacterStatValue>();

		foreach (CharacterStatNames characterStatName in System.Enum.GetValues(typeof(CharacterStatNames)))
		{
		stats.Add(new CharacterStatValue(characterStatName, 0));
		}

		attributes = new List<AttributeValue>();

		foreach (AttributeNames attributeName in System.Enum.GetValues(typeof(AttributeNames)))
		{
		attributes.Add(new AttributeValue(attributeName, 0));
		}
	}
}


[System.Serializable]
public struct BodyPartHolder {
	public SpriteParts spritePart;
	public List<SpriteLibraryAsset> spriteParts;
}