using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AssembleRequirements
{
    List<ItemRequirement> itemRequirements;


    public string GetProgress() {
        return "";
    }

    public void Begin() {

    }
    public void End() {

    }




    public struct ItemRequirement {
        public string itemId;
        public int target;
        public int current;

        public ItemRequirement(string itemId, int target) {
            this.itemId = itemId;
            this.target = target;
            this.current = 0;
        }
    }
}
