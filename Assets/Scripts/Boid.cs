using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Boid : MonoBehaviour
{

    private bool isDead = false;
    [SerializeField] private Agent _targetHunter;
    [SerializeField] private float _HunterDetectionRange = 30f; // nueva variable
    [SerializeField] private GameObject _targetTrap;
    [SerializeField] private float _TrapDetectionRange = 20f; // nueva variable
    [SerializeField] private float _TrapExitRange = 15f;
    [SerializeField] private float _MaxSpeed = 5f;
    [SerializeField] private float _MaxSterring = 5f;
    [SerializeField] private float _SlowingDistance = 3f;
    [SerializeField] private float _MinDistance = 0.1f;

    //Flocking
    private Boid targetAgent;
    private static List<Boid> allAgents = new List<Boid>();
    [SerializeField] private float floatmaximumDetectionRange = 4f;
    [SerializeField] private float minimumSeparationDistance = 1.0f;
    [SerializeField] protected float alignment = 2f;
    //dar importancia 
    [SerializeField, Range(0f, 3f)] private float SeparationWeight = 1f;
    [SerializeField, Range(0f, 3f)] private float AlignmentWeight = 1f;
    [SerializeField, Range(0f, 3f)] private float CohesionWeight = 1f;

    private Vector3 _TargetPosition;
    [SerializeField] private Vector3 _velocity;
    public Vector3 Velocity => _velocity;

    public enum SteeringModes { Seek, Flee, Arrive, Pursuit, Evade, Flocking }
    public SteeringModes currentSterring;

    private void Awake()
    {
        Vector3 randomDireccion = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        _velocity += randomDireccion.normalized * _MaxSpeed;

        allAgents.Add(this);
    }

    private void Update()
    {
        if (isDead) return;
        _velocity.y = 0f;

        ActualizarEstadoSegunHunter(); // nuevo
        ActualizarEstadoSegunTrap(); // nuevo

        _velocity += sterringVector();// si lo descomento , los casazos se van a cualquier lado pero el casador anda con las demas funciones
        transform.position += _velocity * Time.deltaTime;

        if (_velocity != Vector3.zero)
        {
            transform.forward = _velocity;
        }
        transform.position = Bounds.Instance.OutOfBounds(transform.position);
    }

    private void ActualizarEstadoSegunHunter()
    {
        if (_targetHunter == null) return;
        float distance = Vector3.Distance(transform.position, _targetHunter.transform.position);
        //Debug.Log($"{name} - distancia al hunter: {distance} | rango: {_HunterDetectionRange} | estado actual: {currentSterring}");


        if (distance <= _HunterDetectionRange)
        {
            
            currentSterring = SteeringModes.Evade;
        }
        else if (currentSterring == SteeringModes.Evade)
        {
            currentSterring = SteeringModes.Flocking;
        }


    }
    private void ActualizarEstadoSegunTrap()
    {
        if (_targetTrap == null) return;
        if (currentSterring == SteeringModes.Evade) return;

        float distance = Vector3.Distance(transform.position, _targetTrap.transform.position);

        if (currentSterring != SteeringModes.Arrive && distance <= _TrapDetectionRange)
        {
            currentSterring = SteeringModes.Arrive;
        }
        else if (currentSterring == SteeringModes.Arrive && distance > _TrapExitRange)
        {
            currentSterring = SteeringModes.Flocking;
        }
    }
    private void OnDestroy()
    {
        allAgents.Remove(this);
    }


    private Vector3 sterringVector()
    {
        switch (currentSterring)
        {
            case SteeringModes.Seek:
                return Seek(_targetTrap.transform.position);

            case SteeringModes.Flee:
                return Flee(_targetHunter.transform.position);

            case SteeringModes.Arrive:
                return Arrive(_targetTrap.transform.position);

            case SteeringModes.Pursuit:
                return Pursuit(_targetHunter.transform.position);

            case SteeringModes.Evade:
                return Evade(_targetHunter);

            case SteeringModes.Flocking:
                return floking();
            default:
                return Vector3.zero;
        }
    }

    private Vector3 CalculateSteering(Vector3 desired)
    {
        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _MaxSterring * Time.deltaTime);
        return steering;
    }

    private Vector3 DesiredVector(Vector3 target)
    {
        Vector3 desired = (target - transform.position).normalized * _MaxSpeed;
        return desired;
    }

    private Vector3 Seek(Vector3 target)
    {
        Vector3 desired = DesiredVector(target);
        return CalculateSteering(desired);
    }

    private Vector3 Flee(Vector3 target)
    {
        Vector3 desired = DesiredVector(target);
        return CalculateSteering(-desired);
    }

    private Vector3 Arrive(Vector3 target)
    {
        Vector3 direction = target - transform.position;

        float distance = direction.magnitude;

        if (distance < _MinDistance)
        {
            return Vector3.zero;
        }

        float targetSpeed = _MaxSpeed * (distance / _SlowingDistance);
        float desiredSpeed = Mathf.Min(targetSpeed, _MaxSpeed);

        Vector3 desired = direction.normalized * desiredSpeed;
        Vector3 steering = CalculateSteering(desired);

        return CalculateSteering(desired);
    }
    private Vector3 Pursuit(Vector3 target)
    {
        Vector3 dir = (target - transform.position);
        float velocity = _MaxSpeed;
        float distance = dir.magnitude;

        if (distance <= _SlowingDistance)
        {
            float percentDistance = distance / _SlowingDistance;
            velocity *= percentDistance;
        }

        Vector3 desired = dir.normalized * velocity;
        return CalculateSteering(-desired);
    }
    private Vector3 Evade(Agent target)
    {
        var futurePosition = CalculateFuture(target);

        return Flee(futurePosition);
    }

    //floking como el profe lo hizo

    private Vector3 floking()
    {
        Vector3 desiredSeparation = CalculateSeparation();
        Vector3 desiredAlignment = CalculateAlignament();
        Vector3 desoredCohesion = CalculateCohesion();

        Vector3 desired = desiredSeparation * SeparationWeight + desiredAlignment * AlignmentWeight + desoredCohesion * CohesionWeight;

        if (desired == Vector3.zero)
            desired = _velocity;

        float distanceActual = Vector3.Distance(_targetTrap.transform.position, transform.position);

        return CalculateSteering(desired);

    }

    private Vector3 CalculateSeparation()
    {
        Vector3 dessired = default;
        int count = 0;
        foreach (var item in allAgents)
        {
            if (item == this) continue;
            if (Vector3.Distance(item.transform.position, transform.position) < minimumSeparationDistance)
            {
                dessired += (transform.position - item.transform.position).normalized;
                count++;
            }
        }
        if (count > 0)
            return (dessired / count).normalized * _MaxSpeed;
        return Vector3.zero;
    }

    private Vector3 CalculateAlignament()
    {
        Vector3 dessired = default;
        int count = 0;
        foreach (var item in allAgents)
        {
            if (item == this) continue;
            if (Vector3.Distance(item.transform.position, transform.position) < floatmaximumDetectionRange)
            {
                dessired += item._velocity;
                count++;
            }
        }
        if (count > 0)
            return (dessired / count).normalized * _MaxSpeed;
        return Vector3.zero;
    }

    private Vector3 CalculateCohesion()
    {
        Vector3 dessired = default;
        int count = 0;
        foreach (var item in allAgents)
        {
            if (item == this) continue;
            if (Vector3.Distance(item.transform.position, transform.position) < alignment)
            {
                dessired += item.transform.position;
                count++;
            }
        }
        if (count > 0)
            return Seek(dessired);
        return Vector3.zero;
    }
    private Vector3 CalculateFuture(Agent target)
    {
        Vector3 direction = target.transform.position - transform.position;

        float distance = direction.magnitude;

        var prediction = distance / (_MaxSpeed + target.Velocity.magnitude);

        Vector3 futurePosition = target.transform.position + target.Velocity * prediction;

        return futurePosition;
    }


    public void Die()
    {
        isDead = true;
        _velocity = Vector3.zero;
    }
}