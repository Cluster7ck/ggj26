using UnityEditor;
using UnityEngine;

namespace EditorStuff
{
    [CustomEditor(typeof(CaptureHoleTexture))]
    public class CaptureHoleTextureEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var script = (CaptureHoleTexture)target;

            if (GUILayout.Button("Capture"))
            {
                script.Capture();
            }
        }
    }
}