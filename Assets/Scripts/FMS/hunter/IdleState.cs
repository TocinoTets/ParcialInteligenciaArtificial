using UnityEngine;
using static UnityEditor.VersionControl.Asset;

public class IdleState : State
{
    [SerializeField] private float changePatronTimer = 5f;
    [SerializeField] float timer = 0;

    public Transform transform;
    private PatrolData _data;

    public IdleState(PatrolData data, FMS strateMachine) : base(strateMachine)
    {
        _data = data;
    }

    public override void Enter()
    {
        timer = 0;
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        Boid boid = _data.targetDead.GetComponent<Boid>();
        if (timer >= changePatronTimer)
        {
            boid.kill();
            _data.stop = false;
            StrateMachine.ChangeState(Estados.PatrolState);
        }
    }

    public override void Exit()
    {
    }

}
