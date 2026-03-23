using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{

    CinemachineCamera virtualCamera;
    CinemachineBasicMultiChannelPerlin perlin;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineCamera>();
        perlin = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        ResetIntensity();
    }

    public void ShakeCamera(float duration, float intensity)
    {
        perlin.AmplitudeGain = intensity;
        StartCoroutine(WaitTime(duration));
    }

    IEnumerator WaitTime(float shakeTime)
    {
        yield return new WaitForSeconds(shakeTime);
        ResetIntensity();
    }

    void ResetIntensity()
    {
        perlin.AmplitudeGain = 0f;
    }
}
