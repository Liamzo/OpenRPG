using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public CinemachineVirtualCamera cinemachineVirtualCamera;
    float startingIntensity;
    float shakeDuration;
    float shakeTimer = 0f;

    public bool gamePaused {get; private set;} = false;

    public bool waitingHitStop {get; private set;} = false;


    public LevelData startingLevel;
    public GameObject playerPrefab;


    public CinemachineVirtualCamera virtualCamera;


    public List<PrefabInfo> allItems;
    public List<PrefabInfo> allCharacters;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);

        LevelManager.instance.LoadLevel(startingLevel);

        for (int i = 0; i < allItems.Count; i++) {
            allItems[i].prefabId = allItems[i].prefab.GetComponent<ObjectHandler>().prefabId;
        }

        for (int i = 0; i < allCharacters.Count; i++) {
            allCharacters[i].prefabId = allCharacters[i].prefab.GetComponent<ObjectHandler>().prefabId;
        }
    }

    void Start() {
        ObjectHandler player = Instantiate(playerPrefab).GetComponent<ObjectHandler>();

        player.CreateBaseObject();

        //LoadAfterFrame(player);

        virtualCamera.Follow = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.GetInstance().GetMenuPressed()) {
            gamePaused = !gamePaused;

            if (gamePaused) {
                Time.timeScale = 0f;
                return;
            } else {
                Time.timeScale = 1f;
            }
        }

        if (shakeTimer < shakeDuration) {
            shakeTimer += Time.deltaTime;

            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, shakeTimer / shakeDuration);
        }
    }
    // IEnumerator LoadAfterFrame(ObjectHandler objectHandler) {
    //     // Wait a frame for scene to load
    //     yield return null;

    //     objectHandler.CreateBaseObject();
    // }



    public void ShakeCamera(float intensity, float duration) {
        CinemachineBasicMultiChannelPerlin cbmcp = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cbmcp.m_AmplitudeGain = intensity;
        startingIntensity = intensity;
        shakeDuration = duration;
        shakeTimer = 0f;
    }


    public void HitStop(float duration) {
        if (waitingHitStop)
            return;

        StartCoroutine(WaitHitStop(duration));
    }

    IEnumerator WaitHitStop(float duration) {
        waitingHitStop = true;
        Time.timeScale = 0.0f;

        yield return new WaitForSecondsRealtime(duration);

        waitingHitStop = false;
        Time.timeScale = 1.0f;
    }


    public GameObject SpawnPrefab(string prefabId) {
        foreach (PrefabInfo info in allItems) {
            if (info.prefabId == prefabId) {
                return Instantiate(info.prefab);
            }
        }

        foreach (PrefabInfo info in allCharacters) {
            if (info.prefabId == prefabId) {
                return Instantiate(info.prefab);
            }
        }

        Debug.LogWarning("No prefab found with that ID");
        return null;
    }
}

[Serializable]
public class PrefabInfo {
    public string prefabId;
    public GameObject prefab;
}
