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
        // Başlangıç değerlerini sakla
        previousOrthographicSize = cam.orthographicSize;
        previousAspect = cam.aspect;
    }

    private void Update()
    {
        // Eğer kamera ayarlarından herhangi biri değişmişse:
        if (cam.orthographicSize != previousOrthographicSize || cam.aspect != previousAspect)
        {
            OnCameraSettingsChanged();
            // Yeni değerleri güncelle
            previousOrthographicSize = cam.orthographicSize;
            previousAspect = cam.aspect;
        }
    }

    /// <summary>
    /// Kamera ayarlarında bir değişiklik algılandığında tetiklenir.
    /// Buraya kamera ayarları değiştiğinde yapılmasını istediğiniz işlemleri ekleyebilirsiniz.
    /// </summary>
    private void OnCameraSettingsChanged()
    {
        gridManager.GenerateGrid();
    }
}
