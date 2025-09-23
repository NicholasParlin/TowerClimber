// This is a simple, reusable helper class for defining a stat bonus in the Inspector.
// It is not a MonoBehaviour.
[System.Serializable]
public class StatBonus
{
    public StatType statToBuff; // e.g., Strength, Dexterity
    public float value;         // e.g., 5, -10
    public ModifierType type = ModifierType.Flat; // e.g., Flat, Percentage
}
