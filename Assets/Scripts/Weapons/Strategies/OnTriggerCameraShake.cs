using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerCameraShake : BaseStrategy
{
    // Start is called before the first frame update
    public override void Create() {
        base.Create();
        
        weapon.triggerHolders[triggerSlot].OnTrigger += CameraShake;
    }

    void CameraShake(float charge) {
        GameManager.instance.ShakeCamera(3.0f, 0.15f);
    }
}
