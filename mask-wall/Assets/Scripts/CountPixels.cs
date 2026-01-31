using TMPro;
using UnityEngine;

public class CountPixels : MonoBehaviour
{
    public ComputeShader shader;
    public RenderTexture texA;
    public Texture maskTex;
    
    private TMP_Text scoreText;
    private ComputeBuffer resultBuffer;
    private int kernel;

    void Start()
    {
        kernel = shader.FindKernel("CountPixels");
        resultBuffer = new ComputeBuffer(1, sizeof(uint));
        scoreText = GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        var allPixels = GetBlackPixels(useMask: false);
        var maskedPixels = GetBlackPixels(useMask: true);
        var ratio = maskedPixels / (float)allPixels;
        var accuracyPercent = Mathf.Max((int)((1 - ratio) * 100), 0);

        if (scoreText != null)
        {
            scoreText.text = "Accuracy: " + accuracyPercent + "%";
        }
    }

    private uint GetBlackPixels(bool useMask)
    {
        uint[] zero = { 0 };
        resultBuffer.SetData(zero);

        shader.SetTexture(kernel, "TexA", texA);

        shader.SetTexture(kernel, "MaskTex", maskTex);
        if (maskTex != null && useMask)
        {
            shader.SetInt("UseMask", 1);
        }
        else
        {
            shader.SetInt("UseMask", 0);
        }

        shader.SetInt("Width", texA.width);
        shader.SetInt("Height", texA.height);
        shader.SetBuffer(kernel, "Result", resultBuffer);

        var tx = Mathf.CeilToInt(texA.width / 8.0f);
        var ty = Mathf.CeilToInt(texA.height / 8.0f);

        shader.Dispatch(kernel, tx, ty, 1);

        var result = new uint[1];
        resultBuffer.GetData(result);

        return result[0];
    }

    void OnDestroy()
    {
        resultBuffer.Release();
    }
}