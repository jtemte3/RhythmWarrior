using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    public bool enableFog;
    [Range(0,1)]
    public float reflectionIntensity;
    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.fog = enableFog;
        RenderSettings.reflectionIntensity = reflectionIntensity;
    }
}
