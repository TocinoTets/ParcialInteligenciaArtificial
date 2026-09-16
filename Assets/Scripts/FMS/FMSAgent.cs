using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Estados
{
    ArriveState,
    PatrolState,
    PatrolPingPongState,
    IdleState,
    StrokeState
}

public class FMSAgent : MonoBehaviour
{
    [SerializeField] private FMS steamMachine;

    [SerializeField] private PatrolData patrolData;
    [SerializeField] private IdleState idleData;
    [SerializeField] private ArriveState arriveState;

    [SerializeField] private PatrolPingPongState patrolPingPongState;
    [SerializeField] private StrokeState strokeState;

    private Vector3 posicionAnterior;


    private void Awake()
    {
        steamMachine = new FMS();
        patrolData.transform = this.transform; // ✅ inicializar transform

        PatrolState patrolState = new PatrolState(patrolData, steamMachine);
        patrolPingPongState = new PatrolPingPongState(patrolData, steamMachine);
        arriveState = new ArriveState(patrolData, steamMachine);
        idleData = new IdleState(patrolData, steamMachine);

        // Registrar solo estados de movimiento
        steamMachine.RegisterState(Estados.PatrolState, patrolState);
        steamMachine.RegisterState(Estados.PatrolPingPongState, patrolPingPongState);
        steamMachine.RegisterState(Estados.ArriveState, arriveState);
        steamMachine.RegisterState(Estados.IdleState, idleData);

        steamMachine.ChangeState(Estados.PatrolState);
        posicionAnterior = transform.position;

        // Stroke se instancia, pero no se registra como estado
        strokeState = new StrokeState(patrolData, steamMachine);
    }


    private void Update()
    {
        // FSM maneja movimiento
        steamMachine.Update();

        // Stroke corre en paralelo
        strokeState.Update();

        // Orientación del agente
        Vector3 direccion = transform.position - posicionAnterior;
        direccion.y = 0;
        if (direccion != Vector3.zero)
        {
            transform.forward = direccion;
        }
        posicionAnterior = transform.position;
    }
}
