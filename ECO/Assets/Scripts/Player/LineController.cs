using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lr;

    [SerializeField] private Texture2D[] textures;
    [SerializeField] private float fps;
    [SerializeField] private bool isreapeating = true;

    private int animationStep;
    private float fpsCounter;
    bool stopAnimation = false;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (!lr.enabled) 
        { 
            stopAnimation = false;
            animationStep = 0;
            return; 
        }
        if (stopAnimation) { return; }
        fpsCounter += Time.deltaTime;

        if (fpsCounter >= 1f / fps)
        {
            animationStep++;
            Debug.Log("step");
            if (animationStep == textures.Length && isreapeating)
            { animationStep = 0; }
            else if (animationStep == textures.Length && !isreapeating)
            {
                stopAnimation = true;
                return;
            }

            lr.material.SetTexture("_MainTex", textures[animationStep]);

            fpsCounter = 0f;
        }
    }
}
