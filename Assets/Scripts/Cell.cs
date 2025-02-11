using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour
{
    [HideInInspector]
    public int x, y;
    private GridManager gridManager;
    public bool IsMarked { get; private set; } = false;

    [Header("Visual Settings")]
    [Tooltip("GameObject representing the X marker (sprite, textmesh, etc.)")]
    public GameObject xMarker;

    [Header("Scale Settings")]
    [Tooltip("Target scale value when the X marker is activated")]
    public Vector3 targetScale = new Vector3(1.8f, 1.8f, 1.8f);
    [Tooltip("Duration for the scale increase (in seconds)")]
    public float scaleDuration = 0.25f;

    [Header("Clear Settings")]
    [Tooltip("Duration of the shrink animation during the Clear process (in seconds)")]
    public float clearDuration = 0.25f;

    public void Init(int x, int y, GridManager manager)
    {
        this.x = x;
        this.y = y;
        gridManager = manager;

        if (xMarker != null)
        {
            xMarker.SetActive(false);
            xMarker.transform.localScale = Vector3.zero;
        }
    }

    public void Mark()
    {
        IsMarked = true;
        if (xMarker != null)
        {
            xMarker.SetActive(true);
            StartCoroutine(ScaleXMarker());
        }
    }

    public void Clear()
    {
        IsMarked = false;
        if (xMarker != null)
        {
            StartCoroutine(ShrinkAndDisable());
        }
    }

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
        gridManager.CheckAndClearMatches(this);
    }

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

    private void OnMouseDown()
    {
        gridManager.OnCellClicked(this);
    }
}
