using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class UnderwaterDepth : MonoBehaviour
{
    [Header("Depth amounts")]
    [SerializeField] private Transform mainCamera;
    [SerializeField] private int depth = 0;

    [Header("Post processing Volume")]
    [SerializeField] private Volume postProcessingVolume;

    [Header("Post Processing Profiles")]
    [SerializeField] private VolumeProfile surfacePostProcessing;
    [SerializeField] private VolumeProfile underwaterPostProcessing;
    private void Start()
    {
        if (GameObject.FindWithTag("WaterVolume") != null)
        {
           GameObject volumeObj = GameObject.FindGameObjectWithTag("WaterVolume");
            postProcessingVolume = volumeObj.GetComponent<Volume>();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (mainCamera.position.y < depth)
        {
            EnableEffects(true);
        }
        else
        {
            EnableEffects(false);
        }
    }

    private void EnableEffects(bool active)
    {
        if (active)
        {
            RenderSettings.fog = true;
            postProcessingVolume.profile = underwaterPostProcessing;
        }
        else
        {
            RenderSettings.fog = false;
            postProcessingVolume.profile = surfacePostProcessing;
        }
    }
}
