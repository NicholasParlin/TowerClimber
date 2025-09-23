using UnityEngine;

[CreateAssetMenu(fileName = "New Change State Effect", menuName = "Gameplay Effects/Change State")]
public class ChangeStateEffect : GameplayEffect
{
    [Header("State Settings")]
    [Tooltip("The state to force the target character into.")]
    public CharacterState stateToApply;

    // UPDATED: Method signature now matches the base class.
    public override void Execute(Skill sourceSkill, GameObject caster, GameObject target)
    {
        CharacterStateManager targetStateManager = target.GetComponent<CharacterStateManager>();
        if (targetStateManager == null)
        {
            Debug.LogWarning($"ChangeStateEffect: Target {target.name} has no CharacterStateManager component.");
            return;
        }

        targetStateManager.ChangeState(stateToApply);
    }
}