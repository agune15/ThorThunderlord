using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicAmbientController : MonoBehaviour {

    public enum MusicTypes { Default, Battle }
    MusicTypes musicType = MusicTypes.Default;

    public List<SceneAudios> scenesAudios = new List<SceneAudios>();

    List<SourcesAndVolume> currentAudioSources = new List<SourcesAndVolume>();
    List<SourcesAndVolume> upcomingAudioSources = new List<SourcesAndVolume>();

    bool isTransitioning = false;
    float transitionDuration = 0;
    float transitionTimer = 0;


    void Start ()
    {
        int sceneAudiosIndex = scenesAudios.FindIndex(scene => scene.name == SceneManager.GetActiveScene().name);

        for (int index = 0; index < scenesAudios[sceneAudiosIndex].defaultAudios.Length; index++)
        {
            Play(sceneAudiosIndex, index, "Ambient", MusicTypes.Default, false);
        }
	}
	
	void Update () {
		
        if (isTransitioning)
        {
            if (transitionTimer <= transitionDuration)
            {
                transitionTimer += Time.deltaTime;

                foreach (SourcesAndVolume audioSource in currentAudioSources)
                {
                    if (audioSource.source.volume <= 0)
                    {
                        if (audioSource.source.volume != 0) audioSource.source.volume = 0;
                        continue;
                    }

                    audioSource.source.volume = Mathf.SmoothStep(audioSource.volume, 0, transitionTimer / transitionDuration);
                }

                foreach (SourcesAndVolume audioSource in upcomingAudioSources)
                {
                    if (audioSource.source.volume >= audioSource.volume)
                    {
                        if (audioSource.source.volume != audioSource.volume) audioSource.source.volume = audioSource.volume;
                        continue;
                    }

                    audioSource.source.volume = Mathf.SmoothStep(0, audioSource.volume, transitionTimer / transitionDuration);
                }
            }
            else
            {
                isTransitioning = false;

                foreach (SourcesAndVolume audioSource in currentAudioSources)
                {
                    Destroy(audioSource.source);
                }

                foreach (SourcesAndVolume audioSource in upcomingAudioSources)
                {
                    if (audioSource.source.volume != audioSource.volume) audioSource.source.volume = audioSource.volume;
                }
            }
        }
	}

    void Play(int sceneAudiosIndex, int clipIndex, string groupName, MusicTypes clipType, bool addToUpcomingSources)
    {
        AudioSource source = this.gameObject.AddAudioSource();

        switch (clipType)
        {
            case MusicTypes.Default:
                source.Play(scenesAudios[sceneAudiosIndex].defaultAudios[clipIndex].clip, scenesAudios[sceneAudiosIndex].defaultAudios[clipIndex].volume, 1f, true, true, groupName);
                break;
            case MusicTypes.Battle:
                source.Play(scenesAudios[sceneAudiosIndex].battleAudios[clipIndex].clip, scenesAudios[sceneAudiosIndex].battleAudios[clipIndex].volume, 1f, true, true, groupName);
                break;
            default:
                break;
        }

        //if (!loop) Destroy(source, source.clip.length);

        if (addToUpcomingSources)
        {
            SourcesAndVolume newSource = new SourcesAndVolume(source, source.volume);
            upcomingAudioSources.Add(newSource);
        }
    }

    public void SetMusicType(MusicTypes desiredMusicType, float transitionTime) //Metodo para hacer blending de audios
    {
        if (desiredMusicType == musicType) return;

        musicType = desiredMusicType;

        if (isTransitioning)
        {
            transitionTimer = (transitionTimer / transitionDuration) * transitionTime;
            transitionDuration = transitionTime;

            List<SourcesAndVolume> tempCurrentAudioSources = currentAudioSources;

            currentAudioSources = upcomingAudioSources;
            upcomingAudioSources = tempCurrentAudioSources;
        }
        else
        {
            transitionTimer = 0;
            transitionDuration = transitionTime;

            if (currentAudioSources.Capacity > 0) currentAudioSources.Clear();
            if (upcomingAudioSources.Capacity > 0) upcomingAudioSources.Clear();

            foreach (AudioSource source in gameObject.GetComponents<AudioSource>())
            {
                SourcesAndVolume sourceAndVolume = new SourcesAndVolume(source, source.volume);
                currentAudioSources.Add(sourceAndVolume);
            }

            int sceneAudiosIndex = scenesAudios.FindIndex(scene => scene.name == SceneManager.GetActiveScene().name);

            switch (musicType)
            {
                case MusicTypes.Default:
                    for (int index = 0; index < scenesAudios[sceneAudiosIndex].defaultAudios.Length; index++)
                    {
                        string clipGroupName = (index == 0 || index == 1) ? "Ambient" : "Music";
                        Play(sceneAudiosIndex, index, clipGroupName, musicType, true);
                        upcomingAudioSources[index].source.volume = 0;
                    }
                    break;
                case MusicTypes.Battle:
                    for (int index = 0; index < scenesAudios[sceneAudiosIndex].battleAudios.Length; index++)
                    {
                        string clipGroupName = (index == 0) ? "Music" : "Ambient";
                        Play(sceneAudiosIndex, index, clipGroupName, musicType, true);
                        upcomingAudioSources[index].source.volume = 0;
                    }
                    break;
                default:
                    break;
            }


            isTransitioning = true;
        }
    }
}

[System.Serializable]
public class SceneAudios {

    public string name;
    public AudioClipAndVolume[] defaultAudios;
    public AudioClipAndVolume[] battleAudios;

    public SceneAudios (string sceneName, AudioClipAndVolume[] defaultClips, AudioClipAndVolume[] battleClips)
    {
        name = sceneName;
        defaultAudios = defaultClips;
        battleAudios = battleClips;
    }
}

[System.Serializable]
public class AudioClipAndVolume
{
    public AudioClip clip;
    public float volume;

    public AudioClipAndVolume (AudioClip audioClip, float clipVolume)
    {
        clip = audioClip;
        volume = clipVolume;
    }
}

public class SourcesAndVolume
{
    public AudioSource source;
    public float volume;

    public SourcesAndVolume (AudioSource audioSource, float audioVolume)
    {
        source = audioSource;
        volume = audioVolume;
    }
}
