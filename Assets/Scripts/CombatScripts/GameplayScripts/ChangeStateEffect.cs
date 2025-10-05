using UnityEngine;

[CreateAssetMenu(fileName = "New Change State Effect", menuName = "Gameplay Effects/Change State")]
public class ChangeStateEffect : GameplayEffect
{
    [Header("State Settings")]
    [Tooltip("The state to force the target character into.")]
    public CharacterState stateToApply;

    // This field is new, to get a reference to the state factory
    private PlayerStateFactory _stateFactory;

    public override void Execute(Skill sourceSkill, GameObject caster, GameObject target)
    {
        CharacterStateManager targetStateManager = target.GetComponent<CharacterStateManager>();
        if (targetStateManager == null)
        {
            Debug.LogWarning($"ChangeStateEffect: Target {target.name} has no CharacterStateManager component.");
            return;
        }

        // Initialize the factory if we haven't already
        if (_stateFactory == null)
        {
            _stateFactory = new PlayerStateFactory(targetStateManager);
        }

        // Instead of calling ChangeState with an enum, we now get the correct state
        // object from the factory and call SwitchState.
        switch (stateToApply)
        {
            case CharacterState.Staggered:
                targetStateManager.SwitchState(_stateFactory.Staggered());
                break;
            case CharacterState.KnockedDown:
                targetStateManager.SwitchState(_stateFactory.KnockedDown());
                break;
            case CharacterState.Countered:
                targetStateManager.SwitchState(_stateFactory.Countered());
                break;
            // Add other cases as needed for states like Dead, etc.
            default:
                Debug.LogWarning($"ChangeStateEffect does not support switching to state: {stateToApply}");
                break;
        }
    }
}