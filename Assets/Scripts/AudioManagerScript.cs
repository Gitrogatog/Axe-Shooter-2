using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    public enum AudioChannel {Master, Sfx, Music};
    public float masterVolumePercent { get; private set;}
    public float sfxVolumePercent { get; private set;}
    public float musicVolumePercent { get; private set;}

    AudioSource sfx2DSource;
    AudioSource[] musicSources;
    int activeMusicSourceIndex = 0;
    public static AudioManagerScript instance;

    Transform audioListener;
    Transform playerT;

    SoundLibrary library;

    void Awake(){
        if(instance != null){
            Destroy(gameObject);
        }
        else{
            instance = this;
            DontDestroyOnLoad(gameObject);

            musicSources = new AudioSource[2];
            for(int i = 0; i < 2; i++){
                GameObject newMusicSource = new GameObject("Music Source " + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
                musicSources[i].loop = true;
            }

            GameObject newSfx2DSource = new GameObject("2D Sfx Source ");
            sfx2DSource = newSfx2DSource.AddComponent<AudioSource>();
            newSfx2DSource.transform.parent = transform;

            audioListener = FindObjectOfType<AudioListener>().transform;
            PlayerMovement pInput = FindObjectOfType<PlayerMovement>();
            if(pInput != null){
                playerT = pInput.transform;
            }
            library = GetComponent<SoundLibrary>();
        }

        masterVolumePercent = PlayerPrefs.GetFloat("Master Volume", 1);
        sfxVolumePercent = PlayerPrefs.GetFloat("Sfx Volume", 1);
        musicVolumePercent = PlayerPrefs.GetFloat("Music Volume", 1);
    }

    void Update(){
        if(playerT != null){
            audioListener.position = playerT.position;
        }
    }

    public void SetVolume(float newVolumePercent, AudioChannel channel){
        switch(channel){
            case AudioChannel.Master:
                masterVolumePercent = newVolumePercent;
                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = newVolumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = newVolumePercent;
                break;
        }
        
        for(int i = 0; i < 2; i++){
            musicSources[i].volume = musicVolumePercent * masterVolumePercent * 0.5f;
        }

        PlayerPrefs.SetFloat("Master Volume", masterVolumePercent);
        PlayerPrefs.SetFloat("Sfx Volume", sfxVolumePercent);
        PlayerPrefs.SetFloat("Music Volume", musicVolumePercent);
        PlayerPrefs.Save();
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1){
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlaySound(AudioClip clip, Vector2 pos){
        if(clip != null){
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }

    public void PlaySound(string soundName, Vector2 pos){
        PlaySound(library.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName){
        AudioClip clip = library.GetClipFromName(soundName);
        if(clip != null){
            sfx2DSource.PlayOneShot(clip, sfxVolumePercent * masterVolumePercent);
        }
        
    }

    IEnumerator AnimateMusicCrossfade(float duration){
        float percent = 0;
        float speed = 1 / duration;

        while(percent < 1){
            percent += Time.deltaTime * speed;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
    }
    void OnLevelWasLoaded(int index) {
  	if (playerT == null) {
          PlayerMovement playerInput = FindObjectOfType<PlayerMovement> ();
  		if (playerInput != null) {
  			playerT = playerInput.transform;
  		}
  	}
  }
}
