using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class dialogueManager : MonoBehaviour
{
    private Queue<string> sentences;

    private DialogueBox dialogueBox;
    
    void Start()
    {
        sentences = new Queue<string>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (dialogueBox == null)
        {
            dialogueBox = FindFirstObjectByType<DialogueBox>();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueBox.StartDialogue(dialogue.name, dialogue.sentences[0], dialogue.icon);

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        { 
            sentences.Enqueue(sentence); 
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueBox.UpdateDialogue(sentence);

    }

    void EndDialogue()
    {
        dialogueBox.HideBox();
    }


}
