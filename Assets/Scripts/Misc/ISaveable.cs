using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public interface ISaveable
{
    public string SaveComponent();

    public void LoadComponent(JSONNode data);
}
