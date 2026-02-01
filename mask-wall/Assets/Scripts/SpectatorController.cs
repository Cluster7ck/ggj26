using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpectatorController : MonoBehaviour
{
    private enum Mood
    {
        Good,
        Wow,
        Bad
    }

    [Header("Body Parts")] public SpriteRenderer head;
    public SpriteRenderer torso;
    public SpriteRenderer hands;
    public SpriteRenderer eyes;
    public SpriteRenderer mouth;

    [Header("Sprites")] public Sprite sunglasses;
    public Sprite[] eyesBad;
    public Sprite[] eyesGood;
    public Sprite[] eyesWow;

    public Sprite[] mouthBad;
    public Sprite[] mouthGood;
    public Sprite[] mouthWow;

    void Start()
    {
        ApplyMood(Mood.Good, initialGeneration: true);
        GameController.Instance.Score.OnScoreChanged += ScoreOnOnScoreChanged;
    }

    private void OnDestroy()
    {
        GameController.Instance.Score.OnScoreChanged -= ScoreOnOnScoreChanged;
    }

    private Coroutine coroutine;

    private void ScoreOnOnScoreChanged(object sender, int e)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(ApplyMoodAsync(GetMoodFromAccuracy(e)));
    }

    private Mood GetMoodFromAccuracy(int accuracy)
    {
        if (accuracy < 40)
        {
            return Mood.Bad;
        }

        if (accuracy < 70)
        {
            return Mood.Good;
        }

        return Mood.Wow;
    }

    IEnumerator ApplyMoodAsync(Mood mood, bool initialGeneration = false)
    {
        yield return new WaitForSeconds(Random.Range(0.0f, 1f));
        ApplyMood(mood, initialGeneration);
    }

    private void ApplyMood(Mood mood, bool initialGeneration = false)
    {
        ApplyRandomEyes(mood, initialGeneration);
        ApplyRandomMouth(mood);
    }

    private void ApplyRandomEyes(Mood mood, bool initialGeneration = false)
    {
        if (eyes.sprite.name == sunglasses.name)
        {
            return;
        }

        var sprites = mood switch
        {
            Mood.Bad => eyesBad.ToList(),
            Mood.Good => eyesGood.ToList(),
            Mood.Wow => eyesWow.ToList(),
            _ => eyesGood.ToList()
        };

        if (initialGeneration)
        {
            sprites.Add(sunglasses);
        }

        eyes.sprite = GetRandomSprite(sprites);
    }

    private void ApplyRandomMouth(Mood mood)
    {
        var sprites = mood switch
        {
            Mood.Bad => mouthBad,
            Mood.Good => mouthGood,
            Mood.Wow => mouthWow,
            _ => mouthGood
        };

        mouth.sprite = GetRandomSprite(sprites);
    }

    private Sprite GetRandomSprite(IList<Sprite> sprites)
    {
        return sprites[Random.Range(0, sprites.Count)];
    }
}