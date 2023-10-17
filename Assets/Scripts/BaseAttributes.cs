using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attribute Block", menuName = "Attributes/New Attribute Block")]
public class BaseAttributes : ScriptableObject {
    public List<AttributeValue> stats;
}