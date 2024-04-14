using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionHandler : MonoBehaviour
{

    // TODO: All of this will need redone properly. Not a bad start, just done quickly to get things working

    public List<FactionSO> joinedFactions = new List<FactionSO>();
    //public List<Reputation> personalReputations = new List<Reputation>(); // What this thinks of the factions


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float GetPersonalRepuation(FactionHandler otherFactionHandler) {
        float personal = 0f;

        // foreach (Reputation reputation in personalReputations) {
        //     if (reputation.factionName == factionToFind.factionName) {
        //         personal = reputation.reputation;
        //         break;
        //     }
        // }

        return personal;
    }

    public float GetFactionRepuation(FactionHandler otherFactionHandler) {
        float joinedFactionRep = 0f;

        foreach (FactionSO faction in joinedFactions)
        {
            foreach (FactionSO otherFaction in otherFactionHandler.joinedFactions)
            {
                joinedFactionRep += FactionManager.instance.FindRepuation(faction, otherFaction);
            }
        }

        return joinedFactionRep;
    }
    public float FindReputation(FactionHandler otherFactionHandler) {
        float personalRep = GetPersonalRepuation(otherFactionHandler);
        float factionRep = GetFactionRepuation(otherFactionHandler);

        return personalRep + factionRep;
    }



}
