using System;
using UnityEngine;

public class Joint : MonoBehaviour
{
  public LineRenderer linePrefab;
  public bool skipLine;
  private Joint jointChild;
  public float constraintLow;
  public float constraintHigh;
  public float angleOffset;
  public float current;
  public int currentDir = 1;
  private LineRenderer line;

  public void Start()
  {
    angleOffset = GetAngleOffset();
    jointChild = transform.GetComponentInChildrenWithoutSelf<Joint>();
    if (!skipLine && jointChild != null)
    {
      line = Instantiate(linePrefab, this.transform);
      line.positionCount = 2;
    }
  }

  private void Update()
  {
    if (line != null)
    {
      line.SetPosition(0, transform.position);
      line.SetPosition(1, jointChild.transform.position);
    }
  }

  public void StartRot()
  {
    current = transform.localEulerAngles.z;
    constraintLow = constraintLow + current;
    constraintHigh = constraintHigh + current;
  }

  public float GetAngleOffset()
  {
    jointChild = transform.GetComponentInChildrenWithoutSelf<Joint>();

    if (jointChild != null)
    {
      var toJointAngle = jointChild.transform.position - transform.position;
      return Vector3.Angle(transform.up, toJointAngle);
    }

    return 0;
  }

  public (float low, float high) GetRemappedAngles()
  {
    if (constraintLow < 0)
    {
      return (360 - constraintLow, constraintHigh);
    }

    if (constraintHigh > 360)
    {
      return (constraintLow, constraintHigh - 360);
    }

    return (constraintLow, constraintHigh);
  }
}

public static class Extensions
{
  public static T GetComponentInChildrenWithoutSelf<T>(this GameObject go) where T : Component
  {
    return go.transform.GetComponentInChildrenWithoutSelf<T>();
  }
  
  public static T GetComponentInChildrenWithoutSelf<T>(this Transform transform) where T : Component
  {
    foreach (Transform t in transform)
    {
      var comp = t.GetComponent<T>();
      if (comp != null) return comp;
    }
    return null;
  }
  
  public static float remap(this float s, float a1, float a2, float b1, float b2)
  {
    return b1 + (s-a1)*(b2-b1)/(a2-a1);
  }
  
  public static float remap01(this float s, float a1, float a2)
  {
    return (s-a1)/(a2-a1);
  }
}