using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Shape))]
public class ShapeEditor : Editor
{
    [MenuItem("CONTEXT/Shape/Draw All Lines (Undoable)")]
    private static void DrawAllLinesUndoable(MenuCommand command)
    {
        var shape = (Shape)command.context;
        var joints = shape.GetComponentsInChildren<Joint>();

        Undo.SetCurrentGroupName("Draw All Lines");
        int undoGroup = Undo.GetCurrentGroup();

        foreach (var joint in joints)
        {
            DrawLinesForJoint(joint);
        }

        Undo.CollapseUndoOperations(undoGroup);
    }

    private static void DrawLinesForJoint(Joint joint)
    {
        if (joint.skipLine || joint.linePrefab == null) return;

        var childJoints = joint.transform.GetComponentsInChildrenWithoutSelf<Joint>();
        foreach (var childJoint in childJoints)
        {
            string lineName = $"{joint.name}-{childJoint.name}";

            // Check if line already exists
            Transform existing = joint.transform.Find(lineName);
            if (existing != null) continue;

            var line = (LineRenderer)PrefabUtility.InstantiatePrefab(joint.linePrefab, joint.transform);
            line.name = lineName;
            line.positionCount = 2;
            line.SetPosition(0, joint.transform.position);
            line.SetPosition(1, childJoint.transform.position);

            Undo.RegisterCreatedObjectUndo(line.gameObject, "Create Line");
        }
    }
}
