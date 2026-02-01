using System;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public List<Joint> Joints = new();
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

    public void Reset()
    {
        var transforms = GetComponentsInChildren<Transform>();
        foreach (var t in transforms)
        {
            t.localRotation = Quaternion.identity;
            var joint = t.GetComponent<Joint>();
            if(joint) joint.current = 0;
        }

        currentJoinIdx = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.Instance.InputAllowed) return;
        
        if (Input.GetKeyDown(KeyCode.Space) && currentJoinIdx < Joints.Count)
        {
            Joints[currentJoinIdx].SpawnEffect();
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
            
            joint.transform.localRotation = Quaternion.Euler(0, 0, mod(joint.current, 360));
        }
    }
    
    public static float mod(float x, float m) {
        return (x%m + m)%m;
    }
}