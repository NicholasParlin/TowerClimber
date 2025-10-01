using UnityEngine;

// An enum to define the behavior of different panel types.
public enum PanelType
{
    // A primary panel that will close other primary panels when opened.
    Primary,
    // A sub-panel that can stack on top of a primary panel.
    SubPanel
}

// A simple base class for all toggleable UI panels.
public abstract class UIPanel : MonoBehaviour
{
    [Header("Panel Settings")]
    [Tooltip("The main parent GameObject for this UI panel.")]
    [SerializeField] protected GameObject panelObject;
    [Tooltip("The type of this panel, which determines how it interacts with other panels.")]
    [SerializeField] protected PanelType panelType = PanelType.Primary;

    // Public property for other scripts to read the panel type.
    public PanelType Type => panelType;
    public bool IsOpen { get; private set; }

    protected virtual void Start()
    {
        panelObject.SetActive(false);
        IsOpen = false;
    }

    public virtual void Open()
    {
        panelObject.SetActive(true);
        IsOpen = true;
    }

    public virtual void Close()
    {
        panelObject.SetActive(false);
        IsOpen = false;
    }
}