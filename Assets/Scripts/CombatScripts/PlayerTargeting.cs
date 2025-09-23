using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    [Header("Targeting Settings")]
    [Tooltip("The maximum distance the player can target an enemy from.")]
    [SerializeField] private float targetingDistance = 25f;
    [Tooltip("The layer(s) that enemies are on.")]
    [SerializeField] private LayerMask enemyLayer;

    private Camera _mainCamera;
    private EnemyInfoDisplay _currentTargetDisplay;
    private PlayerStats _playerStats;

    // A public property to get the current target's GameObject.
    public GameObject GetCurrentTarget()
    {
        return _currentTargetDisplay != null ? _currentTargetDisplay.gameObject : null;
    }

    private void Start()
    {
        _playerStats = GetComponent<PlayerStats>();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleTargeting();
    }

    private void HandleTargeting()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, targetingDistance, enemyLayer))
        {
            EnemyInfoDisplay hitDisplay = hit.collider.GetComponent<EnemyInfoDisplay>();

            if (hitDisplay != null && hitDisplay != _currentTargetDisplay)
            {
                if (_currentTargetDisplay != null)
                {
                    _currentTargetDisplay.HideDisplay();
                }

                _currentTargetDisplay = hitDisplay;
                _currentTargetDisplay.ShowDisplay(_playerStats);
            }
        }
        else
        {
            if (_currentTargetDisplay != null)
            {
                _currentTargetDisplay.HideDisplay();
                _currentTargetDisplay = null;
            }
        }
    }
}