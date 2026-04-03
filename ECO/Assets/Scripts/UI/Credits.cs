using UnityEngine;
using UnityEngine.UIElements;

public class Credits : MonoBehaviour
{
    DialogueTrigger dialogue;
    UIDocument creditDocument;
    bool isShown;
    // Charlie gröneng was not here
    void Start()
    {
        creditDocument = GetComponent<UIDocument>();
        dialogue = FindFirstObjectByType<DialogueTrigger>();
        creditDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        if (dialogue.triggerEvent && !isShown)
        {
            isShown = true;
            MusicManager.Instance.PlayMusic("Backrooms", 1);
            creditDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        }
    }


}
