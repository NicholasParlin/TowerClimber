using UnityEngine;

public enum ForceDirection
{
    AwayFromCaster,
    TowardsCaster,
    Up
}

[CreateAssetMenu(fileName = "New Apply Force Effect", menuName = "Gameplay Effects/Apply Force")]
public class ApplyForceEffect : GameplayEffect
{
    [Header("Force Settings")]
    [Tooltip("The strength of the force to be applied.")]
    public float forceAmount = 10f;

    [Tooltip("The direction of the force.")]
    public ForceDirection direction = ForceDirection.AwayFromCaster;

    [Tooltip("The type of force to apply (Impulse for instant, Force for sustained).")]
    public ForceMode forceMode = ForceMode.Impulse;

    // UPDATED: Method signature now matches the base class.
    public override void Execute(Skill sourceSkill, GameObject caster, GameObject target)
    {
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        if (targetRigidbody == null)
        {
            Debug.LogWarning($"ApplyForceEffect: Target {target.name} has no Rigidbody component.");
            return;
        }

        Vector3 forceDirectionVector = Vector3.zero;

        switch (direction)
        {
            case ForceDirection.AwayFromCaster:
                forceDirectionVector = (target.transform.position - caster.transform.position).normalized;
                break;
            case ForceDirection.TowardsCaster:
                forceDirectionVector = (caster.transform.position - target.transform.position).normalized;
                break;
            case ForceDirection.Up:
                forceDirectionVector = Vector3.up;
                break;
        }

        targetRigidbody.AddForce(forceDirectionVector * forceAmount, forceMode);
    }
}