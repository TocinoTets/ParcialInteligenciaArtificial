using UnityEngine;

public class PatrolPingPongState : State
{

    private PatrolData _data2;
    private int sentido = 1;
    private int NumberPosition;
    [SerializeField] float timer = 0;
    [SerializeField] private float changePatronTimer = 10f;



    public PatrolPingPongState(PatrolData data, FMS strateMachine) : base(strateMachine)
    {

        _data2 = data;
    }


    public override void Enter()
    {
        timer = 0;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        PatroPingPong();
        if (_data2.stop)
        {
            StrateMachine.ChangeState(Estados.ArriveState);
        }

        if (_data2.trampaPrefab != null && _data2.timer2 > _data2.trapTimer)
        {
            GameObject.Instantiate(_data2.trampaPrefab, _data2.transform.position, Quaternion.identity);
            _data2.timer2 = 0;
        }

        if (timer >= changePatronTimer)
        {
            StrateMachine.ChangeState(Estados.PatrolState);

        }
    }

    public override void Exit()
    {
    }




    private void PatroPingPong()
    {
        int numberOfPoints = _data2.listPositions.Count;

        var nextposition = _data2.listPositions[NumberPosition];
        if (Vector3.Distance(nextposition.position, _data2.transform.position) <= _data2.minDistans)
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


        var direccion = nextposition.position - _data2.transform.position;
        _data2.transform.position += direccion.normalized * _data2.velocity * Time.deltaTime;
    }
}
