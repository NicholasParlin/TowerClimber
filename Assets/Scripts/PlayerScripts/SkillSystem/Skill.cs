using UnityEngine;

// This is the abstract base class for all skills. We will create specific skill types
// that inherit from this.
public abstract class Skill : ScriptableObject
{
    [Header("Core Information")]
    public string skillName;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;
    public Archetype archetype;

    [Header("Skill Type")]
    [Tooltip("Check this if the skill is a toggleable passive. Otherwise, it's considered an active skill.")]
    public bool isPassive;

    [Header("Mechanics")]
    public float activationTime = 0.5f;
    public float cooldown = 1f;

    [Header("Resource Costs")]
    public float manaCost = 0;
    public float energyCost = 0;
    public float healthCost = 0;
    public float anguishCost = 0;

    // This method defines what happens when the skill is used.
    public abstract void Activate(GameObject user);
}