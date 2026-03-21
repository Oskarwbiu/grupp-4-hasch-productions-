using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioSource sfxAudioSource;
    [SerializeField] AudioMixer audioMixer;

    public bool continued = false;
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Load settings from PlayerPrefs
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        bool isMuted = PlayerPrefs.GetInt("Mute", 0) == 1;
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 0) == 1;
        float flySpeed = PlayerPrefs.GetFloat("FlySpeed", 5f);
        bool isGodMode = PlayerPrefs.GetInt("GodMode", 0) == 1;
        bool noClip = PlayerPrefs.GetInt("NoClip", 0) == 1;
        bool levelSkip = PlayerPrefs.GetInt("LevelSkip", 0) == 1;

        // Apply loaded settings
        audioMixer.SetFloat("MasterVolume", masterVolume);
        musicAudioSource.volume = musicVolume;
        sfxAudioSource.volume = sfxVolume;
        AudioListener.pause = isMuted;
        Screen.fullScreen = isFullscreen;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != (int)Level.MAINMENU)
        {
            PlayerPrefs.SetInt("Level", scene.buildIndex);
        }
        // Re-apply settings when a new scene is loaded
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);    
        bool isMuted = PlayerPrefs.GetInt("Mute", 0) == 1;
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 0) == 1;


        Debug.Log($"Volume {masterVolume}");
        Debug.Log($"Music {musicVolume}");
        Debug.Log($"SFX {sfxVolume}");
        Debug.Log($"Mute: {isMuted}");


        audioMixer.SetFloat("MasterVolume", masterVolume);
        musicAudioSource.volume = Mathf.Pow(10f, musicVolume / 20f); ;
        sfxAudioSource.volume = Mathf.Pow(10f, sfxVolume / 20f); ;
        AudioListener.pause = isMuted;
        Screen.fullScreen = isFullscreen;
        StartCoroutine(ApplyCheats());
    }

    IEnumerator ApplyCheats()
    {
        PlayerCheats cheats = null;

        while (cheats == null)
        {
            cheats = FindFirstObjectByType<PlayerCheats>();
            yield return null;
            yield return null;
        }

        if (cheats != null)
        {
            float flySpeed = PlayerPrefs.GetFloat("FlySpeed", 5f);
            bool isGodMode = PlayerPrefs.GetInt("GodMode", 0) == 1;
            bool noClip = PlayerPrefs.GetInt("NoClip", 0) == 1;
            bool levelSkip = PlayerPrefs.GetInt("LevelSkip", 0) == 1;

            cheats.flySpeed = flySpeed;
            cheats.GodMode(isGodMode);
            cheats.NoClip(noClip);
            cheats.canLevelskip = levelSkip;
        }
        Debug.Log("cheats = " + cheats);

        if (continued)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                int checkpointID = PlayerPrefs.GetInt("CheckpointID", -1);
                if (checkpointID != -1)
                {
                    CheckpointManager checkpointManager = CheckpointManager.Instance;
                    if (checkpointManager != null)
                    {
                        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
                        foreach (GameObject checkpoint in checkpoints)
                        {
                            {
                                if (checkpoint.GetComponent<Checkpoint>().checkpointID == checkpointID)
                                {
                                    player.GetComponent<Rigidbody2D>().gravityScale = 0;

                                    
                                    checkpoint.GetComponent<Checkpoint>().ActivateCheckpoint();
                                    break;
                                }
                            }
                        }
                    }
                }
                Vector2 savedPos = new Vector2(PlayerPrefs.GetFloat("PlayerPosX", 0), PlayerPrefs.GetFloat("PlayerPosY", 0));
                if (savedPos != Vector2.zero)
                {
                    player.transform.position = new Vector2(PlayerPrefs.GetFloat("PlayerPosX"), PlayerPrefs.GetFloat("PlayerPosY"));
                }
            }
            continued = false;

        }
    }
}
