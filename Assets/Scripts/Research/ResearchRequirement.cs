using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public abstract class ResearchRequirement
{
    public int total;
    public int current = 0;

    public abstract string GetProgress();

    public abstract void Begin();
    public abstract void End();
}
