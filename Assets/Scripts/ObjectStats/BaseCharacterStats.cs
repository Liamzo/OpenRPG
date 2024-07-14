using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseCharacterStats : StatBlock {
	public List<CharacterStatValue> stats;

	public List<AttributeValue> attributes;

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