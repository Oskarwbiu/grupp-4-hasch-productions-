using UnityEngine;
using UnityEngine.SceneManagement;
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
        SceneManager.sceneLoaded += OnSceneLoaded;

        pauseDocument = GetComponent<UIDocument>();
        pauseVE = pauseDocument.rootVisualElement as VisualElement;
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        

        VisualElement root = pauseDocument.rootVisualElement;

        healthElement = root.Q<VisualElement>("Health");
        if (playerHealth.currentHealth >= healthSprites.Length)
        {
            healthElement.style.backgroundImage = new StyleBackground(healthSprites[0]);
        }

    }
    private void Update()
    {
       if (playerHealth == null)
       {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
       }
        
       if (playerHealth.currentHealth != lastHealth)
       {
            UpdateHealthGUI();
       }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateHealthGUI();
        if (playerHealth != null) { return; }
        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }


        void UpdateHealthGUI()
    {
        if (healthElement != null && playerHealth.currentHealth >= 0 && playerHealth.currentHealth < healthSprites.Length)
        {
            healthElement.style.backgroundImage = new StyleBackground(healthSprites[(int)playerHealth.currentHealth]);
        }
        
        lastHealth = playerHealth.currentHealth;
    }

}
