using System;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public List<Transform> Joints = new();
    public float rotationSpeed;
    private int currentJoinIdx = 0;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var currentJoint = Joints[currentJoinIdx];
        currentJoint.transform.Rotate(Vector3.forward, 90f * rotationSpeed * Time.deltaTime);
    }
}