using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class FMS 
{
    public State CurrentState {  get; private set; }
    //aca vamos a llamar a todos los posibles estados, es decir q se mueva de diferentes formas en este caso
    private Dictionary<Enum, State> state = new Dictionary<Enum, State>();

    public void RegisterState (Enum Key, State newStateFMS)//creo el diccionario para pasarlo a funciones
    {
        state[Key] = newStateFMS;
    }
    public void ChangeState(Enum Key)
    {
        State newState = state[Key];

        if (newState == CurrentState)
        {
            return;
        }
        CurrentState?.Exit(); 
        /* este ? me hace q si llegara a ser nulo el exit (por ejemplo cuando arranca q no tenes nada) se saltee esa parte*/
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}
