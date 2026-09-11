using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PatrolState : State
{

    public enum PatrolMode { Loop, PingPong }
    [SerializeField] private float changePatronTimer = 10f;
    [SerializeField] float timer = 0;
    [SerializeField] private PatrolMode mode = PatrolMode.Loop;
    [SerializeField] private bool Change = true;
    private PatrolData _data;
    private int NumberPosition;
    private int sentido = 1;


    public PatrolState(PatrolData data, FMS strateMachine) : base(strateMachine)
    {
      
        _data = data;
    }
    public override void Enter()
    {
        timer = 0;
        Debug.Log("entre PatrolState ");
    }

    public override void Update()
    {
        // Ejecuta el modo actual
        if (mode == PatrolMode.Loop)
        {
            PatrollingLoop();
        }
        else if (mode == PatrolMode.PingPong)
        {
            PatroPingPong();
        }

        // Avanza el tiempo
        timer += Time.deltaTime;

        // Cambia de modo cuando se cumple el tiempo
        if (timer >= changePatronTimer)
        {
            mode = mode == PatrolMode.Loop ? PatrolMode.PingPong : PatrolMode.Loop;
            timer = 0;
            Debug.Log("Cambio de modo a: " + mode);
        }
    }

    public override void Exit()
    {
        Debug.Log("sali PatrolState");
    }


    private void PatrollingLoop()
    {
        int numberOfPoints = _data.listPositions.Count;

        var nextposition = _data.listPositions[NumberPosition];
        if (Vector3.Distance(nextposition.position, _data.transform.position) <= _data.minDistans)
        {
            NumberPosition = NumberPosition + 1 < _data.listPositions.Count ? NumberPosition + 1 : 0;
        }

        var direccion = nextposition.position - _data.transform.position;
        _data.transform.position += direccion.normalized * _data.velocity * Time.deltaTime;
    }

    private void PatroPingPong()
    {
        int numberOfPoints = _data.listPositions.Count;

        var nextposition = _data.listPositions[NumberPosition];
        if (Vector3.Distance(nextposition.position, _data.transform.position) <= _data.minDistans)
        {
            if (NumberPosition == numberOfPoints - 1)
            {
                sentido = -1;
            }
            else if (NumberPosition == 0)
            {
                sentido = 1;
            }
            NumberPosition += sentido;
        }


        var direccion = nextposition.position - _data.transform.position;
        _data.transform.position += direccion.normalized * _data.velocity * Time.deltaTime;
    }



}


[System.Serializable]
public class PatrolData
{
    public List<Transform> listPositions;
    public Transform transform;
    public float minDistans;
    public float velocity;
}
