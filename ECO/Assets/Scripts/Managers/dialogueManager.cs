using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class dialogueManager : MonoBehaviour
{
    private Queue<Sentences> sentences;

    private DialogueBox dialogueBox;
    bool trigger;
    DialogueTrigger triggerObject;
    
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
        dialogueBox.HideBox();
    }

    public void StartDialogue(Dialogue dialogue, bool triggerOnEnDialogue, DialogueTrigger dialogueTrigger)
    {
        dialogueBox.StartDialogue(dialogue.name, dialogue.sentences[0].ToString(), dialogue.icon);

        trigger = triggerOnEnDialogue;
        triggerObject = dialogueTrigger;

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

    public void EndDialogue()
    {
        SoundManager.Instance.StopPlayingClip();

        if (dialogueBox == null)
        {
            dialogueBox = FindFirstObjectByType<DialogueBox>();
        }

        dialogueBox.HideBox();

        if (trigger)
        {
            triggerObject.triggerEvent = true;
        }

    }


}
