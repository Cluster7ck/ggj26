using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

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

    private Coroutine _coroutine;

    private void Awake()
    {
        transform.Translate(new Vector3(-0.3f + Random.value * 0.6f, 0f,-0.3f  + Random.value * 0.6f));

        if (Random.Range(0, 3) > 0)
        {
           // Destroy(hands.gameObject);
        }
    }

    void Start()
    {
        Cheer();
        ApplyMood(Mood.Good, initialGeneration: true);
        GameController.Instance.Score.OnScoreChanged += ScoreOnOnScoreChanged;
    }

    private Sequence handsSequence;
    private Tween handsTween;

    void Cheer()
    {
        if (hands.transform == null)
        {
            return;
        }
        
        var random = Random.Range(0, 2);

        Tween.ShakeLocalPosition(transform, strength: new Vector3(0, 0.4f), duration: 1 + Random.value * 0.5f,
            frequency: 6 + Random.Range(0, 4));

        if (random == 1)
        {
            CheerRotate();
        }
        else
        {
            CheerMove();
        }
    }

    void CheerRotate()
    {
        handsSequence.Stop();
        handsTween.Stop();
        handsSequence = Sequence.Create()
            .Chain(Tween.LocalEulerAngles(hands.transform, startValue: hands.transform.localRotation.eulerAngles,
                endValue: Vector3.forward * -25, duration: 0.3f + Random.value * 0.5f, Ease.InOutSine))
            .Chain(Tween.LocalEulerAngles(hands.transform, startValue: Vector3.forward * -25,
                endValue: Vector3.forward * 25, duration: 0.3f, Ease.InOutSine, 3 + Random.Range(3, 6),
                CycleMode.Rewind))
            .Chain(Tween.LocalEulerAngles(hands.transform, startValue: hands.transform.localRotation.eulerAngles,
                endValue: Vector3.forward * -15, duration: 0.5f + Random.value * 0.25f, Ease.InOutSine).OnComplete(() =>
            {
                handsTween = Tween.LocalEulerAngles(hands.transform, startValue: Vector3.forward * -15,
                    endValue: Vector3.forward * 15, duration: 0.8f, Ease.InOutSine, -1, CycleMode.Rewind);
            }));
    }

    void CheerMove()
    {
        handsSequence.Stop();
        handsTween.Stop();
        handsSequence = Sequence.Create()
            .Chain(Tween.LocalPosition(hands.transform, startValue: hands.transform.localPosition,
                endValue: Vector3.up * 0.25f, duration: 0.3f + Random.value * 0.5f, Ease.InOutSine))
            .Chain(Tween.LocalPosition(hands.transform, startValue: Vector3.up * 0.25f,
                endValue: Vector3.up * 2f, duration: 0.3f, Ease.InOutSine, 3 + Random.Range(3, 6),
                CycleMode.Rewind))
            .Chain(Tween.LocalPosition(hands.transform, startValue: hands.transform.localPosition,
                endValue: Vector3.up * 0.35f, duration: 0.5f + Random.value * 0.25f, Ease.InOutSine).OnComplete(() =>
            {
                handsTween = Tween.LocalPosition(hands.transform, startValue: Vector3.up * 0.35f,
                    endValue: Vector3.up * 1.8f, duration: 0.8f, Ease.InOutSine, -1, CycleMode.Rewind);
            }));
    }

    private void OnDestroy()
    {
        GameController.Instance.Score.OnScoreChanged -= ScoreOnOnScoreChanged;
    }

    private void ScoreOnOnScoreChanged(object sender, int e)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(ApplyMoodAsync(GetMoodFromAccuracy(e)));
        Cheer();
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