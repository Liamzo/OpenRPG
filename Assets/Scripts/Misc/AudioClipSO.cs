using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Audio Clip", menuName = "Audio/Audio Clip", order = 0)]
public class AudioClipSO : ScriptableObject {
    public AudioID audioID;
    public AudioSourceType audioSourceType;

    public List<AudioClip> audioClips;

    public AudioClip GetRandomClip() {
        return audioClips[Random.Range(0, audioClips.Count)];
    }
}

public enum AudioID {
    FootStepDirt,
    GunShot,
    BulletImpact,
    SwordSwing,
    SwordImpact,
    ShotGunFire,
    HeavyBreathing,
    TakeDamage,
    Death,
    Roll,
    WeaponImpact,
    Block,
    Taunt,
    LootBody,
    OpenInventory,
    OpenMap,
    UI_Click,
    TradeOfferItem,
    TradeComplete,
    OpenLocker,
    PickUp,
    Equip,
    Unequip,
    Drop,
    Travel,
    Hover,
    CloseUI,
    TradeWithdrawItem,
    Dialogue01,
    AmbientWind,
    AmbientBird,
    MusicTown,
    MusicForest,
    PanelChange,
}

public enum AudioSourceType {
    Local,
    Music,
    Ambient,
    Global
}