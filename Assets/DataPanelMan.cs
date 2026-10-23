using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class DataPanelMan : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The world position the panel will move to when activated.")]
    public Vector3 targetPosition = new Vector3(47,-1.8f,-20.3f);
    
    [Tooltip("The final scale the panel will lerp into.")]
    public Vector3 targetScale = new Vector3(0.0008f, 0.0008f, 0.0008f);

    [Header("Animation Settings")]
    [Tooltip("How long the scaling animation takes in seconds.")]
    public float scaleDuration = 0.5f;

    private RectTransform rectTransform;

    private void Awake()
    {
        // Cache the RectTransform reference when the script loads
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Call this function via script or a Unity Event to start the sequence.
    /// </summary>
    public void StartPanelSequence()
    {
        StartCoroutine(PanelSequenceRoutine());
    }

    private IEnumerator PanelSequenceRoutine()
    {
        // 1. Turn the game object on
        gameObject.SetActive(true);

        // 2. Move the object to the target position
        // Note: For World Space canvases, setting the world position directly works best. 
        // If switching to Screen Space later, you would use anchoredPosition3D instead.
        rectTransform.localPosition = targetPosition;

        // 3. Lerp scale into the target scale
        float elapsedTime = 0f;
        
        while (elapsedTime < scaleDuration)
        {
            // Lerps between zero and the target scale based on elapsed time
            rectTransform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsedTime / scaleDuration);
            
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Ensure the scale is set to the exact target value at the end of the loop
        rectTransform.localScale = targetScale;
    }
}