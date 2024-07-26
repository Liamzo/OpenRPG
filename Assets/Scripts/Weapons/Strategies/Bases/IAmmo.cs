using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAmmo {
    public int GetCurrentAmmo();
    public int GetMaxAmmo();
    public void Reload(int ammout);
    public void UseAmmo();
}