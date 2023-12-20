using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Faction
{
    public string factionName;
    public List<Reputation> reputations = new List<Reputation>();

    public bool IsAlly(Faction faction) {
        foreach (Reputation reputation in reputations) {
            if (reputation.faction == faction) {
                if (reputation.reputation >= 100f) return true;

                return false;
            }
        }

        return false;
    }

    public bool IsEnemy(Faction faction) {
        foreach (Reputation reputation in reputations) {
            if (reputation.faction == faction) {
                if (reputation.reputation <= -100f) return true;

                return false;
            }
        }

        return false;
    }
}
