using UnityEngine;
using UnityEngine.UIElements;

public class Bossbar : MonoBehaviour
{
    

    public float health = 100f;
    public float maxHealth = 100f;
    private UIDocument barDocument;
    ProgressBar bossHealthBar;

    private VisualElement barVE;
    private void Start()
    {
        barDocument = GetComponent<UIDocument>();
        barVE = barDocument.rootVisualElement as VisualElement;
        hideBar();
        bossHealthBar = barVE.Q<ProgressBar>("Bossbar");
        bossHealthBar.value = 100;


    }

    public void showBar()
    {
        barVE.style.display = DisplayStyle.Flex;
    }

    void hideBar()
    {
        barVE.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        bossHealthBar.value = health/maxHealth * 100;

        if (health <= 0)
        {
            hideBar();
        }
    }
}
