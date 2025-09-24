using System;
using UnityEngine;

// This component manages the character's current state and enforces the rules of combat.
// It communicates with the Animator and other components to control character behavior.
[RequireComponent(typeof(Animator), typeof(CharacterStatsBase))]
public class CharacterStateManager : MonoBehaviour
{
    // An event that fires whenever the state changes, allowing other scripts to react.
    public event Action<CharacterState> OnStateChanged;

    // Public properties to allow other scripts to easily check the character's status.
    public CharacterState CurrentState { get; private set; }
    public bool CanTakeStaggerDamage => CurrentState != CharacterState.Staggered && CurrentState != CharacterState.KnockedDown && CurrentState != CharacterState.Countered;
    public bool CanAct => CurrentState == CharacterState.Idle || CurrentState == CharacterState.Moving;

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
    /// The central method for changing the character's state. This enforces all game rules.
    /// </summary>
    public void ChangeState(CharacterState newState)
    {
        if (CurrentState == newState) return;

        // --- State Transition Logic ---
        // Handle the re-stagger mechanic
        if (CurrentState == CharacterState.Staggered && newState == CharacterState.KnockedDown)
        {
            Debug.Log("Stagger interrupted by Knockdown!");
        }
        // Prevent changing state if in a non-interruptible state (unless it's a stagger, knockdown, or death)
        else if (!CanAct && newState != CharacterState.Staggered && newState != CharacterState.KnockedDown && newState != CharacterState.Dead)
        {
            Debug.Log($"Cannot change state from {CurrentState} to {newState}. Action is in progress.");
            return;
        }

        CurrentState = newState;
        OnStateChanged?.Invoke(newState); // Fire the event for other scripts to hear.

        // --- Animation Triggering ---
        // This is where you would trigger the animations for each state.
        switch (newState)
        {
            case CharacterState.Idle:
                // Example: _animator.SetBool("IsMoving", false);
                break;
            case CharacterState.Moving:
                // Example: _animator.SetBool("IsMoving", true);
                break;
            case CharacterState.ActivatingSkill:
                // The skill manager will likely trigger the specific skill animation.
                break;
            case CharacterState.Staggered:
                _animator.SetTrigger("StaggerTrigger");
                break;
            case CharacterState.KnockedDown:
                _animator.SetTrigger("KnockdownTrigger");
                break;
            case CharacterState.Countered:
                _animator.SetTrigger("CounteredTrigger");
                break;
            case CharacterState.Dead:
                _animator.SetTrigger("DeathTrigger");
                break;
        }
    }

    /// <summary>
    /// This public method is intended to be called by an Animation Event at the end of
    /// a state's animation clip (e.g., Staggered, KnockedDown, Countered) to return the character to Idle.
    /// </summary>
    public void ReturnToIdleState()
    {
        ChangeState(CharacterState.Idle);
    }
}