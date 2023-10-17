using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpHandler : MonoBehaviour
{
    public int level = 1;
    public int exp = 0;
    public int expNextLevel = 10;

    public int levelScalingBase = 5;
    public int levelScaling = 5;
    public int levelIncrement = 10;


    public event System.Action OnExpChange  = delegate { };
    public event System.Action OnLevelUp = delegate { };


    public void GainExp(int amount) {
        if (amount == 0) {
            return;
        }

        exp += amount;

        if (exp >= expNextLevel) {
            LevelUp();
            return;
        }

        OnExpChange();
    }

    public void LevelUp() {
        level += 1;
        exp = 0;

        levelIncrement += levelScaling;
        levelScaling += levelScalingBase;

        expNextLevel = levelIncrement;

        OnLevelUp();

        GetComponent<ObjectHandler>().Heal(GetComponent<ObjectHandler>().statsObject[ObjectStatNames.Health].GetValue()); // Full heal

        OnExpChange();
    }
}
