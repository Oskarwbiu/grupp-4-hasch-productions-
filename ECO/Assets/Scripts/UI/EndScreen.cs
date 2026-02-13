using UnityEngine;
using UnityEngine.UIElements;

public class EndScreen : MonoBehaviour
{
    private UIDocument document;
    private void Start()
    {
        document = GetComponent<UIDocument>();
        document.rootVisualElement.style.display = DisplayStyle.None;



    }


    public void ShowScreen()
    {
        document.rootVisualElement.style.display = DisplayStyle.Flex;
    }


}
