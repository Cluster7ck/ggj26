using PrimeTween;
using UnityEngine;

public class LockInEffect : MonoBehaviour
{
    public float scaleUpDuration;

    public float shakeStrength = 0.1f;

    public float rotationStrength = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        transform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        DoAnim();
    }

    public void DoAnim()
    {
        var oldPos = transform.localPosition;
        Sequence.Create()
            .Group(Tween.Scale(transform, 1, scaleUpDuration, Easing.Overshoot(1.2f)))
            .Group(Tween.ShakeLocalPosition(transform, new Vector3(1f, 1f, 0) * shakeStrength, scaleUpDuration))
            .Group(Tween.PunchLocalRotation(transform, new Vector3(0f, 0f, 1f) * rotationStrength, scaleUpDuration))
            .OnComplete(() =>
            {
                transform.localPosition = oldPos;
                transform.localScale = Vector3.zero;
            });
    }
}
