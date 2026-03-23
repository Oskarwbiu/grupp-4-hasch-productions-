using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Bossbar : MonoBehaviour
{


    [SerializeField] string bossName;
    private UIDocument barDocument;
    VisualElement fill;
    ProgressBar bossHealthBar;

    private VisualElement barVE;
    private void Start()
    {
        barDocument = GetComponent<UIDocument>();
        barVE = barDocument.rootVisualElement as VisualElement;
        fill = barVE.Q("Bossbar").Q(className: "unity-progress-bar__progress");
        bossHealthBar = barVE.Q<ProgressBar>("Bossbar");
        hideBar();
        bossHealthBar.value = 100f;
        bossHealthBar.title = bossName;

    }

    public void showBar()
    {
        barVE.style.display = DisplayStyle.Flex;
        Debug.Log("showBar called, fill is: " + fill);
    }

    void hideBar()
    {
        barVE.style.display = DisplayStyle.None;
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        float percent = Mathf.Clamp01(currentHealth / maxHealth) * 100f;
        
        StartCoroutine(AnimateBar(bossHealthBar.value, percent, 0.5f));

        if (currentHealth <= 0)
        {
            hideBar();
        }
    }

    IEnumerator AnimateBar(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = Mathf.SmoothStep(0f, 1f, t); // ease in/out
            bossHealthBar.value = Mathf.Lerp(from, to, t);
            yield return null;
        }
        bossHealthBar.value = to;
    }
}
