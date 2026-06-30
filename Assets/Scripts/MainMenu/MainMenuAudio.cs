using UnityEngine;
using UnityEngine.Audio;

public class MainMenuAudio : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer _audioMixer; 

    void Start()
    {
        ApplySavedVolumes();
    }

    private void ApplySavedVolumes()
    {
        if (_audioMixer == null) return;

        float savedMaster = PlayerPrefs.GetFloat("MasterVolumeKey", 1f);
        float savedMusic = PlayerPrefs.GetFloat("MusicVolumeKey", 1f);

        _audioMixer.SetFloat("MasterVol", Mathf.Log10(Mathf.Max(savedMaster, 0.0001f)) * 20f);
        _audioMixer.SetFloat("MusicVol", Mathf.Log10(Mathf.Max(savedMusic, 0.0001f)) * 20f);
    }
}
