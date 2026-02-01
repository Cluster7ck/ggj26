using System;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [Header("Textures")] public RenderTexture texA; // Source texture
    public Texture2D maskTex; // Optional black/white mask (0 = ignore)

    [Header("Settings")] [Range(1, 16)] public int downscale = 4; // How much to downscale for faster counting

    private TMP_Text scoreText;
    private Color[] maskPixels;
    private float accuracyPercent = 0f;

    public event EventHandler<int> OnScoreChanged;

    private void Start()
    {
        GameController.Instance.OnLevelChange += OnOnLevelChange;
        //OnOnLevelChange(this, GameController.Instance.CurrentLevel);

        scoreText = GetComponentInChildren<TMP_Text>();
    }

    private void OnOnLevelChange(object sender, Level e)
    {
        GameController.Instance.Shape.OnJointLocked -= ShapeOnOnJointLocked;
        GameController.Instance.Shape.OnJointLocked += ShapeOnOnJointLocked;

        maskTex = e.wallTexture;
        StoreMask();
        CalculateAccuracy();
    }

    private void ShapeOnOnJointLocked(object sender, Joint e)
    {
        CalculateAccuracy();
    }

    private void CalculateAccuracy()
    {
        var allPixels = CountBlackPixels(useMask: false);
        var maskedPixels = CountBlackPixels(useMask: true);
        var ratio = maskedPixels / (float)allPixels;
        var oldAccuracyPercent = accuracyPercent;
        var newAccuracyPercent = Mathf.Max((int)((1 - ratio) * 100), 0);

        // TODO: Tween
        accuracyPercent = newAccuracyPercent;

        if (scoreText != null)
        {
            scoreText.text = "Accuracy: " + accuracyPercent + "%";
        }

        OnScoreChanged?.Invoke(this, newAccuracyPercent);
    }

    private void OnDestroy()
    {
        GameController.Instance.OnLevelChange -= OnOnLevelChange;
        GameController.Instance.Shape.OnJointLocked -= ShapeOnOnJointLocked;
    }

    void StoreMask()
    {
        if (maskTex == null)
        {
            return;
        }

        int w = maskTex.width / downscale;
        int h = maskTex.height / downscale;

        // w, h = target downscaled size
        Texture2D maskSmall = new Texture2D(w, h, TextureFormat.RGBA32, false);

// Create a temporary render texture using a guaranteed-supported format
        RenderTexture rtMaskSmall = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32);

// Blit the original mask into the RT (this handles downscaling)
        Graphics.Blit(maskTex, rtMaskSmall);

// Read the pixels from the RT into the Texture2D
        RenderTexture.active = rtMaskSmall;
        maskSmall.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        maskSmall.Apply();
        RenderTexture.active = null;

// Release the temporary RT
        RenderTexture.ReleaseTemporary(rtMaskSmall);

// Now you can access the mask pixels safely
        maskPixels = maskSmall.GetPixels(); // use maskPixels[i].r for black/white check
    }

    int CountBlackPixels(bool useMask)
    {
        // 1. Downscale into a temporary small texture to speed up counting
        int w = texA.width / downscale;
        int h = texA.height / downscale;

        RenderTexture rtSmall = RenderTexture.GetTemporary(w, h, 0, texA.format);
        Graphics.Blit(texA, rtSmall);

        // 2. Read into a Texture2D
        Texture2D texSmall = new Texture2D(w, h, TextureFormat.RGBA32, false);
        RenderTexture.active = rtSmall;
        texSmall.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        texSmall.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rtSmall);

        Color[] pixels = texSmall.GetPixels();

        // 3. Count black pixels
        int count = 0;
        for (int i = 0; i < pixels.Length; i++)
        {
            // Mask check
            if (useMask && maskPixels != null && maskPixels[i].grayscale < 0.1f)
                continue;

            // Black check
            Color c = pixels[i];
            if (Mathf.Approximately(c.r, 1.0f) && Mathf.Approximately(c.g, 0.0f) && Mathf.Approximately(c.b, 1.0f))
                continue;

            count++;
        }

        return count;
    }

    /*void Update()
    {
        var allPixels = GetBlackPixels(useMask: false);
        var maskedPixels = GetBlackPixels(useMask: true);
        var ratio = maskedPixels / (float)allPixels;
        var accuracyPercent = Mathf.Max((int)((1 - ratio) * 100), 0);

        if (scoreText != null)
        {
            scoreText.text = "Accuracy: " + accuracyPercent + "%";
        }
    }*/
}