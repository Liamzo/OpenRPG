using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionManager : MonoBehaviour
{
    public static FactionManager instance;

    [SerializeField] private List<FactionSO> allFactionInfo;

    [SerializeField] private List<Reputation> allReputations;


    private void Awake() {
        instance = this;

        // Set up factions from factionSOs
    }


    public FactionSO FindFaction(string factionName) {
        foreach (FactionSO faction in allFactionInfo) {
            if (faction.factionName == factionName) {
                return faction;
            }
        }

        Debug.LogWarning($"No faction found with name: {factionName}");
        return null;
    }


    public float FindRepuation(FactionSO factionOne, FactionSO factionTwo) {
        foreach (Reputation reputation in allReputations)
        {
            if ((reputation.factionOne == factionOne || reputation.factionOne == factionTwo) && (reputation.factionTwo == factionOne || reputation.factionTwo == factionTwo))
            {
                return reputation.reputation;
            }
        }

        // No repution found between these 2 factions
        // Think about how to handle - maybe just create a new one
        return 0;
    }
}

// Used for both personal and faction reputation
[System.Serializable]
public struct Reputation {
    public FactionSO factionOne;
    public FactionSO factionTwo;
    public float reputation; // < -100 = enemy, > -100 + < 100 = neutral, > 100 = ally
}

[System.Serializable]
public enum Feelings {
    Stamina,
    Sight,
    MovementSpeed,
    AttackSpeed

}