using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private float _destroyTrap = 5f;

    private static List<Trap> allTraps = new List<Trap>();
    public static IReadOnlyList<Trap> AllTraps => allTraps;


    private void OnEnable() { allTraps.Add(this); }
    private void OnDisable() { allTraps.Remove(this); }
    public void DestroyTrap()
    {
        Destroy(gameObject, _destroyTrap);
    }
}