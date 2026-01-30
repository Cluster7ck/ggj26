using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Joint))]
public class JointEditor : Editor
{
    private void OnSceneGUI()
    {
        Joint joint = (Joint)target;
        Transform t = joint.transform;

        float radius = 0.5f;
        Vector3 center = t.position;
        Vector3 normal = t.forward;

        var angleOffset = 0;
        float offsetLow = joint.constraintLow + angleOffset;
        float offsetHigh = joint.constraintHigh + angleOffset;

        Vector3 from = Quaternion.AngleAxis(offsetLow, normal) * t.up;
        float angle = offsetHigh - offsetLow;

        Handles.color = new Color(1f, 0f, 0.5f, 0.8f);
        Handles.DrawWireArc(center, normal, from, angle, radius);

        // Draw lines from center to arc endpoints
        Handles.color = new Color(1f, 0f, 0.5f, 0.5f);
        Vector3 lowDir = Quaternion.AngleAxis(offsetLow, normal) * t.up;
        Vector3 highDir = Quaternion.AngleAxis(offsetHigh, normal) * t.up;
        Handles.DrawLine(center, center + lowDir * radius);
        Handles.DrawLine(center, center + highDir * radius);
    }
}
