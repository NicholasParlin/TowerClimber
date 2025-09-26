using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// This script is attached to the prefab for a single quest progress notification (a "toast").
// It handles the animation of fading in and out.
[RequireComponent(typeof(CanvasGroup))]
public class QuestProgressToastUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text progressText;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float displayTime = 3.0f;
    [SerializeField] private float fadeOutTime = 1.0f;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        gameObject.SetActive(false); // Start disabled
    }

    /// <summary>
    /// Shows the notification with the specified text and starts its life cycle.
    /// </summary>
    public void Show(string message)
    {
        progressText.text = message;
        gameObject.SetActive(true);
        StartCoroutine(AnimateToast());
    }

    private IEnumerator AnimateToast()
    {
        // Fade In
        _canvasGroup.alpha = 0;
        float timer = 0;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeInTime);
            yield return null;
        }
        _canvasGroup.alpha = 1;

        // Wait
        yield return new WaitForSeconds(displayTime);

        // Fade Out
        timer = 0;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeOutTime);
            yield return null;
        }
        _canvasGroup.alpha = 0;

        // Deactivate when finished to be reused by the pool.
        gameObject.SetActive(false);
    }
}