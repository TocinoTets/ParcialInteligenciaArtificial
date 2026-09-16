using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private float _destroyTrap = 5f;

    private static List<Trap> allTraps = new List<Trap>();
    public static IReadOnlyList<Trap> AllTraps => allTraps;

    // Guarda qué Boid tiene reservada esta trampa
    public Boid Targeter { get; private set; }
    public bool IsTargeted => Targeter != null;

    private void OnEnable() { allTraps.Add(this); }
    private void OnDisable() { allTraps.Remove(this); }

    public bool Claim(Boid boid)
    {
        // Se puede reclamar si está libre o si ya la tiene este mismo Boid
        if (Targeter == null || Targeter == boid)
        {
            Targeter = boid;
            return true;
        }
        return false;
    }

    public void Release(Boid boid)
    {
        if (Targeter == boid)
        {
            Targeter = null;
        }
    }

    public void DestroyTrap()
    {
        Destroy(gameObject, _destroyTrap);
    }
}