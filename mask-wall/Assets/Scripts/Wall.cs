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
    var dist = Mathf.Abs(initialZ - GameController.Instance.PlayerZ);
    var step = dist / shape.Joints.Count;
    for (int i = 1; i <= shape.Joints.Count; i++)
    {
      zSteps.Add(initialZ + step * i);
    }
  }
}