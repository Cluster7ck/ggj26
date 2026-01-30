using UnityEngine;

using UnityEngine;
using System.IO;

using UnityEngine;
using System.IO;

using UnityEngine;
using System.IO;

public class CameraToAlphaCapture : MonoBehaviour
{
    [Header("Capture Settings")]
    public Camera targetCamera;
    public Vector2Int resolution = new Vector2Int(1024, 1024);
    public string fileName = "AlphaCapture_Hard.png";

    [Header("Processing")]
    [Tooltip("How far the blur spreads (1 = 3x3, 2 = 5x5, etc.)")]
    [Range(0, 20)] public int blurRadius = 2; 
    
    [Tooltip("Brightness threshold: 1.0 is pure white, 0.5 is mid-gray.")]
    [Range(0.01f, 1f)] public float whiteThreshold = 0.9f;

    [ContextMenu("Capture to White-Alpha (Hard)")]
    public void Capture()
    {
        if (targetCamera == null)
        {
            Debug.LogError("Please assign a Target Camera!");
            return;
        }

        // 1. Render to Texture
        RenderTexture rt = new RenderTexture(resolution.x, resolution.y, 24);
        targetCamera.targetTexture = rt;
        targetCamera.Render();

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(resolution.x, resolution.y, TextureFormat.RGBA32, false);
        screenShot.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);
        screenShot.Apply();

        // 2. Apply Radius-based Blur (only if radius > 0)
        if (blurRadius > 0)
        {
            screenShot = ApplyRadiusBlur(screenShot, blurRadius);
        }

        // 3. Process Pixels (Hard Threshold)
        Color[] pixels = screenShot.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            // If brightness is above threshold, keep it white/opaque
            if (pixels[i].grayscale >= whiteThreshold)
            {
                pixels[i] = Color.white;
            }
            else
            {
                // Otherwise, make it fully transparent white
                pixels[i] = new Color(0, 0, 0, 1);
            }
        }

        screenShot.SetPixels(pixels);
        screenShot.Apply();

        // 4. Save and Cleanup
        SaveToFile(screenShot);
        
        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);
        DestroyImmediate(screenShot);
    }

    private Texture2D ApplyRadiusBlur(Texture2D source, int radius)
    {
        int w = source.width;
        int h = source.height;
        Texture2D blurred = new Texture2D(w, h, source.format, false);
        Color[] src = source.GetPixels();
        Color[] dst = new Color[src.Length];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float r = 0, g = 0, b = 0, a = 0;
                int count = 0;

                // Look at neighbors within the radius
                for (int ky = -radius; ky <= radius; ky++)
                {
                    for (int kx = -radius; kx <= radius; kx++)
                    {
                        int nx = Mathf.Clamp(x + kx, 0, w - 1);
                        int ny = Mathf.Clamp(y + ky, 0, h - 1);
                        
                        Color c = src[ny * w + nx];
                        r += c.r; g += c.g; b += c.b; a += c.a;
                        count++;
                    }
                }
                dst[y * w + x] = new Color(r / count, g / count, b / count, a / count);
            }
        }

        blurred.SetPixels(dst);
        blurred.Apply();
        return blurred;
    }

    private void SaveToFile(Texture2D tex)
    {
        byte[] bytes = tex.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllBytes(path, bytes);
        Debug.Log($"<b>Success:</b> Asset created at {path}");
        
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}