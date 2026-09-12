using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Estados//aca estoy llamando a los estados q tengo 
{
    PatrolState,
    PatrolPingPongState,
    IdleState,
    StrokeState

}


public class FMSAgent : MonoBehaviour
{


    [SerializeField] private FMS steamMachine;
    //lo q le paso esta relacionado a esta maquina de estado, si tubiera otra tendria q llamarla
    [SerializeField] private PatrolData patrolData;
    [SerializeField] private IdleState idleData;
    [SerializeField] private PatrolPingPongState patrolPingPongState;
    [SerializeField] private StrokeState strokeState;


    private void Awake()
    {
        steamMachine = new FMS();
        IdleState idleState = new IdleState(steamMachine);
        PatrolState patrolState = new PatrolState(patrolData, steamMachine);
        PatrolPingPongState patrolPingPongState = new PatrolPingPongState (patrolData, steamMachine);
        StrokeState strokeState = new StrokeState(patrolData, steamMachine);


        steamMachine.RegisterState(Estados.IdleState, idleState);
        steamMachine.RegisterState(Estados.PatrolState, patrolState);
        steamMachine.RegisterState(Estados.PatrolPingPongState, patrolPingPongState);
        steamMachine.RegisterState(Estados.StrokeState, strokeState);
        steamMachine.ChangeState(Estados.IdleState);

    }


    private void Update()
    {
        steamMachine.Update();
    }

}