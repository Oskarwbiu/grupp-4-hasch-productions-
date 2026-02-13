using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelExit : MonoBehaviour
{
    public Animator transition;
    
    [SerializeField] Scene level;
    [SerializeField] float transitionTime;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Destroy(collision.gameObject.GetComponent<PlayerInput>());
            StartLevelCoroutine(level);
        }
    }
    public IEnumerator LoadLevel(Scene level)
    {
        CheckpointManager.Instance.ResetCheckpoints();
        transition.SetTrigger("start");

        yield return new WaitForSeconds(transitionTime);

        UnityEngine.SceneManagement.SceneManager.LoadScene((int)level);
    }

    public void StartLevelCoroutine(Scene level)
    {
        StartCoroutine(LoadLevel(level));
    }

}
