using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PatrolState : State
{
    [SerializeField] private float changePatronTimer = 10f;
    [SerializeField] float timer = 0;


    private PatrolData _data;
    private int NumberPosition;


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
        PatrollingLoop();
        timer += Time.deltaTime;
        if (timer >= changePatronTimer)
        {

            StrateMachine.ChangeState(Estados.IdleState);

            Debug.Log("cambio estado idlestate");
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
       
}


[System.Serializable]
public class PatrolData
{
    public List<Transform> listPositions;
    public Transform transform;
    public float minDistans;
    public float velocity;
}
