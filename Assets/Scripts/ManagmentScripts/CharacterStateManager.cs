using UnityEngine;

// This component manages the character's current state and communicates with the Animator.
[RequireComponent(typeof(Animator), typeof(CharacterStatsBase))]
public class CharacterStateManager : MonoBehaviour
{
    // Public property to allow other scripts to read the current state.
    public CharacterState CurrentState { get; private set; }

    // Component references
    private Animator _animator;
    private CharacterStatsBase _stats;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _stats = GetComponent<CharacterStatsBase>();
        CurrentState = CharacterState.Idle;
    }

    /// <summary>
    /// The central method for changing the character's state.
    /// </summary>
    public void ChangeState(CharacterState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        Debug.Log($"Character entered state: {newState}");

        // In a full implementation, you would use a switch statement here
        // to trigger the correct animation based on the new state.
        // For example:
        // switch (newState)
        // {
        //     case CharacterState.Staggered:
        //         _animator.SetTrigger("StaggerTrigger");
        //         break;
        //     case CharacterState.KnockedDown:
        //         _animator.SetTrigger("KnockdownTrigger");
        //         break;
        // }
    }

    // In a full implementation, you would have methods that are called by Animation Events
    // at the end of an animation clip to return the character to the Idle state.
    // For example:
    // public void ReturnToIdleState()
    // {
    //     ChangeState(CharacterState.Idle);
    // }
}