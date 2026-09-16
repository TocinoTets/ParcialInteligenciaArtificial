using UnityEngine;

public class TrapFinder : MonoBehaviour
{
    [SerializeField] private float _TrapDetectionRange = 30f;

    public Trap FindNearestTrap()
    {
        Trap nearest = null;
        float nearestDist = float.MaxValue;

        foreach (Trap trap in Trap.AllTraps)
        {
            float dist = Vector3.Distance(transform.position, trap.transform.position);

            if (dist <= _TrapDetectionRange && dist < nearestDist)
            {
                nearest = trap;
                nearestDist = dist;
            }
        }

        return nearest;
    }
}