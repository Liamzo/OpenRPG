using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchUnlockMod : ResearchUnlock
{
    public string modId;


    public override void Unlock() {
        ModManager.Instance.UnlockMod(modId);
    }
}
