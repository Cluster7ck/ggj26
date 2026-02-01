using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class CrowdCheer : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip[] badSounds;
    public AudioClip[] goodSounds;
    public AudioClip[] wowSounds;

    private SpectatorController.Mood lastMood = SpectatorController.Mood.Bad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameController.Instance.Score.OnScoreChanged += ScoreOnOnScoreChanged;
        audioSource = GetComponent<AudioSource>();
        Cheer(SpectatorController.Mood.Good);
    }

    private void OnDestroy()
    {
        GameController.Instance.Score.OnScoreChanged -= ScoreOnOnScoreChanged;
    }

    private void ScoreOnOnScoreChanged(object sender, int e)
    {
        Cheer(GetMoodFromAccuracy(e));
    }
    
    private SpectatorController.Mood GetMoodFromAccuracy(int accuracy)
    {
        if (accuracy < 40)
        {
            return SpectatorController.Mood.Bad;
        }

        if (accuracy < 70)
        {
            return SpectatorController.Mood.Good;
        }

        return SpectatorController.Mood.Wow;
    }

    private void Cheer(SpectatorController.Mood mood)
    {
        if (audioSource.isPlaying && mood == lastMood)
        {
            return;
        }
        
        var sounds = mood switch
        {
            SpectatorController.Mood.Bad => badSounds,
            SpectatorController.Mood.Good => goodSounds,
            SpectatorController.Mood.Wow => wowSounds,
            _ => goodSounds
        };

        var clip = GetRandomSound(sounds);
        audioSource.PlayOneShot(clip);

        lastMood = mood;
    }

    private AudioClip GetRandomSound(IList<AudioClip> audioClips)
    {
        return audioClips[Random.Range(0, audioClips.Count)];
    }
}