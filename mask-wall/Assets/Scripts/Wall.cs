using System;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
  private static readonly int baseTextureName = Shader.PropertyToID("Base");
  private float initialZ;
  private List<float> zSteps = new();

  private MeshRenderer meshRenderer;

  private void Awake()
  {
    meshRenderer = GetComponent<MeshRenderer>();
    initialZ = transform.position.z;
    GameController.Instance.OnLevelChange += OnLevelChange;
  }

  private void OnDestroy()
  {
    GameController.Instance.OnLevelChange -= OnLevelChange;
  }

  public void OnLevelChange(object ev, Level level)
  {
    meshRenderer.material.SetTexture(baseTextureName, level.wallTexture);
      
    var resetPost = new Vector3(transform.position.x, transform.position.y, initialZ);
    transform.position = new Vector3(transform.position.x, transform.position.y, initialZ);
    var shape = level.player.GetComponent<Shape>();
    zSteps.Clear();
    //var dist = initialZ vw
    for (int i = 0; i < shape.Joints.Count; i++)
    {
    }
    
  }
}