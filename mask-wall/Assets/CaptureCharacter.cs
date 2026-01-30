using UnityEngine;

public class CaptureCharacter : MonoBehaviour
{
    public MeshRenderer wallRenderer;
    public Camera targetCamera;
    public Vector2Int resolution = new Vector2Int(1024, 1024);

    private Material wallMaterial;
    private Texture2D wallTexture;
    
    void Start()
    {
        wallMaterial = wallRenderer.material;
        wallTexture = new Texture2D(resolution.x, resolution.y, TextureFormat.RGBA32, false);
    }
    
    void Update()
    {
        if (targetCamera == null)
        {
            Debug.LogError("Please assign a Target Camera!");
            return;
        }
        
        var renderTexture = new RenderTexture(resolution.x, resolution.y, 24);
        targetCamera.targetTexture = renderTexture;
        targetCamera.Render();
        RenderTexture.active = renderTexture; 
        
        wallTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        wallTexture.Apply();
        
        wallMaterial.SetTexture("Camera", wallTexture);
    }
}
