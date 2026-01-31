using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Joint : MonoBehaviour
{
  public GameObject effectParent;
  public List<LockInEffect> effects = new();
  public LineRenderer linePrefab;
  public bool skipLine;
  public float constraintLow;
  public float constraintHigh;
  public float current;
  public int currentDir = 1;
  private List<(Joint, LineRenderer)> childJoints = new();

  private void Awake()
  {
    string prefix = $"{name}-";
    foreach (Transform child in transform)
    {
      var line = child.GetComponent<LineRenderer>();
      if (line == null) continue;

      if (!child.name.StartsWith(prefix)) continue;

      string jointName = child.name.Substring(prefix.Length);
      var joint = transform.Find(jointName)?.GetComponent<Joint>();
      if (joint != null)
      {
        childJoints.Add((joint, line));
      }
    }

    //effects = effectParent.GetComponentsInChildren<LockInEffect>().ToList();
  }

  public void SpawnEffect()
  {
    //var target = transform.GetComponentInChildrenWithoutSelf<Joint>();
    //var dirToPrev = transform.parent.position - transform.position;
    //var dirToNext = target.transform.position - transform.position;

    //var angle1 = Vector3.Angle(dirToPrev, dirToNext);
    //var angle2 = Vector3.Angle(dirToNext, dirToPrev);
    //var rotNorm = Quaternion.AngleAxis(angle1 / 2, Vector3.forward);
    //Debug.Log("A1: "+angle1+", A2: "+angle2);
    //Debug.DrawRay(transform.position, rotNorm * dirToNext, Color.magenta, 10f);
    //Debug.DrawRay(transform.position, rotNorm * dirToPrev, Color.green, 10f);
    effects = effectParent.GetComponentsInChildren<LockInEffect>(true).ToList();
    if (effects.Count == 0) return;
    var randEffect = effects[Random.Range(0, effects.Count)];
    //Instantiate(randEffect, transform.position + new Vector3())
    randEffect.gameObject.SetActive(true);
  }

  private void Update()
  {
    foreach (var (joint, line) in childJoints)
    {
      line.SetPosition(0, transform.position);
      line.SetPosition(1, joint.transform.position);
    }
  }

  public void StartRot()
  {
    current = transform.localEulerAngles.z;
    constraintLow = constraintLow + current;
    constraintHigh = constraintHigh + current;
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
  
  public static List<T> GetComponentsInChildrenWithoutSelf<T>(this Transform transform) where T : Component
  {
    List<T> list = new List<T>();
    foreach (Transform t in transform)
    {
      var comp = t.GetComponent<T>();
      if (comp != null)
      {
        list.Add(comp);
      }
    }
    return list;
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