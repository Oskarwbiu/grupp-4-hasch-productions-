using UnityEngine;

public class MechanicTalkScript : MonoBehaviour
{
    [SerializeField] DialogueTrigger trigger;
    [SerializeField] Collider2D bossBounds;

    private void Start()
    {
        bossBounds.enabled = false;
    }
    void Update()
    {
        if (trigger.triggerEvent)
        {
            bossBounds.enabled = true;
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
