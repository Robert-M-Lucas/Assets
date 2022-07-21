using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlScript : MonoBehaviour
{
    [Header("Settings")]
    public float PerspectiveSwitchTime = 1f;
    public float MaxDist = 15;
    public float MinDist = 10;
    public float ScrollSensitivity = 1;
    public float NormalFov = 60;
    public float _2DFov = 2;
    public float OrthographicWidth = 11;

    [Header("References")]
    public Transform RotationalPivot;
    public Transform UpDownPivot;
    public Transform CameraPosition;
    public InputManager inputManager;
    public Camera _camera;
    

    public AppearanceManager3D appearanceManager3D;

    public float perspective_progress = 0;


    Vector2 camera_rotation;

    void Start()
    {
        camera_rotation.y = RotationalPivot.localRotation.eulerAngles.y;
        camera_rotation.x = UpDownPivot.localRotation.eulerAngles.x;
    }

    public void RotateCamera(Vector2 look)
    {
        camera_rotation.x -= look.y;
        camera_rotation.y += look.x;

        camera_rotation.x = Mathf.Clamp(camera_rotation.x, 0, 90);

        RotationalPivot.transform.localRotation = Quaternion.Euler(0, camera_rotation.y, 0);
        UpDownPivot.transform.localRotation = Quaternion.Euler(camera_rotation.x, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (perspective_progress == 0)
        {
            CameraPosition.localPosition = new Vector3(0, 0, -Mathf.Clamp((-CameraPosition.localPosition.z) + (-Input.mouseScrollDelta.y * ScrollSensitivity), MinDist, MaxDist));
        }

        if ((inputManager.PerspectiveMode && perspective_progress != 0) || (!inputManager.PerspectiveMode && perspective_progress != 1))
        {
            Vector3 movingFrom = transform.parent.position;
            Quaternion rotatingFrom = transform.parent.rotation;

            if (inputManager.PerspectiveMode == false)
            {
                perspective_progress += Time.deltaTime / PerspectiveSwitchTime;
                if (perspective_progress >= 1)
                {
                    perspective_progress = 1;
                }
            }
            else
            {
                perspective_progress -= Time.deltaTime / PerspectiveSwitchTime;
                if (perspective_progress <= 0)
                {
                    perspective_progress = 0;
                }
            }

            if (perspective_progress < 0.7f)
            {
                float smooth_progress = MathP.CosSmooth(perspective_progress * (1/0.7f));

                transform.position = Vector3.Lerp(movingFrom, Vector3.up * (OrthographicWidth / (2 * Mathf.Tan(0.5f * NormalFov * Mathf.Deg2Rad))), smooth_progress);
                transform.rotation = Quaternion.Lerp(rotatingFrom, Quaternion.Euler(90, 0, 180), smooth_progress);
                _camera.fieldOfView = NormalFov;
            }
            else
            {
                float smooth_progress = Mathf.Clamp(MathP.CosSmooth((perspective_progress - 0.7f) * (1/0.3f)), 0.000000001f, 1);

                _camera.fieldOfView = NormalFov - (smooth_progress * (NormalFov - _2DFov));

                transform.rotation = Quaternion.Euler(90, 0, 180);

                transform.position = Vector3.up * (OrthographicWidth / (2*Mathf.Tan(0.5f * _camera.fieldOfView * Mathf.Deg2Rad)));
            }
            

            
        }
    }
}
