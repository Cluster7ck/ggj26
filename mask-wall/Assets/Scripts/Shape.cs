using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public List<Joint> Joints = new();
    public Joint RootJoint;
    public float rotationSpeed;
    private int currentJoinIdx = 0;
    private int currentDir = 1;

    public event EventHandler<Joint> OnJointLocked;
    
    void Start()
    {
        if (Joints.Count > 0 && currentJoinIdx < Joints.Count)
        {
            Joints[currentJoinIdx].StartRot();
        }
    }

    private void DoTheThing(Transform parent)
    {
        foreach (Transform child in parent)
        {
            var comp = child.GetComponent<Joint>();
            if (comp != null)
            {
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && currentJoinIdx < Joints.Count)
        {
            OnJointLocked?.Invoke(this, Joints[currentJoinIdx]);

            currentJoinIdx++;
            if (Joints.Count > 0 && currentJoinIdx < Joints.Count)
            {
                Joints[currentJoinIdx].StartRot();
            }
        }
        
        if (Joints.Count > 0 && currentJoinIdx < Joints.Count)
        {
            var joint = Joints[currentJoinIdx];
            //joint.transform.Rotate(Vector3.forward, currentDir * 90f * rotationSpeed * Time.deltaTime);

            joint.current += joint.currentDir * 90f * rotationSpeed * Time.deltaTime;

            if (joint.current < joint.constraintLow)
            {
                joint.currentDir = -1 * joint.currentDir;
                joint.current = joint.constraintLow;
            } else if (joint.current > joint.constraintHigh)
            {
                joint.currentDir = -1 * joint.currentDir;
                joint.current = joint.constraintHigh;
            }
            
            //Debug.Log("current: "+joint.current+ " ,, mod: "+mod(joint.current, 360));
            joint.transform.localRotation = Quaternion.Euler(0, 0, mod(joint.current, 360));
            
            //var asd = joint.current.remap(joint.constraintLow, joint.constraintHigh, 0, )
        }
    }
    
    public static float mod(float x, float m) {
        return (x%m + m)%m;
    }
}