using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class dialogueManager : MonoBehaviour
{
    private Queue<Sentences> sentences;

    private DialogueBox dialogueBox;
    
    void Start()
    {
        sentences = new Queue<Sentences>();
        dialogueBox = FindFirstObjectByType<DialogueBox>();
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
        dialogueBox.StartDialogue(dialogue.name, dialogue.sentences[0].ToString(), dialogue.icon);

        sentences.Clear();

        foreach (Sentences sentence in dialogue.sentences)
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

        Sentences sentence = sentences.Dequeue();
        dialogueBox.UpdateDialogue(sentence.ToString(), sentence.GetVoice());

    }

    void EndDialogue()
    {
        dialogueBox.HideBox();
    }


}
