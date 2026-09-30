using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class NitrateDetector : MonoBehaviour
{
    
    public Transform nitrateLocation;

    private float nitrateRange = 10f;

    private Color nitrateColor = new Color(0, 1, 0);
    private Color defaultColor = new Color(0, 0.66f, 1f);

    [SerializeField]
    private Volume volume;
    private ColorAdjustments colorAdjustments;


    private void Start()
    {
        volume.profile.TryGet(out colorAdjustments);
    }


    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(this.transform.position, nitrateLocation.position);
        float t = 1f - Mathf.Clamp01(distance / nitrateRange);

        colorAdjustments.colorFilter.value = Color.Lerp(defaultColor, nitrateColor, t);

        Debug.Log(distance);
        Debug.Log(t);
    }

}
