using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// This singleton manager controls the opening and closing of major UI panels,
// handling a stack to allow Sub-Panels to open on top of Primary panels.
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private Stack<UIPanel> _panelStack = new Stack<UIPanel>();

    // NEW: A public property to check which panel is currently on top.
    public UIPanel TopPanel => _panelStack.Count > 0 ? _panelStack.Peek() : null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Opens a UI panel and manages the panel stack.
    /// </summary>
    public void OpenPanel(UIPanel panelToOpen)
    {
        if (panelToOpen == null || panelToOpen.IsOpen) return;

        // If the new panel is a Primary panel, close all currently open panels.
        if (panelToOpen.Type == PanelType.Primary)
        {
            CloseAllPanels();
        }

        // Open the new panel and add it to the stack.
        panelToOpen.Open();
        _panelStack.Push(panelToOpen);
    }

    /// <summary>
    /// Closes the most recently opened UI panel.
    /// </summary>
    public void CloseTopPanel()
    {
        if (_panelStack.Count > 0)
        {
            UIPanel panelToClose = _panelStack.Pop();
            panelToClose.Close();
        }
    }

    /// <summary>
    /// Closes all currently open UI panels.
    /// </summary>
    public void CloseAllPanels()
    {
        while (_panelStack.Count > 0)
        {
            UIPanel panel = _panelStack.Pop();
            panel.Close();
        }
    }
}