using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCondition
{
    public abstract void Start();
    public abstract void Tick();
    public abstract void End();
}
