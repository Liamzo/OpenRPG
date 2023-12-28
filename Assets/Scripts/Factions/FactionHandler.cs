using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionHandler : MonoBehaviour
{
    public List<Faction> joinedFactions = new List<Faction>();
    public List<Reputation> personalReputations = new List<Reputation>(); // What this thinks of the factions


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float GetPersonalRepuation(Faction factionToFind) {
        float personal = 0f;

        foreach (Reputation reputation in personalReputations) {
            if (reputation.factionName == factionToFind.factionName) {
                personal = reputation.reputation;
                break;
            }
        }

        return personal;
    }

    public float GetFactionRepuation(Faction factionToFind) {
        float joinedFactionRep = 0f;

        foreach (Faction joinedFaction in joinedFactions) {
            foreach (Reputation reputation in joinedFaction.reputations) {
                if (reputation.factionName == factionToFind.factionName) {
                    joinedFactionRep = reputation.reputation;
                    break;
                }
            }
        }

        return joinedFactionRep;
    }

    public bool CheckIfEnemy(Faction faction) {
        float personalRep = GetPersonalRepuation(faction);

        bool isPersonalEnemy = personalRep <= -100f ? true : false;
        bool isPersonalAlly = personalRep >= 100f ? true : false;

        bool isFactionEnemy = false;
        bool isFactionAlly = false;

        foreach (Faction joinedFaction in joinedFactions) {
            foreach (Reputation reputation in joinedFaction.reputations) {
                if (reputation.factionName == faction.factionName) {
                    // This will need fixed for multiple joined factions
                    isFactionEnemy = faction.IsEnemy(faction);
                    isFactionAlly = faction.IsAlly(faction);
                    break;
                }
            }
        }

        if ((isPersonalEnemy && !isFactionAlly) || (isFactionEnemy && !isPersonalAlly)) {
            return true;
        }

        return false;
    }

    public bool CheckIfAlly(Faction faction) {




        return false;
    }


    public float FindReputation(Faction factionToFind) {
        float personal = 0f;

        foreach (Reputation reputation in personalReputations) {
            if (reputation.factionName == factionToFind.factionName) {
                personal = reputation.reputation;
                break;
            }
        }


        float joinedFactionRep = 0f;

        foreach (Faction joinedFaction in joinedFactions) {
            foreach (Reputation reputation in joinedFaction.reputations) {
                if (reputation.factionName == factionToFind.factionName) {
                    joinedFactionRep = reputation.reputation;
                    break;
                }
            }
        }

        return (personal + joinedFactionRep) / 2;
    }
}
