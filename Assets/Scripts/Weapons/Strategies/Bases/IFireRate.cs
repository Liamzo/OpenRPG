using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFireRate {
    public bool CanFire();
    public void FiredShot();
}