using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionManager : MonoBehaviour
{
    public static FactionManager instance;

    [SerializeField] private List<FactionSO> allFactionInfo;

    public List<Faction> AllFactions {get; private set;}


    private void Awake() {
        instance = this;

        // Set up factions from factionSOs
    }


    public Faction FindFaction(string factionName) {
        foreach (Faction faction in AllFactions) {
            if (faction.factionName == factionName) {
                return faction;
            }
        }

        Debug.LogWarning($"No faction found with name: {factionName}");
        return null;
    }
}

// Used for both personal and faction reputation
[System.Serializable]
public struct Reputation {
    public Faction faction;
    public float reputation; // < -100 = enemy, > -100 + < 100 = neutral, > 100 = ally
}
