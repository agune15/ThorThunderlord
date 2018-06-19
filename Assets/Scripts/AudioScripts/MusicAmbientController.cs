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

        if (SceneManager.GetActiveScene().name == "Thor_Tunderlord_Forest")
        {
            foreach(AudioClip defaultClips in scenesAudios[sceneAudiosIndex].defaultAudios)
            {
                if (defaultClips.name == "ForestAmbient") Play(sceneAudiosIndex, 0, 0.8f, 1, true, true, "Ambient", MusicTypes.Default);
                else if (defaultClips.name == "HowlingWind") Play(sceneAudiosIndex, 1, 0.35f, 1, true, true, "Ambient", MusicTypes.Default);
            }
        }
        else if (SceneManager.GetActiveScene().name == "Thor_Tunderlord_FenrirBoss")
        {

        }
	}
	
	void Update () {
		
        if (isTransitioning)
        {
            if (transitionTimer <= transitionDuration)
            {
                transitionTimer += Time.deltaTime;

                //Mathf.SmoothStep para las transiciones

            }
        }

	}
    
    void Play(int sceneAudiosIndex, int clipIndex, float volume, float pitch, bool loop, bool audio2D, string groupName, MusicTypes clipType) //Metodo para playear audio
    {
        AudioSource source = this.gameObject.AddAudioSource();

        switch (clipType)
        {
            case MusicTypes.Default:
                source.Play(scenesAudios[sceneAudiosIndex].defaultAudios[clipIndex], volume, pitch, loop, audio2D, groupName);
                break;
            case MusicTypes.Battle:
                source.Play(scenesAudios[sceneAudiosIndex].battleAudios[clipIndex], volume, pitch, loop, audio2D, groupName);
                break;
            default:
                break;
        }
        
        if (!loop) Destroy(source, source.clip.length);
    }

    public void SetMusicType(MusicTypes musicType, float transitionTime) //Metodo para hacer blending de audios
    {
        transitionDuration = transitionTime;

        if (currentAudioSources.Capacity > 0) currentAudioSources.Clear();
        if (upcomingAudioSources.Capacity > 0) upcomingAudioSources.Clear();

        foreach (AudioSource source in gameObject.GetComponents<AudioSource>())
        {
            SourcesAndVolume sourceAndVolume = new SourcesAndVolume(source, source.volume);
            currentAudioSources.Add(sourceAndVolume);
        }

        //playear audios
        //guardar audioplayers

        //isTransitioning = true;
    }


    
}

[System.Serializable]
public class SceneAudios {

    public string name;
    public AudioClip[] defaultAudios;
    public AudioClip[] battleAudios;

    public SceneAudios (string sceneName, AudioClip[] defaultClips, AudioClip[] battleClips)
    {
        name = sceneName;
        defaultAudios = defaultClips;
        battleAudios = battleClips;
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
