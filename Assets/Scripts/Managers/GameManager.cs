using System.Collections;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public CinemachineVirtualCamera cinemachineVirtualCamera;
    float startingIntensity;
    float shakeDuration;
    float shakeTimer = 0f;

    public bool gamePaused {get; private set;} = false;

    public bool waitingHitStop {get; private set;} = false;
    public bool waitingTimeScaleChange {get; private set;} = false;


    public LevelData startingLevel;
    public BaseStats playerBaseStats;


    public CinemachineVirtualCamera virtualCamera;


    public event System.Action PlayerCreated = delegate { };


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);

        LevelManager.instance.LoadLevel(startingLevel);
    }

    void Start() {
        ObjectHandler player = Instantiate(playerBaseStats.prefab).GetComponent<ObjectHandler>();

        player.CreateBaseObject(playerBaseStats);

        PlayerCreated();

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


    public void ChangeTimeScale(float duration, float timeScale) {
        if (waitingTimeScaleChange)
            return;
            
        StartCoroutine(WaitChangeTimeScale(duration, timeScale));
    }

    private IEnumerator WaitChangeTimeScale(float duration, float timeScale) 
    {
        waitingTimeScaleChange = true;
        Time.timeScale = timeScale;

        yield return new WaitForSecondsRealtime(duration);

        waitingTimeScaleChange = false;
        Time.timeScale = 1f;
    }
}
