using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    bool hasBeenTriggered = false;
    [SerializeField] bool triggerEventOnEnd = false;
    public bool triggerEvent;

    public void TriggerDialogue()
    {
        FindFirstObjectByType<dialogueManager>().StartDialogue(dialogue, triggerEventOnEnd, this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenTriggered) { return; }
        hasBeenTriggered = true;
        
        TriggerDialogue();
    }

    
}
