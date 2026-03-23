using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] GameObject[] keyEnemies;

    private void Update()
    {
        if (AllEnemiesDestroyed())
        {
            Destroy(gameObject);
        }
    }

    private bool AllEnemiesDestroyed()
    {
        foreach (GameObject enemy in keyEnemies)
        {
            if (enemy != null) 
            { return false; }
        }
        return true;
    }
}
