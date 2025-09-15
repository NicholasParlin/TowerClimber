using UnityEngine;

// This is a ScriptableObject, which means we can create "assets" from this script
// in our project folder. Each asset will represent a single, unique skill.
public abstract class Skill : ScriptableObject
{
    [Header("Skill Information")]
    public string skillName;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;
    public Archetype archetype; // The category this skill belongs to.

    [Header("Skill Timings")]
    [Tooltip("How long the skill takes to execute, during which other skills cannot be used.")]
    public float activationTime = 0.5f;
    [Tooltip("The time in seconds before this skill can be used again.")]
    public float cooldown = 1f;

    [Header("Resource Costs")]
    public int manaCost = 0;
    public int energyCost = 0;
    public int healthCost = 0;
    public int anguishCost = 0;

    /// <summary>
    /// The core logic of the skill. This method is called when the skill is used.
    /// It is 'abstract', meaning each specific skill type (like a damage or buff skill)
    /// MUST provide its own implementation for this method.
    /// </summary>
    /// <param name="user">The GameObject using the skill (the player or an enemy).</param>
    public abstract void Activate(GameObject user);
}