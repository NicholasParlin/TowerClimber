using UnityEngine;

public abstract class GameplayEffect : ScriptableObject
{
    [Header("Effect Description")]
    [TextArea] public string description;

    /// <summary>
    /// Executes the effect's logic.
    /// </summary>
    /// <param name="sourceSkill">The Skill asset that initiated this effect.</param>
    /// <param name="caster">The GameObject initiating the effect.</param>
    /// <param name="target">The primary GameObject being affected.</param>
    public abstract void Execute(Skill sourceSkill, GameObject caster, GameObject target);
}