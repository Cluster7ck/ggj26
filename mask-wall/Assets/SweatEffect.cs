using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;

public class SweatEffect : MonoBehaviour
{
  public float distance;

  [FormerlySerializedAs("duration")] public float moveDuration;
  [FormerlySerializedAs("duration")] public float scaleDuration;

  public void Trigger()
  {
    Sequence.Create(
        Tween.PositionY(transform, transform.position.y - distance, moveDuration)
      ).Chain(Tween.Scale(transform, 0, scaleDuration))
      .OnComplete(() => Destroy(gameObject));
  }
}