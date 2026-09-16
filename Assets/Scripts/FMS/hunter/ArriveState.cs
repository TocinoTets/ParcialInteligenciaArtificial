using UnityEngine;

public class ArriveState : State
{
    private PatrolData _data;

    public ArriveState(PatrolData data, FMS strateMachine) : base(strateMachine)
    {
        _data = data;
    }

    public override void Enter()
    {
        _data._renderer.material = _data.grab;
    }

    public override void Update()
    {
   
        if (_data.targetDead == null)
        {
            StrateMachine.ChangeState(Estados.PatrolState);
            return;
        }

        Vector3 direction = _data.targetDead.position - _data.transform.position;
        float distance = direction.magnitude;
        float velocity = _data.velocity;


        if (distance <= _data.minDistans)
        {
            StrateMachine.ChangeState(Estados.IdleState);
        }

        Vector3 desired = direction.normalized * velocity;
        _data.transform.position += desired * Time.deltaTime;
    }

    public override void Exit()
    {
    }
}