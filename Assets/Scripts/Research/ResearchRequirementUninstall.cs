using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchRequirementUninstall : ResearchRequirement
{
    public string modId;


    public override string GetProgress() {
        return $"Uninstalled {current}/{total} {ModManager.Instance.FindModById(modId).modName}";
    }

    public override void Begin() {
        ModManager.Instance.OnModChanged += OnModChanged;
    }

    public override void End() {
        ModManager.Instance.OnModChanged -= OnModChanged;
    }


    void OnModChanged(WeaponMod oldMod, WeaponMod newMod) {
        if (oldMod.modId == modId) {
            current++;

            // Think do this in ReseachOption, just so it isn't called multiple times
            // if (current >= total) {
            //     End();
            // }
        }
    }
}
