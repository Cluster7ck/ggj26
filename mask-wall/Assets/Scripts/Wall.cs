using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class Wall : MonoBehaviour
{
  private static readonly int baseTextureName = Shader.PropertyToID("_Base");
  private float initialZ;
  private List<float> zSteps = new();
  private int currentStep = 0;
  private float currentStepSize;
  private Shape currenShape;
  private Sequence currentSequence;

  public MeshRenderer meshRenderer;

  private void Awake()
  {
    initialZ = transform.position.z;
  }

  private void Start()
  {
    GameController.Instance.OnLevelChange += OnLevelChange;
    // we miss the first event
    //OnLevelChange(this, GameController.Instance.CurrentLevel);
  }

  private void OnDestroy()
  {
    GameController.Instance.OnLevelChange -= OnLevelChange;
  }

  private void OnLevelChange(object ev, Level level)
  {
    if (currenShape)
    {
      currenShape.OnJointLocked -= OnJointLocked;
    }

    currentSequence.Stop();
    currenShape = level.player.GetComponent<Shape>();
    currentStep = 0;
    zSteps.Clear();
    var dist = Mathf.Abs(initialZ - GameController.Instance.PlayerZ);
    currentStepSize = dist / currenShape.Joints.Count;
    for (int i = 1; i <= currenShape.Joints.Count; i++)
    {
      zSteps.Add(initialZ - currentStepSize * i);
    }

    OnJointLocked(this, currenShape.Joints[0]);

    currenShape.OnJointLocked += OnJointLocked;
  }

  public Sequence AnimateToNext(Level level)
  {
    return Sequence.Create()
      .Chain(Tween.PositionZ(this.transform, initialZ + 20f, 2f).OnComplete(() =>
      {
        meshRenderer.material.SetTexture(baseTextureName, level.wallTexture);
      }))
      .Chain(Tween.PositionZ(this.transform, initialZ, 2f));
  }
  
  public Tween AnimateToReset()
  {
    return Tween.PositionZ(transform, initialZ, 2f);
  }

  private void OnJointLocked(object ev, Joint joint)
  {
    var rest = transform.position.z - zSteps[currentStep];
    if (currentStep == zSteps.Count - 1)
    {
      // last step?
      currentSequence = Sequence.Create()
        .Chain(Tween.PositionZ(transform, zSteps[currentStep], rest.remap(0, currentStepSize, 0, 3)))
        .OnComplete(() => { GameController.Instance.NextLevel(); });
    }
    else
    {
      currentSequence = Sequence.Create()
        .Chain(Tween.PositionZ(transform, zSteps[currentStep], rest.remap(0, currentStepSize, 0, 1)))
        .Chain(Tween.PositionZ(transform, zSteps[currentStep + 1], 1f, Ease.OutCirc));

      currentStep++;
    }
  }
}