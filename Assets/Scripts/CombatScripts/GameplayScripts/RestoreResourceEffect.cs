using UnityEngine;

public enum ResourceType
{
    Mana,
    Energy
}

[CreateAssetMenu(fileName = "New Restore Resource Effect", menuName = "Gameplay Effects/Restore Resource")]
public class RestoreResourceEffect : GameplayEffect
{
    [Header("Resource Settings")]
    [Tooltip("The type of resource to restore.")]
    public ResourceType resourceToRestore;

    [Tooltip("The amount of the resource to restore.")]
    public float amountToRestore;

    // UPDATED: Method signature now matches the base class.
    public override void Execute(Skill sourceSkill, GameObject caster, GameObject target)
    {
        PlayerStats targetStats = target.GetComponent<PlayerStats>();
        if (targetStats == null)
        {
            Debug.LogWarning($"RestoreResourceEffect: Target {target.name} has no PlayerStats component.");
            return;
        }

        switch (resourceToRestore)
        {
            case ResourceType.Mana:
                targetStats.RestoreMana(amountToRestore);
                Debug.Log($"{caster.name} restored {amountToRestore} mana for {target.name}.");
                break;
            case ResourceType.Energy:
                targetStats.RestoreEnergy(amountToRestore);
                Debug.Log($"{caster.name} restored {amountToRestore} energy for {target.name}.");
                break;
        }
    }
}