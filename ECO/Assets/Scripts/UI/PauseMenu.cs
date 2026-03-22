using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UIElements.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused;
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

        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 1); ;
        audioMixer.SetFloat("MasterVolume", masterVol);
        masterVolume.value = Mathf.Pow(10f, masterVol/ 20f);
        masterVolume.RegisterCallback<ChangeEvent<float>>(evt => audioMixer.SetFloat("MasterVolume", Mathf.Log10(evt.newValue) * 20));
        masterVolume.RegisterCallback<ChangeEvent<float>>(evt => PlayerPrefs.SetFloat("MasterVolume", Mathf.Log10(evt.newValue) * 20));


        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1); ;
        audioMixer.SetFloat("MusicVolume", musicVol);
        musicVolume.value = Mathf.Pow(10f, musicVol / 20f);
        musicVolume.RegisterCallback<ChangeEvent<float>>(evt => audioMixer.SetFloat("MusicVolume", Mathf.Log10(evt.newValue) * 20));
        musicVolume.RegisterCallback<ChangeEvent<float>>(evt => PlayerPrefs.SetFloat("MusicVolume", Mathf.Log10(evt.newValue) * 20));


        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1);
        audioMixer.SetFloat("SFXVolume", sfxVol);
        sfxVolume.value = Mathf.Pow(10f, sfxVol / 20f);
        sfxVolume.RegisterCallback<ChangeEvent<float>>(evt => audioMixer.SetFloat("SFXVolume", Mathf.Log10(evt.newValue) * 20));
        sfxVolume.RegisterCallback<ChangeEvent<float>>(evt => PlayerPrefs.SetFloat("SFXVolume", Mathf.Log10(evt.newValue) * 20));



        Toggle muteToggle = settingsPanel.Q<Toggle>("MuteToggle");
        muteToggle.value = AudioListener.pause;
        muteToggle.RegisterCallback<ChangeEvent<bool>>(evt => AudioListener.pause = evt.newValue);
        muteToggle.RegisterCallback<ChangeEvent<bool>>(evt => PlayerPrefs.SetInt("Mute", evt.newValue ? 1 : 0));

        Toggle fullscreenToggle = settingsPanel.Q<Toggle>("FullscreenToggle");
        fullscreenToggle.value = Screen.fullScreen;
        fullscreenToggle.RegisterValueChangedCallback(evt => Screen.fullScreen = evt.newValue);
        fullscreenToggle.RegisterValueChangedCallback(evt => PlayerPrefs.SetInt("Fullscreen", evt.newValue ? 1 : 0));



        Toggle godmodeToggle = settingsPanel.Q<Toggle>("GodmodeToggle");
        godmodeToggle.value = PlayerPrefs.GetInt("GodMode", 0) == 1;
        godmodeToggle.RegisterValueChangedCallback(evt => PlayerPrefs.SetInt("GodMode", evt.newValue ? 1 : 0));
        godmodeToggle.RegisterValueChangedCallback(evt =>
        {
            PlayerCheats cheats = GetCurrentCheatsScript();
            if (cheats != null)
                cheats.isGodMode = evt.newValue;
        });

        Toggle noClipToggle = settingsPanel.Q<Toggle>("NoClipToggle");
        noClipToggle.value = PlayerPrefs.GetInt("NoClip", 0) == 1;
        noClipToggle.RegisterValueChangedCallback(evt => PlayerPrefs.SetInt("NoClip", evt.newValue ? 1 : 0));
        noClipToggle.RegisterValueChangedCallback(evt =>
        {
            PlayerCheats cheats = GetCurrentCheatsScript();
            if (cheats != null)
                cheats.NoClip(evt.newValue);
        });

        float flySpeed = PlayerPrefs.GetFloat("FlySpeed", 0); ;
        Slider flySpeedSlider = settingsPanel.Q<Slider>("SpeedSlider");
        flySpeedSlider.value = flySpeed;
        flySpeedSlider.RegisterCallback<ChangeEvent<float>>(evt => PlayerPrefs.SetFloat("FlySpeed", evt.newValue));
        flySpeedSlider.RegisterCallback<ChangeEvent<float>>((evt =>
        {
            PlayerCheats cheats = GetCurrentCheatsScript();
            if (cheats != null)
                cheats.flySpeed = evt.newValue;
        }));

        Toggle levelSkipToggle = settingsPanel.Q<Toggle>("LevelSkipToggle");
        levelSkipToggle.value = PlayerPrefs.GetInt("LevelSkip", 0) == 1;
        levelSkipToggle.RegisterValueChangedCallback(evt => PlayerPrefs.SetInt("LevelSkip", evt.newValue ? 1 : 0));
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
        ResumeGame();
        cheatsScript = null;
        playerInput = null;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus && isPaused)
        {
            Pause();
        }
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
        isPaused = true;
        PlayerInput pi = GetCurrentPlayerInput();
        if (pi != null)
            pi.enabled = false;
        pauseDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        Time.timeScale = 0;
    }

    void ResumeGame()
    {
        isPaused = false;
        PlayerPrefs.Save();
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
        Transform playerPos = GameObject.FindWithTag("Player").transform;
        PlayerPrefs.SetFloat("PlayerPosX", playerPos.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", playerPos.position.y);
        
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)Level.MAINMENU);
        Time.timeScale = 1;
    }
}
