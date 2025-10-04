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
    /// </summary>
    /// <param name="controller">The enemy AI controller.</param>
    /// <returns>A score multiplier (typically 0 to 1).</returns>
    public float Score(EnemyController controller)
    {
        if (controller == null) return 0f;

        float normalizedValue = 0f;

        switch (considerationType)
        {
            case ConsiderationType.MyHealth:
                normalizedValue = controller.GetHealthPercentage();
                break;

            case ConsiderationType.TargetDistance:
                normalizedValue = controller.GetDistanceToTarget() / controller.GetDetectionRadius();
                break;

            case ConsiderationType.HasStatusEffect:
                // This is a placeholder for a future implementation.
                // You would need to add a way to check a target's active status effects.
                // For now, it will return 0.
                break;
        }

        // Evaluate the normalized value on the curve to get the final score multiplier.
        return scoreCurve.Evaluate(Mathf.Clamp01(normalizedValue));
    }
}