using UnityEngine;

public abstract class State 
{
    protected FMS StrateMachine;//aca llamamos a lo de cambiar las maquinas de estado(state machin)

    protected State (FMS strateMachine)
    {
        StrateMachine = strateMachine;
    }
    //como state , es el q va a modificar cada estado (en este caso idle y patrol) hacemos q todos tenga la parte del change

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
