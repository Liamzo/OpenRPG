using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stat Block", menuName = "Stats/New Character Stat Block")]
public class BaseCharacterStats : ScriptableObject {
        public List<CharacterStatValue> stats;
}