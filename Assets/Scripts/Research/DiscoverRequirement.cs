using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public abstract class DiscoverRequirement : ResearchRequirement
{
    public event System.Action OnDiscover = delegate { };

    protected void CallOnDiscover() {
        OnDiscover();
    }
}
