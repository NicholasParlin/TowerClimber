using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualizedScrollView : MonoBehaviour
{
    public event Action<GameObject> OnItemCreated;

    [Header("UI References")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private GameObject itemPrefab;

    [Header("Layout Settings")]
    [SerializeField] private float itemHeight = 100f;
    [SerializeField] private float itemSpacing = 10f;

    private List<object> _dataList; // Now a generic list of objects
    private Action<GameObject, object> _setupFunction; // The function used to populate a UI item
    private List<GameObject> _pooledItemObjects = new List<GameObject>();
    private int _firstVisibleIndex = -1;

    private void Awake()
    {
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    /// <summary>
    /// Initializes the scroll view with a list of data and a function to set up the UI elements.
    /// </summary>
    public void Initialize(List<object> data, Action<GameObject, object> setupFunction)
    {
        _dataList = data;
        _setupFunction = setupFunction;

        float totalContentHeight = _dataList.Count * (itemHeight + itemSpacing);
        contentPanel.sizeDelta = new Vector2(contentPanel.sizeDelta.x, totalContentHeight);

        CreateInitialPool();
        OnScroll(Vector2.zero);
    }

    private void CreateInitialPool()
    {
        float viewportHeight = scrollRect.viewport.rect.height;
        int requiredPoolSize = Mathf.CeilToInt(viewportHeight / (itemHeight + itemSpacing)) + 1;

        while (_pooledItemObjects.Count < requiredPoolSize)
        {
            GameObject newItem = Instantiate(itemPrefab, contentPanel);
            _pooledItemObjects.Add(newItem);
            OnItemCreated?.Invoke(newItem);
        }
    }

    private void OnScroll(Vector2 scrollPosition)
    {
        if (_dataList == null || _dataList.Count == 0 || _setupFunction == null) return;

        float scrollY = 1 - scrollPosition.y;
        int newFirstVisibleIndex = Mathf.FloorToInt((contentPanel.rect.height * scrollY) / (itemHeight + itemSpacing));

        if (newFirstVisibleIndex == _firstVisibleIndex && contentPanel.gameObject.activeInHierarchy) return;

        _firstVisibleIndex = Mathf.Max(0, newFirstVisibleIndex);

        for (int i = 0; i < _pooledItemObjects.Count; i++)
        {
            int dataIndex = _firstVisibleIndex + i;
            GameObject itemObject = _pooledItemObjects[i];

            if (dataIndex < _dataList.Count)
            {
                itemObject.SetActive(true);

                RectTransform itemRect = itemObject.GetComponent<RectTransform>();
                itemRect.anchoredPosition = new Vector2(0, -dataIndex * (itemHeight + itemSpacing));

                // Use the provided setup function to populate the UI element.
                _setupFunction(itemObject, _dataList[dataIndex]);
            }
            else
            {
                itemObject.SetActive(false);
            }
        }
    }
}