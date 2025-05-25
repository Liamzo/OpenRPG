using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExclusionZone
{
    public abstract bool CheckInZone(Vector2 position, float distance = 0f);
}
