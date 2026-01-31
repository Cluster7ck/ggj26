using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;

public class SweatEffect : MonoBehaviour
{
  public float distance;

  [FormerlySerializedAs("duration")] public float moveDuration;
  [FormerlySerializedAs("duration")] public float scaleDuration;

  private Vector3 startPos;
  private Vector3 startScale;
  private void Awake()
  {
    startPos = transform.position;
    startScale = transform.GetChild(0).localScale;
  }

  private void OnEnable()
  {
    Trigger();
  }

  private void Trigger()
  {
    var child = transform.GetChild(0);
    Sequence.Create()
      .Group(Tween.ShakeLocalPosition(transform, Vector3.right * 0.015f, moveDuration))
      .Group(Tween.PositionY(transform, transform.position.y - distance, moveDuration))
      .Chain(Tween.Scale(child.transform, 0, scaleDuration))
      .OnComplete(() =>
      {
        gameObject.SetActive(false);
        transform.position = startPos;
        transform.GetChild(0).localScale = startScale;
      });
  }
}