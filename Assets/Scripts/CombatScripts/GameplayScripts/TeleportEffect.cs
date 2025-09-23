using UnityEngine;

public enum TeleportType
{
    Forward,
    ToTarget,
    GroundArea
}

[CreateAssetMenu(fileName = "New Teleport Effect", menuName = "Gameplay Effects/Teleport")]
public class TeleportEffect : GameplayEffect
{
    [Header("Teleport Settings")]
    [Tooltip("The method of teleportation.")]
    public TeleportType type = TeleportType.Forward;

    [Tooltip("The distance to teleport if using the 'Forward' type.")]
    public float distance = 10f;

    // UPDATED: Method signature now matches the base class.
    public override void Execute(Skill sourceSkill, GameObject caster, GameObject target)
    {
        CharacterController controller = caster.GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError($"TeleportEffect: Caster {caster.name} is missing a CharacterController component.");
            return;
        }

        Vector3 destination = Vector3.zero;

        switch (type)
        {
            case TeleportType.Forward:
                destination = caster.transform.position + (caster.transform.forward * distance);
                break;

            case TeleportType.ToTarget:
                if (target == null)
                {
                    Debug.LogWarning("TeleportEffect: 'ToTarget' type was used but no target was provided.");
                    return;
                }
                destination = target.transform.position;
                break;

            case TeleportType.GroundArea:
                Debug.Log("Ground Area teleport needs a ground targeting system to be implemented.");
                return;
        }

        controller.enabled = false;
        caster.transform.position = destination;
        controller.enabled = true;

        Debug.Log($"{caster.name} teleported.");
    }
}