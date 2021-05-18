using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    public AudioMixer mainMixer;

    [Header("Sliders")]
    public Slider mainSlider;
    public Slider musicSlider;
    public Slider ambientSlider;
    public Slider sfxSlider;

    public List<AudioSource> listeMusique;
    public List<AudioSource> listeSFX;
    public List<AudioSource> listeAmbient;

    float volumeMain;
    float volumeAmbient;
    float volumeSFX;
    float volumeMusic;


    // Start is called before the first frame update
    void Start()
    {
        volumeMain = PlayerPrefs.GetFloat("volume_main", 1f);
        volumeAmbient = PlayerPrefs.GetFloat("volume_ambient", 1f);
        volumeSFX = PlayerPrefs.GetFloat("volume_sfx", 1f);
        volumeMusic = PlayerPrefs.GetFloat("volume_music", 1f);


        

        SliderSetup(mainSlider, volumeMain);
        SliderSetup(musicSlider, volumeMusic);
        SliderSetup(ambientSlider, volumeAmbient);
        SliderSetup(sfxSlider, volumeSFX);


        mainSlider.onValueChanged.AddListener(sliderVolMaster_onValueChanged);
        musicSlider.onValueChanged.AddListener(sliderVolMusic_onValueChanged);
        ambientSlider.onValueChanged.AddListener(sliderVolAmbient_onValueChanged);
        sfxSlider.onValueChanged.AddListener(sliderVolSFX_onValueChanged);
    }

	/// <summary>
	/// Change le volume selon la valeur du slider
	/// </summary>
	/// <param name="paramName">La catégorie de son à changer de volume</param>
	/// <param name="value">Le nouveau volume</param>
    void setVolume(string paramName, float value)
    {
        mainMixer.SetFloat(paramName, Mathf.Log(value) * 20f);

        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (paramName == "volMain")
            {
                foreach (AudioSource audio in listeMusique)
                {
                    audio.volume = value;
                }
                foreach (AudioSource audio in listeSFX)
                {
                    audio.volume = value;
                }
                foreach (AudioSource audio in listeAmbient)
                {
                    audio.volume = value;
                }
            }
            else if (paramName == "volMusic")
            {
                foreach (AudioSource audio in listeMusique)
                {
                    audio.volume = value;
                }
            }
            else if (paramName == "volSFX")
            {
                foreach (AudioSource audio in listeSFX)
                {
                    audio.volume = value;
                }
            }
            else if (paramName == "volAmbient")
            {
                foreach (AudioSource audio in listeAmbient)
                {
                    audio.volume = value;
                }
            }
        }
        
    }

	/// <summary>
	/// Enregistre le volume dans le cache
	/// </summary>
    public void sauvegarderSon()
    {
        PlayerPrefs.SetFloat("volume_main", volumeMain);
        PlayerPrefs.SetFloat("volume_ambient", volumeAmbient);
        PlayerPrefs.SetFloat("volume_sfx", volumeSFX);
        PlayerPrefs.SetFloat("volume_music", volumeMusic);

        PlayerPrefs.Save();
    }

	//----------Les différents sliders de volume-----------------

    void sliderVolMaster_onValueChanged(float value)
    {
        setVolume("volMain", value);
        volumeMain = value;
    }

    void sliderVolMusic_onValueChanged(float value)
    {
        setVolume("volMusic", value);
        volumeMusic = value;
    }

    void sliderVolAmbient_onValueChanged(float value)
    {
        setVolume("volAmbient", value);
        volumeAmbient = value;
    }

    void sliderVolSFX_onValueChanged(float value)
    {
        setVolume("volSFX", value);
        volumeSFX = value;
    }


	/// <summary>
	/// Setup la valeur des sliders selon le volume du son
	/// </summary>
	/// <param name="volumeSlider">Le slider à changer</param>
	/// <param name="valueIntiale">Le volume</param>
    void SliderSetup(Slider volumeSlider, float valueIntiale)
    {
        volumeSlider.minValue = 0.001f;
        volumeSlider.maxValue = 1.6f;

        volumeSlider.value = valueIntiale;
    }
}
