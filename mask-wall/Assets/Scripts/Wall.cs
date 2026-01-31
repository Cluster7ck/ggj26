using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
  private float initialZ;
  private List<float> zSteps = new();

  private void Awake()
  {
    initialZ = transform.position.z;
  }

  public void Reset()
  {
    var resetPost = new Vector3(transform.position.x, transform.position.y, initialZ);
    transform.position = new Vector3(transform.position.x, transform.position.y, initialZ);
  }
}