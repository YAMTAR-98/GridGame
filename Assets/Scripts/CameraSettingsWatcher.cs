using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSettingsWatcher : MonoBehaviour
{
    private Camera cam;

    public GridManager gridManager;
    private float previousOrthographicSize;
    private float previousAspect;

    private void Start()
    {
        cam = GetComponent<Camera>();
        previousOrthographicSize = cam.orthographicSize;
        previousAspect = cam.aspect;
    }

    private void Update()
    {
        if (cam.orthographicSize != previousOrthographicSize || cam.aspect != previousAspect)
        {
            OnCameraSettingsChanged();
            previousOrthographicSize = cam.orthographicSize;
            previousAspect = cam.aspect;
        }
    }

    private void OnCameraSettingsChanged()
    {
        gridManager.GenerateGrid();
    }
}
