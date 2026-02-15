using UnityEngine;

public class DontDestroyMenu : MonoBehaviour
{
    private void Awake()
    {
        int numberOfInstances = FindObjectsByType<DontDestroyMenu>(FindObjectsSortMode.None).Length;

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
