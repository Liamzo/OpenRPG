using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "Research Option", menuName = "Research Option")]
public class ResearchOption : ScriptableObject
{
    public string researchId;
    public string researchName;
    public string researchDescription;


    public List<ResearchCategory> researchCategories;
    public List<ResearchTag> researchTags;
    public ResearchState researchState;


    [SerializeReference] public List<DiscoverRequirement> discoverRequirements;
    [SerializeReference] public List<ResearchRequirement> researchRequirements;
    [SerializeReference] public List<AssembleRequirement> assembleRequirements;
    [SerializeReference] public List<ResearchUnlock> unlockRewards;


    // Maybe when saving/loading games, this will take in an optional json string to load from
    public void Create() { 
        if (researchState == ResearchState.Unknown) {
            discoverRequirements.ForEach(discoverRequirement => discoverRequirement.Begin());
            discoverRequirements.ForEach(discoverRequirement => discoverRequirement.OnDiscover += OnDiscover);
        }
        else if (researchState == ResearchState.Discovered) {
            researchRequirements.ForEach(researchRequirement => researchRequirement.Begin());
        } 
        else if (researchState == ResearchState.Researched) {
            assembleRequirements.ForEach(assembleRequirement => assembleRequirement.Begin());
        }
    }

    public bool SetState(ResearchState state) {
        if (state <= researchState) return false;

        if (researchState == ResearchState.Unknown) {
            discoverRequirements.ForEach(discoverRequirement => discoverRequirement.End());
        } 
        else if (researchState == ResearchState.Discovered) {
            researchRequirements.ForEach(researchRequirement => researchRequirement.End());
        } 
        else if (researchState == ResearchState.Researched) {
            assembleRequirements.ForEach(assembleRequirement => assembleRequirement.End());
        }

        if (state == ResearchState.Discovered) {
            researchRequirements.ForEach(researchRequirement => researchRequirement.Begin());
        } 
        else if (state == ResearchState.Researched) {
            assembleRequirements.ForEach(assembleRequirement => assembleRequirement.Begin());
        }
        else if (state == ResearchState.Unlocked) {
            unlockRewards.ForEach(unlockReward => unlockReward.Unlock());
        }

        researchState = state;


        return true;
    }


    void OnDiscover() {
        bool completed = true;

        foreach (DiscoverRequirement discoverRequirement in discoverRequirements) {
            if (discoverRequirement.current < discoverRequirement.total) {
                completed = false;
                break;
            }
        }

        if (completed)
            SetState(ResearchState.Discovered);
    }


    public bool TryComplete() {
        if (researchState == ResearchState.Discovered) {
            bool completed = true;

            foreach (ResearchRequirement researchRequirement in researchRequirements) {
                if (researchRequirement.current < researchRequirement.total) {
                    completed = false;
                    break;
                }
            }

            if (completed)
                SetState(ResearchState.Researched);
            

            return completed;
        }
        else if (researchState == ResearchState.Researched) {
            bool completed = true;

            foreach (ResearchRequirement assembleRequirement in assembleRequirements) {
                if (assembleRequirement.current < assembleRequirement.total) {
                    completed = false;
                    break;
                }
            }

            if (completed)
                SetState(ResearchState.Unlocked);
            

            return completed;
        }


        return false;
    }


    public string GetName() {
        if (researchState == ResearchState.Unknown) return "?";

        return researchName;
    }
    public string GetDescription() {
        if (researchState == ResearchState.Unknown) return "";

        return researchDescription;
    }

    public string GetProgress() {
        string requirements = "";

        if (researchState == ResearchState.Discovered) {
            foreach (ResearchRequirement researchRequirement in researchRequirements) {
                requirements += researchRequirement.GetProgress();
            }
        }
        else if (researchState == ResearchState.Researched) {
            foreach (ResearchRequirement assembleRequirement in assembleRequirements) {
                requirements += assembleRequirement.GetProgress();
            }
        }

        return requirements;
    }
}


public enum ResearchState {
    Unknown,
    Discovered,
    Researched,
    Unlocked
}

public enum ResearchTag {
    [Description("Pistol Magazine")] PistolMagazine,
    [Description("Pistol Barrel")]PistolBarrel,
    [Description("Sword Blade")]SwordBlade,
}