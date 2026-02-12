using UnityEngine;
using UnityEngine.UIElements;

public class HUDScript : MonoBehaviour
{
    private UIDocument pauseDocument;

    private VisualElement pauseVE;

    [SerializeField] Sprite[] healthSprites;
    [SerializeField] PlayerHealth playerHealth;

    VisualElement healthElement;
    float lastHealth = 0;
    private void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        pauseDocument = GetComponent<UIDocument>();
        pauseVE = pauseDocument.rootVisualElement as VisualElement;

        VisualElement root = pauseDocument.rootVisualElement;

        healthElement = root.Q<VisualElement>("Health");
        healthElement.style.backgroundImage = new StyleBackground(healthSprites[0]);

    }
    private void Update()
    {
       
        if (playerHealth.currentHealth != lastHealth)
        {
            UpdateHealthGUI();
        }
        
        


    }

    void UpdateHealthGUI()
    {
        healthElement.style.backgroundImage = new StyleBackground(healthSprites[(int)playerHealth.currentHealth]);
        lastHealth = playerHealth.currentHealth;
    }
}
