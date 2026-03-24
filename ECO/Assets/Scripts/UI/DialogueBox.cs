using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueBox : MonoBehaviour
{
    private UIDocument dialogueDocument;

    private VisualElement dialogueVE;

    VisualElement icon;
    Label dialogue;
    Label characterName;

    void Start()
    {
        dialogueDocument = GetComponent<UIDocument>();
        dialogueVE = dialogueDocument.rootVisualElement as VisualElement;

        dialogueVE.style.display = DisplayStyle.None;

        VisualElement root = dialogueDocument.rootVisualElement;

        characterName = root.Q<Label>("Name");

        dialogue = root.Q<Label>("Dialogue");

        Button continueButton = root.Q<Button>("Continue");
        continueButton.RegisterCallback<ClickEvent>(evt => FindFirstObjectByType<dialogueManager>().DisplayNextSentence());

        icon = root.Q<VisualElement>("Icon");
    }

    public void StartDialogue(string name, string currentDialogue, Sprite currentIcon)
    {
        dialogueVE.style.display = DisplayStyle.Flex;
        characterName.text = name;
        dialogue.text = currentDialogue;
        icon.style.backgroundImage = new StyleBackground(currentIcon);
    }

    public void UpdateDialogue(string currentDialogue, AudioClip voice)
    {
        StopAllCoroutines();
        if (voice != null)
        {
            SoundManager.Instance.PlaySoundByClip2D(voice);
        }

        StartCoroutine(AnimateLetters(currentDialogue));
    }

    IEnumerator AnimateLetters(string currentDialogue)
    {
        dialogue.text = "";
        
        foreach (char letter in currentDialogue.ToCharArray())
        {
            dialogue.text += letter;
            yield return null;
            yield return null;
        }
    }

    public void HideBox()
    {
        dialogueVE.style.display = DisplayStyle.None;
    }

    
}
