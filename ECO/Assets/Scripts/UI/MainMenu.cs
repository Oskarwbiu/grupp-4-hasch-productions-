using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{


    private Button exitButton;
    private Button settingsButton;

    private VisualElement settingsMenu;
    private VisualElement mainMenu;

    [SerializeField] Level intro;
    [SerializeField] LevelExit levelExit;
    [SerializeField] AudioMixer audioMixer;




    private UIDocument _document;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        PlayerPrefs.DeleteKey("GodMode");
        PlayerPrefs.DeleteKey("LevelSkip");
        PlayerPrefs.DeleteKey("NoClip");
        PlayerPrefs.DeleteKey("FlySpeed");
    }

    private void Start()
    {

        float volumeOffset = 20f;

        VisualElement root = _document.rootVisualElement;
        settingsMenu = root.Q<VisualElement>("SettingsMenu");
        VisualElement settingsRoot = settingsMenu.Q<VisualElement>();
        mainMenu = root.Q<VisualElement>("MainMenu");
        VisualElement settingsPanel = settingsRoot.Q<VisualElement>().Q<VisualElement>();


        settingsButton = mainMenu.Q<Button>("SettingsButton");
        exitButton = settingsPanel.Q<Button>();

        settingsButton.RegisterCallback<ClickEvent>(evt => EnableSettings());
        exitButton.RegisterCallback<ClickEvent>(evt => ExitSettings());




        


        Button quitButton = root.Q<Button>("QuitButton");
        quitButton.RegisterCallback<ClickEvent>(evt => Application.Quit());

        Button startButton = root.Q<Button>("StartButton");
        startButton.RegisterCallback<ClickEvent>(evt => Time.timeScale = 1);
        startButton.RegisterCallback<ClickEvent>(evt => levelExit.StartLevelCoroutine(intro));
        startButton.RegisterCallback<ClickEvent>(evt => PlayerPrefs.DeleteKey("CheckpointID"));

        Button continueButton = root.Q<Button>("ContinueButton");
        continueButton.RegisterCallback<ClickEvent>(evt => levelExit.StartLevelCoroutine((Level)PlayerPrefs.GetInt("Level")));
        continueButton.RegisterCallback<ClickEvent>(evt => FindFirstObjectByType<SettingsManager>().continued = true);
        continueButton.RegisterCallback<ClickEvent>(evt => Time.timeScale = 1);


        Slider masterVolume = settingsPanel.Q<Slider>("MasterVolume");
        Slider musicVolume = settingsPanel.Q<Slider>("MusicVolume");
        Slider sfxVolume = settingsPanel.Q<Slider>("SFXVolume");

        float masterVol = 0;
        audioMixer.GetFloat("MasterVolume", out masterVol);
        masterVolume.value = masterVol + volumeOffset;
        masterVolume.RegisterCallback<ChangeEvent<float>>(evt => audioMixer.SetFloat("MasterVolume", Mathf.Log10(evt.newValue) * 20));
        masterVolume.RegisterCallback<ChangeEvent<float>>(evt => PlayerPrefs.SetFloat("MasterVolume", Mathf.Log10(evt.newValue) * 20));


        float musicVol = 0;
        audioMixer.GetFloat("MusicVolume", out musicVol);
        musicVolume.value = musicVol + volumeOffset;
        musicVolume.RegisterCallback<ChangeEvent<float>>(evt => audioMixer.SetFloat("MusicVolume", Mathf.Log10(evt.newValue) * 20));
        musicVolume.RegisterCallback<ChangeEvent<float>>(evt => PlayerPrefs.SetFloat("MusicVolume", Mathf.Log10(evt.newValue) * 20));


        float sfxVol = 0;
        audioMixer.GetFloat("SFXVolume", out sfxVol);
        sfxVolume.value = sfxVol + volumeOffset;
        sfxVolume.RegisterCallback<ChangeEvent<float>>(evt => audioMixer.SetFloat("SFXVolume", Mathf.Log10(evt.newValue) * 20));
        sfxVolume.RegisterCallback<ChangeEvent<float>>(evt => PlayerPrefs.SetFloat("SFXVolume", Mathf.Log10(evt.newValue) * 20));


        Toggle muteToggle = settingsPanel.Q<Toggle>("MuteToggle");
        muteToggle.RegisterValueChangedCallback(evt => PlayerPrefs.SetInt("Mute", evt.newValue ? 1 : 0));
        muteToggle.value = AudioListener.pause;
        muteToggle.RegisterCallback<ChangeEvent<bool>>(evt => AudioListener.pause = evt.newValue);

        Toggle fullscreenToggle = settingsPanel.Q<Toggle>("FullscreenToggle");
        fullscreenToggle.RegisterCallback<ChangeEvent<bool>>(evt => PlayerPrefs.SetInt("Fullscreen", evt.newValue ? 1 : 0));
        fullscreenToggle.value = Screen.fullScreen;
        fullscreenToggle.RegisterValueChangedCallback(evt => Screen.fullScreen = evt.newValue);

        Toggle godModeToggle = settingsPanel.Q<Toggle>("GodmodeToggle");
        godModeToggle.RegisterCallback<ChangeEvent<bool>>(evt => PlayerPrefs.SetInt("GodMode", evt.newValue ? 1 : 0));
        godModeToggle.value = PlayerPrefs.GetInt("GodMode", 0) == 1;

        Toggle levelSkipToggle = settingsPanel.Q<Toggle>("LevelSkipToggle");
        levelSkipToggle.RegisterCallback<ChangeEvent<bool>>(evt => PlayerPrefs.SetInt("LevelSkip", evt.newValue ? 1 : 0));
        levelSkipToggle.value = PlayerPrefs.GetInt("LevelSkip", 0) == 1;

        Toggle noClipToggle = settingsPanel.Q<Toggle>("NoClipToggle");
        noClipToggle.RegisterCallback<ChangeEvent<bool>>(evt => PlayerPrefs.SetInt("NoClip", evt.newValue ? 1 : 0));
        noClipToggle.value = PlayerPrefs.GetInt("NoClip", 0) == 1;

        Slider flySpeedSlider = settingsPanel.Q<Slider>("SpeedSlider");
        flySpeedSlider.RegisterCallback<ChangeEvent<float>>(evt => PlayerPrefs.SetFloat("FlySpeed", evt.newValue));
        flySpeedSlider.value = PlayerPrefs.GetFloat("FlySpeed", 5f);

    }



    void EnableSettings()
    {
        mainMenu.style.display = DisplayStyle.None;
        settingsMenu.style.display = DisplayStyle.Flex;
    }

    void ExitSettings()
    {
        mainMenu.style.display = DisplayStyle.Flex;
        settingsMenu.style.display = DisplayStyle.None;
    }
}
