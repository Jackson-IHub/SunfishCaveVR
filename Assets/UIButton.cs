using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
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

    [Header("UI Animation Settings")]
    [Tooltip("Drag the UI panel/object you want to animate here.")]
    public RectTransform targetUI;
    [Tooltip("The scale the UI will be set to before it grows (usually 0,0,0).")]
    public Vector3 initialUIScale = Vector3.zero;
    [Tooltip("How long it takes for the UI to move and grow to its final spot.")]
    public float uiAnimationDuration = 0.5f;
    [Tooltip("Curve to make the movement/scale look smooth (EaseInOut is a good default).")]
    public AnimationCurve uiAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Quaternion originalLocalRotation;
    private Coroutine shakeCoroutine;
    private Coroutine fadeCoroutine;
    private Coroutine uiCoroutine;

    // Saved UI states
    private Vector3 savedUIPosition;
    private Vector3 savedUIScale;

    // Reference to the Button component
    private Button thisButton;

    public DataPanelMan dataPanelMan;
    public MovementMan movementMan;

    private void Awake()
    {
        // Get the Button component attached to this GameObject
        thisButton = GetComponent<Button>();

        // Save the desired layout position and scale, then snap it to the button and hide it.
        if (targetUI != null)
        {
            savedUIPosition = targetUI.position;
            savedUIScale = targetUI.localScale;

            targetUI.position = transform.position;
            targetUI.localScale = initialUIScale;
            targetUI.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        originalLocalRotation = transform.localRotation;
        shakeCoroutine = StartCoroutine(ShakeCycle());
    }

    private void OnDisable()
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        if (uiCoroutine != null) StopCoroutine(uiCoroutine);

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

        childImage.color = new Color(color.r, color.g, color.b, targetA);

        // Trigger the new UI Animation
        if (targetUI != null)
        {
            targetUI.gameObject.SetActive(true);
            if (uiCoroutine != null) StopCoroutine(uiCoroutine);
            uiCoroutine = StartCoroutine(AnimateUI());
        }

        // Maintain original external script triggers
        if (dataPanelMan != null)
        {
            dataPanelMan.gameObject.SetActive(true);
            dataPanelMan.StartPanelSequence();
        }

        if (movementMan != null)
        {
            movementMan.OnButtonPressed();
        }
    }

    private IEnumerator AnimateUI()
    {
        float elapsed = 0f;
        Vector3 startPos = targetUI.position;
        Vector3 startScale = targetUI.localScale;

        while (elapsed < uiAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / uiAnimationDuration;
            float curveValue = uiAnimationCurve.Evaluate(t);

            // Interpolate position and scale based on the curve
            targetUI.position = Vector3.Lerp(startPos, savedUIPosition, curveValue);
            targetUI.localScale = Vector3.Lerp(startScale, savedUIScale, curveValue);

            yield return null;
        }

        // Ensure it ends perfectly on the exact saved values
        targetUI.position = savedUIPosition;
        targetUI.localScale = savedUIScale;

        this.gameObject.SetActive(false);
    }
}