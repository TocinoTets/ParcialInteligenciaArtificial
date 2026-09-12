using UnityEngine;
using static UnityEditor.VersionControl.Asset;

public class IdleState : State
{
    [SerializeField] private float changePatronTimer = 1f;
    [SerializeField] float timer = 0;

    private PatrolData _data;//termine usando data con todos 
    private int NumberPosition;
    private int sentido = 1;


    public IdleState(FMS strateMachine) : base(strateMachine)
    {

        
    }
    public override void Enter()
    {
        timer = 0;
        Debug.Log("entre IdleState");
    }

    public override void Update()
    {
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

}
