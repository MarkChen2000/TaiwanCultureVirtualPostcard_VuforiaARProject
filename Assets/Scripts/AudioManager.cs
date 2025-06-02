using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Serializable]
    public class OneShotAudioData
    {
        public string audioID;
        public AudioClip audioClip; // 音效檔案
        public AudioMixerGroup audioMixerGroup; // 音效混音組
    }

    [SerializeField]
    AudioMixer audioMixer_Master;

    [SerializeField]
    Transform audioMainListenerTrans;

    [SerializeField]
    List<OneShotAudioData> loadedOneShotAudioDataList = new List<OneShotAudioData>();

    public void SetPostcardBGAudioVolume(float volume)
    {
        // 設定明信片背景音樂的音量
        audioMixer_Master.SetFloat("PostcardBGAudioVolume", volume);
    }

    public void PlayOneShotAudioByID(string audioID)
    {
        // 根據音效ID播放一次性音效
        OneShotAudioData audioData = loadedOneShotAudioDataList.Find(data => data.audioID == audioID);
        if (audioData == null) {
            Debug.LogWarning($"Audio with ID '{audioID}' not found in loaded audio data.");
            return;
        }
        GameObject audioContainerGO = new GameObject($"OneShotAudio_{audioID}");
        audioContainerGO.transform.SetParent(audioMainListenerTrans, false);
        AudioSource audioSource = audioContainerGO.AddComponent<AudioSource>();
        audioSource.clip = audioData.audioClip;
        if (audioSource.outputAudioMixerGroup!=null) {
            audioSource.outputAudioMixerGroup = audioData.audioMixerGroup;
        }
        audioSource.Play();
        Destroy(audioContainerGO, audioSource.clip.length); 
    }
}
