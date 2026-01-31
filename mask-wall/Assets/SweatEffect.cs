using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;

public class SweatEffect : MonoBehaviour
{
  public float distance;

  [FormerlySerializedAs("duration")] public float moveDuration;
  [FormerlySerializedAs("duration")] public float scaleDuration;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    Sequence.Create(
        Tween.PositionY(transform, transform.position.y - distance, moveDuration)
      ).Chain(Tween.Scale(transform, 0, scaleDuration))
      .OnComplete(() => Destroy(gameObject));
  }
}