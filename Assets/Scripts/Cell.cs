using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour
{
    [HideInInspector]
    public int x, y;
    private GridManager gridManager;
    public bool IsMarked { get; private set; } = false;

    [Header("Görsel Ayarlar")]
    [Tooltip("X işaretini temsil eden GameObject (sprite, textmesh, vs.)")]
    public GameObject xMarker;

    [Header("Scale Ayarları")]
    [Tooltip("X işareti aktif edildiğinde ulaşılacak hedef scale değeri")]
    public Vector3 targetScale = new Vector3(0.16f, 0.16f, 0.16f);
    [Tooltip("Scale artış süresi (saniye cinsinden)")]
    public float scaleDuration = 0.25f;

    [Header("Clear Ayarları")]
    [Tooltip("Clear işlemi sırasında küçülme animasyonunun süresi (saniye cinsinden)")]
    public float clearDuration = 0.25f;

    // Hücreyi başlatmak için çağrılır
    public void Init(int x, int y, GridManager manager)
    {
        this.x = x;
        this.y = y;
        gridManager = manager;

        // Başlangıçta X işareti gizli ve scale sıfırlanmış olsun
        if (xMarker != null)
        {
            xMarker.SetActive(false);
            xMarker.transform.localScale = Vector3.zero;
        }
    }

    // Hücreye X işareti ekle
    public void Mark()
    {
        IsMarked = true;
        if (xMarker != null)
        {
            xMarker.SetActive(true);
            StartCoroutine(ScaleXMarker());
        }
    }

    // Hücredeki X işaretini temizle: önce küçülme animasyonu oynar, sonra devre dışı bırakılır.
    public void Clear()
    {
        IsMarked = false;
        if (xMarker != null)
        {
            StartCoroutine(ShrinkAndDisable());
        }
    }

    // xMarker'ın scale'ini hedef scale değerine doğru arttıran coroutine
    private IEnumerator ScaleXMarker()
    {
        Vector3 initialScale = xMarker.transform.localScale;
        float elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            xMarker.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / scaleDuration);
            yield return null;
        }
        xMarker.transform.localScale = targetScale;
        // Kısa bir bekleme süresi sonrasında gridManager'dan eşleşme kontrolü tetikleniyor
        gridManager.CheckAndClearMatches(this);
    }

    // xMarker'ın scale'ini sıfıra doğru küçülten ve ardından devre dışı bırakan coroutine
    private IEnumerator ShrinkAndDisable()
    {
        gridManager.ScoreCount();
        Vector3 initialScale = xMarker.transform.localScale;
        float elapsed = 0f;
        while (elapsed < clearDuration)
        {
            elapsed += Time.deltaTime;
            xMarker.transform.localScale = Vector3.Slerp(initialScale, Vector3.zero, elapsed / clearDuration);
            yield return null;
        }
        xMarker.transform.localScale = Vector3.zero;
        xMarker.SetActive(false);
    }

    // Tıklama algılayıcısı: Bu metodun çalışabilmesi için GameObject üzerinde Collider bulunmalı!
    private void OnMouseDown()
    {
        gridManager.OnCellClicked(this);
    }
}
