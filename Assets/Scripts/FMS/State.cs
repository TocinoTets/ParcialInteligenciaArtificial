using UnityEngine;

public abstract class State 
{
    protected FMS StrateMachine;

    protected State (FMS strateMachine)
    {
        StrateMachine = strateMachine;
    }

    public virtual void Enter()
    {

    }

    public virtual void Update() 
    { 

    }

    public virtual void Exit() 
    {

    }

}
