using UnityEngine;
using System.Collections.Generic;

// An enum to define the shape of the melee attack's hitbox.
public enum HitboxShape
{
    Sphere,
    Box
}

// This effect projects a shape in front of the caster and applies a list of effects to all valid targets hit.
[CreateAssetMenu(fileName = "New Melee Attack Effect", menuName = "Gameplay Effects/Melee Attack")]
public class MeleeAttackEffect : GameplayEffect
{
    [Header("Hit Detection")]
    [Tooltip("The shape of the hit detection area.")]
    public HitboxShape shape = HitboxShape.Sphere;
    [Tooltip("The layer(s) that are considered valid targets.")]
    public LayerMask targetLayers;

    [Header("Shape Properties")]
    [Tooltip("The center of the shape, offset from the caster's position.")]
    public Vector3 offset;
    [Tooltip("The radius of the sphere if Shape is set to Sphere.")]
    public float radius = 1.5f;
    [Tooltip("The dimensions of the box if Shape is set to Box.")]
    public Vector3 boxHalfExtents = new Vector3(1f, 1f, 1.5f);

    [Header("Effects to Apply")]
    [Tooltip("The list of effects to apply to every target hit by this attack.")]
    public List<GameplayEffect> effectsToApplyOnHit;

    public override void Execute(Skill sourceSkill, GameObject caster, GameObject target)
    {
        // The 'target' parameter is ignored here, as a melee attack finds its own targets.
        Collider[] hits = DetectHits(caster);

        if (hits.Length > 0)
        {
            Debug.Log($"Melee attack hit {hits.Length} targets.");
            foreach (Collider hit in hits)
            {
                // Apply all assigned effects to each target that was hit.
                foreach (GameplayEffect effect in effectsToApplyOnHit)
                {
                    effect.Execute(sourceSkill, caster, hit.gameObject);
                }
            }
        }
    }

    private Collider[] DetectHits(GameObject caster)
    {
        Vector3 position = caster.transform.position + caster.transform.TransformDirection(offset);
        Quaternion rotation = caster.transform.rotation;

        switch (shape)
        {
            case HitboxShape.Sphere:
                return Physics.OverlapSphere(position, radius, targetLayers);
            case HitboxShape.Box:
                return Physics.OverlapBox(position, boxHalfExtents, rotation, targetLayers);
        }
        return new Collider[0];
    }
}