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
    OnLevelChange(this, GameController.Instance.CurrentLevel);
  }

  private void OnDestroy()
  {
    GameController.Instance.OnLevelChange -= OnLevelChange;
  }

  private void OnLevelChange(object ev, Level level)
  {
    meshRenderer.material.SetTexture(baseTextureName, level.wallTexture);
      
    //var resetPost = new Vector3(transform.position.x, transform.position.y, initialZ);
    transform.position = new Vector3(transform.position.x, transform.position.y, initialZ);
    // TODO animate
    currentSequence.Stop();
    if (currenShape)
    {
      currenShape.OnJointLocked -= OnJointLocked;
    }
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

  private void OnJointLocked(object ev, Joint joint)
  {
    var rest = transform.position.z - zSteps[currentStep];
    if (currentStep == zSteps.Count-1)
    {
      // last step?
      currentSequence = Sequence.Create()
        .Chain(Tween.PositionZ(transform, zSteps[currentStep], rest.remap(0, currentStepSize, 0, 3)));
    }
    else
    {
      currentSequence = Sequence.Create()
        .Chain(Tween.PositionZ(transform, zSteps[currentStep], rest.remap(0, currentStepSize, 0, 1)))
        .Chain(Tween.PositionZ(transform, zSteps[currentStep + 1], 10, Ease.OutCirc));
        
      currentStep++;
    }
  }
}