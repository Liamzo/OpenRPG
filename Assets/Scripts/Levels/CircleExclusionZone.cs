using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleExclusionZone : ExclusionZone
{
    public Vector2 position;
    public float radius;

    public CircleExclusionZone(Vector2 position, float radius) {
        this.position = position;
        this.radius = radius;
    }

    public override bool CheckInZone(Vector2 testPosition, float distance = 0) {
        float minDistance = radius + distance;

        return Vector2.Distance(position, testPosition) < minDistance;
    }
}
