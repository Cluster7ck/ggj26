using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    public AudioMixerGroup audioMixerGroup;
    public Button soundButton;

    private bool isMuted = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soundButton.onClick.AddListener(ToggleAudio);
    }
    
    void ToggleAudio()
    {
        isMuted = !isMuted;
        var volume = isMuted ? -80f : 0f;
        audioMixerGroup.audioMixer.SetFloat("Volume", volume);

        soundButton.GetComponentInChildren<TMP_Text>().text = isMuted ? "Unmute" : "Mute";
    }
    
}
