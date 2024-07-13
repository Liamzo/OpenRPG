using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private List<AudioClipSO> audioClips;
    private Dictionary<AudioID, AudioClipSO> audioClipsDict;
    
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource globalSource;


    public float ambientWaitDuration;
    public float ambientWaitDurationRange;
    float ambientWaitTimer;

    public float musicPauseDuration;
    public float musicPauseDurationRange;
    float musicPauseTimer;


    private void Awake() {
        instance = this;

        audioClipsDict = new Dictionary<AudioID, AudioClipSO>();

        foreach (AudioClipSO audioClipSO in audioClips) {
            audioClipsDict.Add(audioClipSO.audioID, audioClipSO);
        }


        ambientWaitTimer = Random.Range(ambientWaitDuration - ambientWaitDurationRange, ambientWaitDuration + ambientWaitDurationRange);
        musicPauseTimer = Random.Range(musicPauseDuration - musicPauseDurationRange, musicPauseDuration + musicPauseDurationRange);
    }

    private void Update() {
        if (LevelManager.instance.currentLevel == null) return;

        // Ambient
        ambientWaitTimer -= Time.deltaTime;

        if (ambientWaitTimer <= 0f) {
            List<AudioID> audioIDs = LevelManager.instance.currentLevel.levelData.ambientAudioList;

            if (audioIDs.Count > 0) {
                int index = Random.Range(0, audioIDs.Count);

                PlayClipRandom(audioIDs[index]);

                ambientWaitTimer = Random.Range(ambientWaitDuration - ambientWaitDurationRange, ambientWaitDuration + ambientWaitDurationRange);
            }
        }


        // Music
        musicPauseTimer -= Time.deltaTime;

        if (musicPauseTimer <= 0f) {
            List<AudioID> audioIDs = LevelManager.instance.currentLevel.levelData.musicAudioList;
            int index = Random.Range(0, audioIDs.Count);

            SetMusic(audioIDs[index]);
        }
    }


    public void PlayClipRandom(AudioID audioID, AudioSource source = null) {
        // Find AudioClipSO
        if (audioClipsDict.ContainsKey(audioID) == false) {
            Debug.LogWarning($"No audio clip found with ID: {audioID}");
            return;
        }

        AudioClipSO audioClipSO = audioClipsDict[audioID];

        // Get random clip
        AudioClip audioClip = audioClipSO.GetRandomClip();

        // Play clip
        if (audioClipSO.audioSourceType == AudioSourceType.Music) {
            musicSource.PlayOneShot(audioClip);
        } else if (audioClipSO.audioSourceType == AudioSourceType.Ambient) {
            ambientSource.PlayOneShot(audioClip);
        } else if (audioClipSO.audioSourceType == AudioSourceType.Local) {
            if (source == null) {
                Debug.LogWarning($"No local source given");
                return;
            }

            source.PlayOneShot(audioClip);
        } else if (audioClipSO.audioSourceType == AudioSourceType.Global) {
            globalSource.PlayOneShot(audioClip);
        }
    }

    public void SetAmbientAudio(AudioID audioID) {
        if (audioClipsDict.ContainsKey(audioID) == false) {
            Debug.LogWarning($"No audio clip found with ID: {audioID}");
            return;
        }

        AudioClipSO audioClipSO = audioClipsDict[audioID];

        if (audioClipSO.audioSourceType != AudioSourceType.Ambient) {
            Debug.LogWarning($"Wrong audio type");
            return;
        }

        ambientSource.clip = audioClipSO.GetRandomClip();
        ambientSource.Play();
    }
    
    public void SetMusic(AudioID audioID) {
        if (audioClipsDict.ContainsKey(audioID) == false) {
            Debug.LogWarning($"No audio clip found with ID: {audioID}");
            return;
        }

        AudioClipSO audioClipSO = audioClipsDict[audioID];

        if (audioClipSO.audioSourceType != AudioSourceType.Music) {
            Debug.LogWarning($"Wrong audio type");
            return;
        }

        musicSource.clip = audioClipSO.GetRandomClip();
        musicSource.Play();

        musicPauseTimer = Random.Range(musicSource.clip.length + musicPauseDuration - musicPauseDurationRange, musicSource.clip.length + musicPauseDuration + musicPauseDurationRange);
    }
}
