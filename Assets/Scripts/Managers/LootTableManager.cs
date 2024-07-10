using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTableManager : MonoBehaviour
{
    public static LootTableManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }
}
