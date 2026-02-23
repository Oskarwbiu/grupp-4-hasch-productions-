using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UIElements.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private UIDocument pauseDocument;

    private VisualElement pauseVE;

    private Button resumeButton;
    private Button settingsButton;
    private Button mainMenuButton;
    private Button exitButton;

    private VisualElement pauseMenu;
    private VisualElement settingsMenu;
    
    PlayerCheats cheatsScript;
    PlayerInput playerInput;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] UIDocument HUD;


    private void Awake()
    {

        float volumeOffset = 20f;
        pauseDocument = GetComponent<UIDocument>();
        pauseVE = pauseDocument.rootVisualElement as VisualElement;

        VisualElement root = pauseDocument.rootVisualElement;
        pauseMenu = root.Q<VisualElement>("PauseMenu");
        settingsMenu = root.Q<VisualElement>("SettingsMenu");
        

        VisualElement image = pauseMenu.Q<VisualElement>();
        VisualElement settingsRoot = settingsMenu.Q<VisualElement>();

        VisualElement settingsPanel = settingsRoot.Q<VisualElement>().Q<VisualElement>();

        resumeButton = image.Q<Button>("Resume");
        settingsButton = image.Q<Button>("Settings");
        mainMenuButton = image.Q<Button>("MainMenu");
        exitButton = settingsPanel.Q<Button>();

        resumeButton.RegisterCallback<ClickEvent>(evt => ResumeGame());
        settingsButton.RegisterCallback<ClickEvent>(evt => EnableSettings());
        mainMenuButton.RegisterCallback<ClickEvent>(evt => LoadMainMenu());
        exitButton.RegisterCallback<ClickEvent>(evt => ExitSettings());


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



        Toggle godmodeToggle = settingsPanel.Q<Toggle>("GodmodeToggle");
        godmodeToggle.RegisterValueChangedCallback(evt =>
        {
            PlayerCheats cheats = GetCurrentCheatsScript();
            if (cheats != null)
                cheats.isGodMode = evt.newValue;
        });

        Toggle noClipToggle = settingsPanel.Q<Toggle>("NoClipToggle");
        noClipToggle.RegisterValueChangedCallback(evt =>
        {
            PlayerCheats cheats = GetCurrentCheatsScript();
            if (cheats != null)
                cheats.NoClip(evt.newValue);
        });

        float flySpeed = 0;
        Slider flySpeedSlider = settingsPanel.Q<Slider>("SpeedSlider");
        flySpeedSlider.value = flySpeed;
        flySpeedSlider.RegisterCallback<ChangeEvent<float>>((evt =>
        {
            PlayerCheats cheats = GetCurrentCheatsScript();
            if (cheats != null)
                cheats.flySpeed = evt.newValue;
        }));

        Toggle levelSkipToggle = settingsPanel.Q<Toggle>("LevelSkipToggle");
        levelSkipToggle.RegisterValueChangedCallback(evt =>
        {
            PlayerCheats cheats = GetCurrentCheatsScript();
            if (cheats != null)
                cheats.canLevelskip = evt.newValue;
        });

        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    

    private void Start()
    {
        pauseDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        cheatsScript = null;
        playerInput = null;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private PlayerCheats GetCurrentCheatsScript()
    {
        if (cheatsScript == null)
        {
            cheatsScript = FindFirstObjectByType<PlayerCheats>();
        }
        return cheatsScript;
    }

   
    private PlayerInput GetCurrentPlayerInput()
    {
        if (playerInput == null)
        {
            playerInput = FindFirstObjectByType<PlayerInput>();
        }
        return playerInput;
    }


    public void Pause()
    {
        HUD.rootVisualElement.style.display = DisplayStyle.None;
        PlayerInput pi = GetCurrentPlayerInput();
        if (pi != null)
            pi.enabled = false;
        pauseDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        Time.timeScale = 0;
    }

    void ResumeGame()
    {
        HUD.rootVisualElement.style.display = DisplayStyle.Flex;
        PlayerInput pi = GetCurrentPlayerInput();
        if (pi != null)
            pi.enabled = true;
        pauseDocument.rootVisualElement.style.display = DisplayStyle.None;
        Time.timeScale = 1;
        pauseVE.Blur();
    }

    void EnableSettings()
    {
        pauseMenu.style.display = DisplayStyle.None;
        settingsMenu.style.display = DisplayStyle.Flex;
    }

    void ExitSettings()
    {
        pauseMenu.style.display = DisplayStyle.Flex;
        settingsMenu.style.display = DisplayStyle.None;
    }
    void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)Level.MAINMENU);
        FindFirstObjectByType<PlayerHealth>().ResetHealth();
        Time.timeScale = 1;
    }
}
