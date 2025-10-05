using System;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(PlayerMovement), typeof(PlayerStats))]
public class CharacterStateManager : MonoBehaviour
{
    // --- State Machine ---
    private PlayerBaseState _currentState;
    private PlayerStateFactory _states;

    // --- Properties for States to Access ---
    // These act as the "context" for the individual state classes.
    public PlayerBaseState CurrentState => _currentState;
    public Animator Animator { get; private set; }
    public PlayerMovement PlayerMovement { get; private set; }
    public bool IsMovementPressed { get; set; }
    public float CurrentActionTime { get; set; } // Set by PlayerSkillManager before activating a skill
    public Skill CurrentSkillInUse { get; set; } // Set by PlayerSkillManager to check for super armor

    private void Awake()
    {
        // Get references to all necessary components
        Animator = GetComponent<Animator>();
        PlayerMovement = GetComponent<PlayerMovement>();

        // Setup the State Machine
        _states = new PlayerStateFactory(this);
        _currentState = _states.Idle(); // Start in the Idle state
        _currentState.EnterState();
    }

    private void Start()
    {
        // Subscribe to the stats update event to check for death
        GetComponent<PlayerStats>().OnStatsUpdated += CheckForDeath;
    }

    private void OnDestroy()
    {
        // Always unsubscribe from events
        GetComponent<PlayerStats>().OnStatsUpdated -= CheckForDeath;
    }

    private void Update()
    {
        // The core of the state machine: simply call the current state's Update method.
        _currentState.UpdateState();
    }

    /// <summary>
    /// The main method for transitioning between states.
    /// </summary>
    public void SwitchState(PlayerBaseState newState)
    {
        // Call the Exit method of the current state
        _currentState.ExitState();

        // Set the new state and call its Enter method
        _currentState = newState;
        _currentState.EnterState();
    }

    /// <summary>
    /// This public method is called by Animation Events at the end of clips
    /// like Stagger, Knockdown, etc., to return the character to a neutral state.
    /// </summary>
    public void ReturnToIdleState()
    {
        SwitchState(_states.Idle());
    }

    private void CheckForDeath()
    {
        // If health is 0 and we are not already in the Dead state, switch to it.
        if (GetComponent<PlayerStats>().currentHealth <= 0 && !(_currentState is PlayerDeadState))
        {
            SwitchState(_states.Dead());
        }
    }
}