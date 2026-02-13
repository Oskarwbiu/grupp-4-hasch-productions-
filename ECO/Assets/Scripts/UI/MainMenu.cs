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

    [SerializeField] Scene intro;
    [SerializeField] LevelExit levelExit;
    [SerializeField] AudioMixer audioMixer;




    private UIDocument _document;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        
    }

    private void Start()
    {

        float volumeOffset = 20f;
        MusicManager.Instance.PlayMusic("Backrooms");

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
        startButton.RegisterCallback<ClickEvent>(evt => MusicManager.Instance.PlayMusic("MainMenu"));
        startButton.RegisterCallback<ClickEvent>(evt => levelExit.StartLevelCoroutine(intro));


        Slider masterVolume = settingsPanel.Q<Slider>("MasterVolume");
        Slider musicVolume = settingsPanel.Q<Slider>("MusicVolume");
        Slider sfxVolume = settingsPanel.Q<Slider>("SFXVolume");

        float masterVol = 0;
        audioMixer.GetFloat("MasterVolume", out masterVol);
        masterVolume.value = masterVol + volumeOffset;
        masterVolume.RegisterCallback<ChangeEvent<float>>(evt => audioMixer.SetFloat("MasterVolume", Mathf.Log10(evt.newValue) * 20));


        float musicVol = 0;
        audioMixer.GetFloat("MusicVolume", out musicVol);
        musicVolume.value = musicVol + volumeOffset;
        musicVolume.RegisterCallback<ChangeEvent<float>>(evt => audioMixer.SetFloat("MusicVolume", Mathf.Log10(evt.newValue) * 20));


        float sfxVol = 0;
        audioMixer.GetFloat("SFXVolume", out sfxVol);
        sfxVolume.value = sfxVol + volumeOffset;
        sfxVolume.RegisterCallback<ChangeEvent<float>>(evt => audioMixer.SetFloat("SFXVolume", Mathf.Log10(evt.newValue) * 20));


        Toggle muteToggle = settingsPanel.Q<Toggle>("MuteToggle");
        muteToggle.value = AudioListener.pause;
        muteToggle.RegisterCallback<ChangeEvent<bool>>(evt => AudioListener.pause = evt.newValue);

        Toggle fullscreenToggle = settingsPanel.Q<Toggle>("FullscreenToggle");
        fullscreenToggle.value = Screen.fullScreen;
        fullscreenToggle.RegisterValueChangedCallback(evt => Screen.fullScreen = evt.newValue);
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
