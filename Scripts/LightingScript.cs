using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingScript : MonoBehaviour
{
    public Vector3 InitialRotation;
    public Vector3 FinalRotation;
    public float RotationTime = 1;
    float RotationProgress = 0;
    public Light _light;
    public CanvasGroup canvasGroup;
    public Color TargetColor;

    void Start()
    {
        
    }

    void Update()
    {
        if (RotationProgress < 1)
        {
            RotationProgress += Time.deltaTime / RotationTime;
            if (RotationProgress > 1)
            {
                RotationProgress = 1;
            }
            _light.intensity = RotationProgress * RotationProgress * RotationProgress;
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(InitialRotation), Quaternion.Euler(FinalRotation), RotationProgress);
            canvasGroup.alpha = RotationProgress;
            RenderSettings.ambientLight = TargetColor * RotationProgress * RotationProgress * RotationProgress;
        }
    }
}
