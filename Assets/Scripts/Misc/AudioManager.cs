using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private List<AudioClipSO> audioClips;
    private Dictionary<AudioID, AudioClipSO> audioClipsDict;
    
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource;


    private void Awake() {
        instance = this;

        audioClipsDict = new Dictionary<AudioID, AudioClipSO>();

        foreach (AudioClipSO audioClipSO in audioClips) {
            audioClipsDict.Add(audioClipSO.audioID, audioClipSO);
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
        }
    }
}
