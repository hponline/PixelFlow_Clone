using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] Slider audioSlider;

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource bgMusicSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioClip turretShotClip;
    [SerializeField] AudioClip turretSelectClip;
    [SerializeField] AudioClip uiClickClip;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        audioSlider.onValueChanged.AddListener(x => SetMasterVolume(x));

        GameEvent.OnTurretFired += PlayTurretShot;
    }
    private void OnDisable()
    {
        GameEvent.OnTurretFired -= PlayTurretShot;
        
    }

    public void SetMasterVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        audioMixer.SetFloat("MasterVolume", dB);

    }

    public void PlayTurretShot()
    {
        sfxSource.PlayOneShot(turretShotClip);
    }

    public void PlayTurretSelect()
    {
        sfxSource.PlayOneShot(turretSelectClip);
    }

    public void PlayUIClick()
    {
        sfxSource.PlayOneShot(uiClickClip);        
    }
}