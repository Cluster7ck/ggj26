using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;

public class SweatEffect : MonoBehaviour
{
  public float distance;

  [FormerlySerializedAs("duration")] public float moveDuration;
  [FormerlySerializedAs("duration")] public float scaleDuration;

  private void Start()
  {
    Trigger();
  }

  public void Trigger()
  {
    var child = transform.GetChild(0);
    Sequence.Create(
        Tween.PositionY(transform, transform.position.y - distance, moveDuration)
      ).Chain(Tween.Scale(child.transform, 0, scaleDuration))
      .OnComplete(() => Destroy(gameObject));
  }
}