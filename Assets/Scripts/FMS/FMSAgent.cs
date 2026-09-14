using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Estados
{
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
    [SerializeField] private PatrolPingPongState patrolPingPongState;
    [SerializeField] private StrokeState strokeState;

    // Guarda la posición anterior del agente
    private Vector3 posicionAnterior;


    private void Awake()
    {
        steamMachine = new FMS();

        idleData = new IdleState(steamMachine);
        PatrolState patrolState = new PatrolState(patrolData, steamMachine);
        patrolPingPongState = new PatrolPingPongState(patrolData, steamMachine);
        strokeState = new StrokeState(patrolData, steamMachine); // ahora sí inicializa el campo

        steamMachine.RegisterState(Estados.IdleState, idleData);
        steamMachine.RegisterState(Estados.PatrolState, patrolState);
        steamMachine.RegisterState(Estados.PatrolPingPongState, patrolPingPongState);
        steamMachine.RegisterState(Estados.StrokeState, strokeState);

        steamMachine.ChangeState(Estados.IdleState);

        posicionAnterior = transform.position;
    }


    private void Update()
    {
        strokeState.Update();
        steamMachine.Update();

        Vector3 direccion = transform.position - posicionAnterior;

        direccion.y = 0;

        if (direccion != Vector3.zero)
        {
            transform.forward = direccion;
        }

        posicionAnterior = transform.position;
    }
}
