using UnityEngine;

public class DontDestroy : MonoBehaviour
{
   
    private void Awake()
    {
        int numberOfInstances = FindObjectsByType<DontDestroy>(FindObjectsSortMode.None).Length;

        if (numberOfInstances > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
        }

    }

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
    
}

