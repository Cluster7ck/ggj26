using TMPro;
using UnityEngine;

public class CountPixels : MonoBehaviour
{
    public ComputeShader shader;
    public RenderTexture texA;
    public Texture maskTex; // optional
    public TMP_Text scoreText;

    ComputeBuffer resultBuffer;
    int kernel;

    void Start()
    {
        kernel = shader.FindKernel("CountPixels");
        resultBuffer = new ComputeBuffer(1, sizeof(uint));
    }

    void Update()
    {
        var allPixels = GetBlackPixels(useMask: false);
        var maskedPixels = GetBlackPixels(useMask: true);
        var ratio = maskedPixels / (float)allPixels;

        if (scoreText != null)
        {
            scoreText.text = "Accuracy: " + (int)((1 - ratio) * 100) + "%";
        }
    }

    private uint GetBlackPixels(bool useMask)
    {
        uint[] zero = { 0 };
        resultBuffer.SetData(zero);

        shader.SetTexture(kernel, "TexA", texA);

        if (maskTex != null && useMask)
        {
            shader.SetTexture(kernel, "MaskTex", maskTex);
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