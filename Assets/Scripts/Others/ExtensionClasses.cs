//This class is used to create classes that aren't directly attatched to a GameObject and don't inherit from Monobehaviour. For example, classes that are used to make custom lists.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationClipName      //Store AnimationName (string) + AnimationClip
{
    public string animationName;
    public AnimationClip animationClip;

    public AnimationClipName(string name, AnimationClip clip)
    {
        animationName = name;
        animationClip = clip;
    }
}

[System.Serializable]
public class EnemyStatsTransform    //Store Transform + EnemyStats
{
    public Transform transform;
    public EnemyStats stats;

    public EnemyStatsTransform(Transform enemyTransform, EnemyStats enemyStats)
    {
        transform = enemyTransform;
        stats = enemyStats;
    }
}

#region Sound related classes

[System.Serializable]
public class GameplaySceneAudios
{
    public string name;
    public ClipAndVolume[] defaultAudios;
    public ClipAndVolume[] battleAudios;

    public GameplaySceneAudios(string sceneName, ClipAndVolume[] defaultClips, ClipAndVolume[] battleClips)
    {
        name = sceneName;
        defaultAudios = defaultClips;
        battleAudios = battleClips;
    }
}

[System.Serializable]
public class EndingSceneAudios
{
    public ClipAndVolume[] victoryAudios;
    public ClipAndVolume[] defeatAudios;

    public EndingSceneAudios(ClipAndVolume[] victoryClips, ClipAndVolume[] defeatClips)
    {
        victoryAudios = victoryClips;
        defeatAudios = defeatClips;
    }
}

[System.Serializable]
public class ClipAndVolume
{
    public AudioClip clip;
    public float volume;

    public ClipAndVolume(AudioClip audioClip, float audioVolume)
    {
        clip = audioClip;
        volume = audioVolume;
    }
}

public class SourceAndVolume
{
    public AudioSource source;
    public float volume;

    public SourceAndVolume(AudioSource audioSource, float sourceVolume)
    {
        source = audioSource;
        volume = sourceVolume;
    }
}

#endregion