using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Estados//aca estoy llamando a los estados q tengo 
{
    PatrolState,
    IdleState
}


public class FMSAgent : MonoBehaviour
{


    //maquina de estado 
    [SerializeField] private FMS steamMachine;
    //lo q le paso esta relacionado a esta maquina de estado, si tubiera otra tendria q llamarla
    [SerializeField] private PatrolData patrolData;
    [SerializeField] private IdleState idleData;


    private void Awake()
    {
        steamMachine = new FMS();
        IdleState idleState = new IdleState(patrolData, steamMachine);
        PatrolState patrolState = new PatrolState(patrolData, steamMachine);

        steamMachine.RegisterState(Estados.IdleState, idleState);
        //aca lo q hago es como registrar q el estado Estados.IdleState es el estaod idleState para llamarlo despues
        steamMachine.RegisterState(Estados.PatrolState, patrolState);
        steamMachine.ChangeState(Estados.IdleState);
        // steamMachine.ChangeState(Estados.PatrolState);//si a este lo comento , nunca pasa a esta maquina
    }


    private void Update()
    {
        steamMachine.Update();
    }

}



    //estos 2 los lleve al PatrolState q tiene los comportamientos de las maquinas de estado
    /*
    private void PatrollingLoop()
    {
        int numberOfPoints = listPositions.Count;

        var nextposition = listPositions[NumberPosition];
        if (Vector3.Distance(nextposition.position, transform.position) <= minDistans)
        {
            NumberPosition = NumberPosition + 1 < listPositions.Count ? NumberPosition + 1 : 0;
        }

        var direccion = nextposition.position - transform.position;
        transform.position += direccion.normalized * velocity * Time.deltaTime;
    } 

    private void PatroPingPong()
    {
        int numberOfPoints = listPositions.Count;

        var nextposition = listPositions[NumberPosition];
        if (Vector3.Distance(nextposition.position, transform.position) <= minDistans)
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


        var direccion = nextposition.position - transform.position;
        transform.position += direccion.normalized * velocity * Time.deltaTime;
    }*/

