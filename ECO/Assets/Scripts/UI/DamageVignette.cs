using UnityEngine;
using System.Collections;

public class DamageVignette : MonoBehaviour
{
    [SerializeField] private CanvasGroup damageOverlay;
    [SerializeField] private float fadeDuration = 0.3f;

    public void ShowDamageVignette()
    {
        StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
        damageOverlay.alpha = 1f;
        yield return new WaitForSeconds(fadeDuration);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            damageOverlay.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        damageOverlay.alpha = 0f;
    }
}