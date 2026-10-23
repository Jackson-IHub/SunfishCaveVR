using System.Collections;
using UnityEngine;
using UnityEngine.UI; 

public class InfoButtonMan : MonoBehaviour
{
    [Header("Shake Intervals")]
    [Tooltip("Minimum time (in seconds) between shakes.")]
    public float minInterval = 2f;
    [Tooltip("Maximum time (in seconds) between shakes.")]
    public float maxInterval = 5f;

    [Header("Shake Settings")]
    [Tooltip("How long the shake lasts.")]
    public float shakeDuration = 0.5f;
    [Tooltip("How many degrees the object rotates on the Z axis during the shake.")]
    public float shakeIntensity = 5f; 

    [Header("Activation Settings")]
    [Tooltip("Drag the child Image object here.")]
    public Image childImage;
    [Tooltip("Drag the external Particle System object here.")]
    public ParticleSystem activationParticles;
    [Tooltip("The alpha value (0 to 1) to fade to. 0 is fully transparent.")]
    public float targetAlpha = 0.3f;
    [Tooltip("How long it takes to fade to the target alpha.")]
    public float fadeDuration = 0.5f;

    private Quaternion originalLocalRotation;
    private Coroutine shakeCoroutine;
    private Coroutine fadeCoroutine;

    public DataPanelMan dataPanelMan;

    private void OnEnable()
    {
        originalLocalRotation = transform.localRotation;
        shakeCoroutine = StartCoroutine(ShakeCycle());
    }

    private void OnDisable()
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        
        transform.localRotation = originalLocalRotation;
    }

    private IEnumerator ShakeCycle()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            float elapsed = 0f;
            while (elapsed < shakeDuration)
            {
                float zAngleOffset = Random.Range(-shakeIntensity, shakeIntensity);
                transform.localRotation = originalLocalRotation * Quaternion.Euler(0f, 0f, zAngleOffset);
                
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localRotation = originalLocalRotation;
        }
    }

    public void ActivateButton()
    {
        if (activationParticles != null)
        {
            // Apply a completely random rotation before playing
            //activationParticles.transform.rotation = Random.rotation;
            activationParticles.Play();
        }

        if (childImage != null)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeImageAlpha(targetAlpha, fadeDuration));
        }
    }

    private IEnumerator FadeImageAlpha(float targetA, float duration)
    {
        Color color = childImage.color;
        float startAlpha = color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetA, elapsed / duration);
            childImage.color = new Color(color.r, color.g, color.b, newAlpha);
            yield return null;
        }
        
        dataPanelMan.gameObject.SetActive(true);
        dataPanelMan.StartPanelSequence();
        childImage.color = new Color(color.r, color.g, color.b, targetA);
    }
}