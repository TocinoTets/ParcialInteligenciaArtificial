using UnityEngine;
using static UnityEditor.VersionControl.Asset;

public class IdleState : State
{
    [SerializeField] private float changePatronTimer = 10f;
    [SerializeField] float timer = 0;

    private PatrolData _data;//termine usando data con todos 
    private int NumberPosition;
    private int sentido = 1;


    public IdleState(PatrolData data, FMS strateMachine) : base(strateMachine)
    {

        _data = data;
    }
    public override void Enter()
    {
        timer = 0;
        Debug.Log("entre IdleState");
    }

    public override void Update()
    {
        PatroPingPong();
        timer += Time.deltaTime;
        if (timer >= changePatronTimer) 
        {
            
            StrateMachine.ChangeState(Estados.PatrolState);

            Debug.Log("cambio estado patrolState");
        }
    }

    public override void Exit()
    {
        Debug.Log("sali IdleState");
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
