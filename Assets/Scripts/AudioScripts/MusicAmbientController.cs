using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicAmbientController : MonoBehaviour {

    public enum MusicTypes { None, Default, Battle, Victory, Defeat } //Add new state "None" and call SetMusicType add Start()
    MusicTypes musicType = MusicTypes.None;  //Actual
    MusicTypes previousMusicType = MusicTypes.None;  //Previa a la actual

    public List<SceneAudios> scenesAudios = new List<SceneAudios>();
    public EndingAudios endingAudios;

    List<SourcesAndVolume> currentAudioSources = new List<SourcesAndVolume>();
    List<SourcesAndVolume> upcomingAudioSources = new List<SourcesAndVolume>();

    bool initTransitionDone = false;
    bool isTransitioning = false;
    bool transitionWithDelay = false;

    float transitionDuration = 0;
    float transitionTimer = 0;
    float transitionDelayDuration = 0;
    float transitionDelayTimer = 0;

	
	void Update ()
    {
        if (!initTransitionDone)
        {
            if (scenesAudios.Exists(scene => scene.name == SceneManager.GetActiveScene().name))
            {
                SetMusicType(MusicTypes.Default, 1f, 0.7f);
                initTransitionDone = true;
            }
        }

	    if (isTransitioning)
        {
            if (transitionWithDelay)
            {
                transitionTimer += Time.deltaTime;

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
                }

                if(transitionTimer <= transitionDuration + transitionDelayDuration)
                {
                    if (transitionDelayTimer > 0)
                    {
                        transitionDelayTimer -= Time.deltaTime;

                        if (transitionDelayTimer <= 0)
                        {
                            foreach (SourcesAndVolume audioSource in upcomingAudioSources)
                            {
                                audioSource.source.UnPause();
                            }
                        }

                        return;
                    }

                    foreach (SourcesAndVolume audioSource in upcomingAudioSources)
                    {
                        if (audioSource.source.volume >= audioSource.volume)
                        {
                            if (audioSource.source.volume != audioSource.volume) audioSource.source.volume = audioSource.volume;
                            continue;
                        }

                        audioSource.source.volume = Mathf.SmoothStep(0, audioSource.volume, transitionTimer - transitionDelayDuration / transitionDuration);
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
            else
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
	}

    void Play(int sceneAudiosIndex, int clipIndex, string groupName, MusicTypes clipType, bool addToUpcomingSources, bool playedWithDelay)
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
            case MusicTypes.Victory:
                source.Play(endingAudios.victoryAudios[clipIndex].clip, scenesAudios[sceneAudiosIndex].battleAudios[clipIndex].volume, 1f, true, true, groupName);
                break;
            case MusicTypes.Defeat:
                source.Play(endingAudios.defeatAudios[clipIndex].clip, scenesAudios[sceneAudiosIndex].battleAudios[clipIndex].volume, 1f, true, true, groupName);
                break;
            default:
                break;
        }

        //if (!loop) Destroy(source, source.clip.length);

        if (addToUpcomingSources)
        {
            SourcesAndVolume newSource = new SourcesAndVolume(source, source.volume);
            upcomingAudioSources.Add(newSource);

            if (playedWithDelay) source.Pause();
        }
    }

    /// <summary>
    /// Changes the audio of the game in a smooth way
    /// </summary>
    /// <param name="desiredMusicType"></param> Type of music to play
    /// <param name="transitionTime"></param> Time of the current audios to fade out and the upcoming audios to fade in
    /// <param name="transitionDelayTime"></param> Delay of the audio transition
    public void SetMusicType(MusicTypes desiredMusicType, float transitionTime, float transitionDelayTime) //Metodo para hacer blending de audios
    {
        if (desiredMusicType == musicType) return;

        transitionWithDelay = (transitionDelayTime > 0) ? true : false;

        if (isTransitioning)
        {
            if (previousMusicType == desiredMusicType)  //Musica previa a la actual == Musica por venir
            {
                transitionTimer = (transitionTimer / transitionDuration) * transitionTime;
                transitionDuration = transitionTime;
                transitionDelayTimer = (1 - (transitionDelayTimer / transitionDelayDuration)) * transitionDelayTime;
                transitionDelayDuration = transitionDelayTime;

                List<SourcesAndVolume> tempCurrentAudioSources = currentAudioSources;

                currentAudioSources = upcomingAudioSources;
                upcomingAudioSources = tempCurrentAudioSources;
            }
            else
            {
                transitionTimer = 0;
                transitionDuration = transitionTime;
                if (transitionWithDelay) transitionDelayTimer = transitionDelayDuration = transitionDelayTime;

                currentAudioSources.AddRange(upcomingAudioSources);

                foreach (SourcesAndVolume audioSource in currentAudioSources)
                {
                    audioSource.volume = audioSource.source.volume;
                }

                upcomingAudioSources.Clear();

                int sceneAudiosIndex = scenesAudios.FindIndex(scene => scene.name == SceneManager.GetActiveScene().name);

                switch (desiredMusicType)
                {
                    case MusicTypes.Default:
                        for (int index = 0; index < scenesAudios[sceneAudiosIndex].defaultAudios.Length; index++)
                        {
                            string clipGroupName = " ";

                            if (scenesAudios[sceneAudiosIndex].name == "Thor_Tunderlord_Forest") clipGroupName = (index == 0 || index == 1) ? "Ambient" : "Music";
                            else if (scenesAudios[sceneAudiosIndex].name == "Thor_Tunderlord_FenrirBoss") clipGroupName = (index == 0) ? "Music" : "Ambient";

                            Play(sceneAudiosIndex, index, clipGroupName, desiredMusicType, true, transitionWithDelay);
                            upcomingAudioSources[index].source.volume = 0;
                        }
                        break;
                    case MusicTypes.Battle:
                        for (int index = 0; index < scenesAudios[sceneAudiosIndex].battleAudios.Length; index++)
                        {
                            string clipGroupName = " ";

                            if (scenesAudios[sceneAudiosIndex].name == "Thor_Tunderlord_Forest") clipGroupName = (index == 0) ? "Music" : "Ambient";
                            else if (scenesAudios[sceneAudiosIndex].name == "Thor_Tunderlord_FenrirBoss") clipGroupName = (index == 0) ? "Music" : "Ambient";

                            Play(sceneAudiosIndex, index, clipGroupName, desiredMusicType, true, transitionWithDelay);
                            upcomingAudioSources[index].source.volume = 0;
                        }
                        break;
                    case MusicTypes.Victory:
                        for (int index = 0; index < endingAudios.victoryAudios.Length; index++)
                        {
                            string clipGroupName = (index == 0) ? "Music" : "Ambient";
                            Play(sceneAudiosIndex, index, clipGroupName, desiredMusicType, true, transitionWithDelay);
                            upcomingAudioSources[index].source.volume = 0;
                        }
                        break;
                    case MusicTypes.Defeat:
                        for (int index = 0; index < endingAudios.defeatAudios.Length; index++)
                        {
                            string clipGroupName = (index == 0) ? "Music" : "Ambient";
                            Play(sceneAudiosIndex, index, clipGroupName, desiredMusicType, true, transitionWithDelay);
                            upcomingAudioSources[index].source.volume = 0;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            transitionTimer = 0;
            transitionDuration = transitionTime;
            if (transitionWithDelay) transitionDelayTimer = transitionDelayDuration = transitionDelayTime;

            if (currentAudioSources.Capacity > 0) currentAudioSources.Clear();
            if (upcomingAudioSources.Capacity > 0) upcomingAudioSources.Clear();

            foreach (AudioSource source in gameObject.GetComponents<AudioSource>())
            {
                SourcesAndVolume sourceAndVolume = new SourcesAndVolume(source, source.volume);
                currentAudioSources.Add(sourceAndVolume);
            }

            int sceneAudiosIndex = scenesAudios.FindIndex(scene => scene.name == SceneManager.GetActiveScene().name);

            switch (desiredMusicType)
            {
                case MusicTypes.Default:
                    for (int index = 0; index < scenesAudios[sceneAudiosIndex].defaultAudios.Length; index++)
                    {
                        string clipGroupName = " ";

                        if (scenesAudios[sceneAudiosIndex].name == "Thor_Tunderlord_Forest") clipGroupName = (index == 0 || index == 1) ? "Ambient" : "Music";
                        else if (scenesAudios[sceneAudiosIndex].name == "Thor_Tunderlord_FenrirBoss") clipGroupName = (index == 0) ? "Music" : "Ambient";
                        
                        Play(sceneAudiosIndex, index, clipGroupName, desiredMusicType, true, transitionWithDelay);
                        upcomingAudioSources[index].source.volume = 0;
                    }
                    break;
                case MusicTypes.Battle:
                    for (int index = 0; index < scenesAudios[sceneAudiosIndex].battleAudios.Length; index++)
                    {
                        string clipGroupName = " ";

                        if (scenesAudios[sceneAudiosIndex].name == "Thor_Tunderlord_Forest") clipGroupName = (index == 0) ? "Music" : "Ambient";
                        else if (scenesAudios[sceneAudiosIndex].name == "Thor_Tunderlord_FenrirBoss") clipGroupName = (index == 0) ? "Music" : "Ambient";

                        Play(sceneAudiosIndex, index, clipGroupName, desiredMusicType, true, transitionWithDelay);
                        upcomingAudioSources[index].source.volume = 0;
                    }
                    break;
                case MusicTypes.Victory:
                    for (int index = 0; index < endingAudios.victoryAudios.Length; index++)
                    {
                        string clipGroupName = (index == 0) ? "Music" : "Ambient";
                        Play(sceneAudiosIndex, index, clipGroupName, desiredMusicType, true, transitionWithDelay);
                        upcomingAudioSources[index].source.volume = 0;
                    }
                    break;
                case MusicTypes.Defeat:
                    for (int index = 0; index < endingAudios.defeatAudios.Length; index++)
                    {
                        string clipGroupName = (index == 0) ? "Music" : "Ambient";
                        Play(sceneAudiosIndex, index, clipGroupName, desiredMusicType, true, transitionWithDelay);
                        upcomingAudioSources[index].source.volume = 0;
                    }
                    break;
                default:
                    break;
            }

            isTransitioning = true;
        }

        previousMusicType = musicType;
        musicType = desiredMusicType;
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
public class EndingAudios
{
    public AudioClipAndVolume[] victoryAudios;
    public AudioClipAndVolume[] defeatAudios;

    public EndingAudios (AudioClipAndVolume[] victoryClips, AudioClipAndVolume[] defeatClips)
    {
        victoryAudios = victoryClips;
        defeatAudios = defeatClips;
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
