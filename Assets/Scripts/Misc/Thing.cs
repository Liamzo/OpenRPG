using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class Thing : MonoBehaviour
{
    public string prefabId;
    public float exclusionDist;


    public JSONNode SaveObject() {
        Destroy(gameObject);

        string json = $"{{prefabId: {prefabId}, x: {transform.position.x}, y: {transform.position.y}";

        foreach (ISaveable saveable in GetComponents<ISaveable>()) {
            json += ", " + saveable.SaveComponent();
        }

        return json + "}";
    }

    public void LoadObject(JSONNode data) {
        transform.position = new Vector3(data["x"], data["y"], 0f);

        foreach (ISaveable saveable in GetComponents<ISaveable>()) {
            saveable.LoadComponent(data);
        }
    }
}
