using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Research Option", menuName = "Research Option")]
public class ResearchOption : ScriptableObject
{
    public string researchId;
    public string researchName;
    public string researchDescription;


    public ResearchCategory researchCategory;
    public ResearchState researchState;
}


public enum ResearchState {
    Unknown,
    Discovered,
    Researched,
    Unlocked
}