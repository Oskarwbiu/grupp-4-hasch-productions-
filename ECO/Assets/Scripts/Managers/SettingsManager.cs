using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioSource sfxAudioSource;
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
        AudioListener.volume = masterVolume;
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
        bool isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        bool isFullscreen = PlayerPrefs.GetInt("IsFullscreen", 0) == 1;

        StartCoroutine(ApplyCheats());


        AudioListener.volume = masterVolume;
        musicAudioSource.volume = musicVolume;
        sfxAudioSource.volume = sfxVolume;
        AudioListener.pause = isMuted;
        Screen.fullScreen = isFullscreen;
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
                                    Vector3 checkpointPosition = checkpoint.transform.position;
                                    player.GetComponent<Rigidbody2D>().gravityScale = 0;

                                    player.transform.position = checkpointPosition;
                                    StartCoroutine(player.GetComponent<PlayerDeath>().Respawn());
                                    checkpoint.GetComponent<Checkpoint>().TriggerRespawn();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            continued = false;

        }
    }
}
