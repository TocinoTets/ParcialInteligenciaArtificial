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
        _data.timer2 = 0;
    }

    public override void Update()
    {

        PatrollingLoop();
        // imagen de caminar 
        if (_data.stop)
        {
            StrateMachine.ChangeState(Estados.ArriveState);
        }

        timer += Time.deltaTime;
        _data.timer2 += Time.deltaTime;


        if (_data.trampaPrefab != null && _data.timer2 > _data.trapTimer)
        {
            GameObject.Instantiate(_data.trampaPrefab, _data.transform.position, Quaternion.identity);
            _data.timer2 = 0;
        }

        if (timer >= changePatronTimer)
        {

            StrateMachine.ChangeState(Estados.PatrolPingPongState);

        }

    }

    public override void Exit()
    {
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
    public float trapTimer = 5f;
    public float timer2 = 0;
    public GameObject trampaPrefab;
    public GameObject bullet;
    public Transform targetDead;

    public List<Transform> listPositions;
    public Transform transform;
    public float minDistans;
    public float velocity;
    public bool stop;

}
