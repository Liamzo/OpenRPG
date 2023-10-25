using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReload {
    public void ReloadUpdate();
    public void Update();
    public float ReloadPercentage();
}