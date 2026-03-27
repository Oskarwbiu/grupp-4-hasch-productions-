using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lr;

    [SerializeField] private Texture2D[] textures;
    [SerializeField] private float fps;

    private int animationStep;
    private float fpsCounter;


    private void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        fpsCounter += Time.deltaTime;

        if (fpsCounter >= 1f / fps)
        {
            animationStep++;

            if (animationStep == textures.Length)
            { animationStep = 0; }

            lr.material.SetTexture("_MainTex", textures[animationStep]);

            fpsCounter = 0f;
        }
    }
}
