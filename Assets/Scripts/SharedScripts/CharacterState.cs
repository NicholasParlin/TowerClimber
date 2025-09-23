// This enum defines all the possible action states a character can be in.
public enum CharacterState
{
    // Default & Movement States
    Idle,           // Not moving or performing any action.
    Moving,         // Walking or running.

    // Combat Action States
    ActivatingSkill, // The brief "wind-up" period for a skill.
    ActionInProgress,// For skills that lock the character into an animation for a duration (e.g., a dodge).
    ChannelingSkill, // For skills that have a sustained effect over time (e.g., a laser beam).
    Staggered,      // The character is briefly interrupted and cannot act.
    KnockedDown,    // The character is knocked to the ground and is vulnerable for a longer duration.
    Countered,      // The character has been hit by a counter-attack and is briefly stunned. NEW

    // Other States
    Dead            // The character has 0 health and is inactive.
}