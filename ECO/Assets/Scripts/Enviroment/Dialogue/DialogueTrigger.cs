using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    bool hasBeenTriggered = false;

    public void TriggerDialogue()
    {
        FindFirstObjectByType<dialogueManager>().StartDialogue(dialogue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenTriggered) { return; }
        hasBeenTriggered = true;
        
        TriggerDialogue();
    }
}
