using UnityEngine;

public class TrapFinder : MonoBehaviour
{
    [SerializeField] private float _TrapDetectionRange = 30f;

    public Trap FindNearestTrap(Boid requester)
    {
        Trap nearest = null;
        float nearestDist = float.MaxValue;

        foreach (Trap trap in Trap.AllTraps)
        {
            // Si la trampa está ocupada por OTRO Boid, la ignoramos
            if (trap.IsTargeted && trap.Targeter != requester)
                continue;

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