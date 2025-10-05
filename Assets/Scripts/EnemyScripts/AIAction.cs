using UnityEngine;

// An enum to define the different conditions the AI can consider.
public enum ConsiderationType
{
    MyHealth,
    TargetDistance,
    HasStatusEffect
}

// A ScriptableObject to define a single consideration for scoring an AI action.
[CreateAssetMenu(fileName = "New AI Consideration", menuName = "Enemies/AI Consideration")]
public class AIAction : ScriptableObject
{
    [Header("Consideration")]
    [Tooltip("The type of condition to check.")]
    public ConsiderationType considerationType;

    [Header("Scoring")]
    [Tooltip("The curve that maps the consideration's value (x-axis, 0 to 1) to a score multiplier (y-axis).")]
    public AnimationCurve scoreCurve;

    // --- Optional Parameters for Specific Consideration Types ---
    [Header("Status Effect (Optional)")]
    [Tooltip("If 'HasStatusEffect' is the type, specify which effect to check for on the target.")]
    public StatusEffect statusEffect;

    /// <summary>
    /// Calculates the score for this specific consideration.
    /// MODIFIED: The method now accepts the new EnemyStateManager.
    /// </summary>
    /// <param name="controller">The enemy's state manager, which holds all contextual info.</param>
    /// <returns>A score multiplier (typically 0 to 1).</returns>
    public float Score(EnemyStateManager controller)
    {
        if (controller == null) return 0f;

        float normalizedValue = 0f;

        // We need a reference to the enemy's stats to calculate health percentage.
        // Let's get it from the EnemyStateManager's context.
        CharacterStatsBase stats = controller.GetComponent<CharacterStatsBase>();
        if (stats == null) return 0f;

        switch (considerationType)
        {
            case ConsiderationType.MyHealth:
                // MODIFIED: Calculate health percentage using the stats component.
                normalizedValue = stats.currentHealth / stats.maxHealth;
                break;

            case ConsiderationType.TargetDistance:
                // MODIFIED: Get distance and detection radius from the controller's public properties.
                float distance = Vector3.Distance(controller.transform.position, controller.PlayerTransform.position);
                normalizedValue = distance / controller.DetectionRadius;
                break;

            case ConsiderationType.HasStatusEffect:
                // This is a placeholder for a future implementation.
                break;
        }

        // Evaluate the normalized value on the curve to get the final score multiplier.
        return scoreCurve.Evaluate(Mathf.Clamp01(normalizedValue));
    }
}