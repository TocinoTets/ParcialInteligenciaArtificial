using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private static List<Trap> allTraps = new List<Trap>();
    public static IReadOnlyList<Trap> AllTraps => allTraps;

    private void OnEnable() { allTraps.Add(this); }
    private void OnDisable() { allTraps.Remove(this); }
}