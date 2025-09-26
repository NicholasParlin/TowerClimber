using UnityEngine;

// A simple base class for all toggleable UI panels.
// Provides common functionality for opening and closing.
public abstract class UIPanel : MonoBehaviour
{
    [Header("Panel Settings")]
    [Tooltip("The main parent GameObject for this UI panel.")]
    [SerializeField] protected GameObject panelObject;

    public bool IsOpen { get; private set; }

    protected virtual void Start()
    {
        // Ensure all panels start in a closed state.
        panelObject.SetActive(false);
        IsOpen = false;
    }

    /// <summary>
    /// Opens the UI panel.
    /// </summary>
    public virtual void Open()
    {
        panelObject.SetActive(true);
        IsOpen = true;
    }

    /// <summary>
    /// Closes the UI panel.
    /// </summary>
    public virtual void Close()
    {
        panelObject.SetActive(false);
        IsOpen = false;
    }
}