using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private bool isDead = false;

    [Header("Targets & Detection")]
    [SerializeField] private Agent _targetHunter;
    [SerializeField] private float _HunterDetectionRange = 30f;
    [SerializeField] private GameObject _targetTrap;
    [SerializeField] private float _TrapDetectionRange = 20f;
    [SerializeField] private float _MinDistance = 0.1f;

    [Header("Movement Settings")]
    [SerializeField] private float _MaxSpeed = 5f;
    [SerializeField] private float _MaxSteering = 5f;
    [SerializeField] private float _SlowingDistance = 3f;

    [Header("Flocking Settings")]
    private static List<Boid> allAgents = new List<Boid>();
    [SerializeField] private float floatmaximumDetectionRange = 4f;
    [SerializeField] private float minimumSeparationDistance = 1f;
    [SerializeField] private float alignment = 2f;

    [Header("Flocking Weights")]
    [SerializeField, Range(0f, 5f)] private float SeparationWeight = 1f;
    [SerializeField, Range(0f, 5f)] private float AlignmentWeight = 1f;
    [SerializeField, Range(0f, 5f)] private float CohesionWeight = 1f;
    [SerializeField, Range(0f, 10f)] private float EvadeWeight = 3f;
    [SerializeField, Range(0f, 10f)] private float TrapWeight = 2f;

    [SerializeField] private Vector3 _velocity;
    public Vector3 Velocity => _velocity;

    public enum SteeringModes { Seek, Flee, Arrive, Pursuit, Evade, Flocking }
    public SteeringModes currentSteering = SteeringModes.Flocking;

    private void Awake()
    {
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        _velocity = randomDirection.normalized * _MaxSpeed;

        allAgents.Add(this);
    }

    private void Update()
    {
        if (isDead) return;

        _velocity += GetSteeringForce();
        _velocity.y = 0f;

        _velocity = Vector3.ClampMagnitude(_velocity, _MaxSpeed);

        transform.position += _velocity * Time.deltaTime;

        if (_velocity != Vector3.zero)
        {
            transform.forward = _velocity;
        }

        if (Bounds.Instance != null)
        {
            transform.position = Bounds.Instance.OutOfBounds(transform.position);
        }
    }

    private void OnDestroy()
    {
        allAgents.Remove(this);
    }

    private Vector3 GetSteeringForce()
    {
        switch (currentSteering)
        {
            case SteeringModes.Seek:
                if (_targetTrap == null) return Vector3.zero;
                return CalculateSteering(Seek(_targetTrap.transform.position));

            case SteeringModes.Flee:
                if (_targetHunter == null) return Vector3.zero;
                return CalculateSteering(Flee(_targetHunter.transform.position));

            case SteeringModes.Arrive:
                if (_targetTrap == null) return Vector3.zero;
                return CalculateSteering(Arrive(_targetTrap.transform.position));

            case SteeringModes.Pursuit:
                if (_targetHunter == null) return Vector3.zero;
                return CalculateSteering(Pursuit(_targetHunter));

            case SteeringModes.Evade:
                if (_targetHunter == null) return Vector3.zero;
                return CalculateSteering(Evade(_targetHunter));

            case SteeringModes.Flocking:
                return Flocking();

            default:
                return Vector3.zero;
        }
    }

    private Vector3 Flocking()
    {
        Vector3 desiredVelocity = Vector3.zero;

        // 1. Fuerzas base de Flocking
        desiredVelocity += CalculateSeparation() * SeparationWeight
            + CalculateAlignment() * AlignmentWeight
            + CalculateCohesion() * CohesionWeight;

        // esto anda bien el tema es cuando lo pones en una funcion y lo llamas desde el update,( ahi se rompe todo, no se porque)lo que esta entre parantesis no lo escribi yo lo dijo copilot
        if (_targetHunter != null && Vector3.Distance(transform.position, _targetHunter.transform.position) <= _HunterDetectionRange)
        {
            desiredVelocity += Evade(_targetHunter) * EvadeWeight;
        }

        //aca deberia cambiar de estado a arrive
        if (_targetTrap != null && Vector3.Distance(transform.position, _targetTrap.transform.position) <= _TrapDetectionRange)
        {
            desiredVelocity += Arrive(_targetTrap.transform.position) * TrapWeight;
        }

        if (desiredVelocity == Vector3.zero)
        {
            desiredVelocity = _velocity;
        }

        return CalculateSteering(desiredVelocity);
    }

    private Vector3 CalculateSteering(Vector3 desired)
    {
        Vector3 steering = desired - _velocity;
        return Vector3.ClampMagnitude(steering, _MaxSteering * Time.deltaTime);
    }

    /*/comportamientos individuales/*/
    private Vector3 Seek(Vector3 target)
    {
        return (target - transform.position).normalized * _MaxSpeed;
    }

    private Vector3 Flee(Vector3 target)
    {
        return (transform.position - target).normalized * _MaxSpeed;
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

    private Vector3 Pursuit(Agent target)
    {
        Vector3 futurePosition = CalculateFuture(target);
        return Seek(futurePosition);
    }

    private Vector3 Evade(Agent target)
    {
        Vector3 futurePosition = CalculateFuture(target);
        return Flee(futurePosition);
    }

    private Vector3 CalculateSeparation()
    {
        Vector3 desired = Vector3.zero;
        int count = 0;

        foreach (var item in allAgents)
        {
            if (item == this || item.isDead) continue; // Ignora agentes muertos
            float dist = Vector3.Distance(item.transform.position, transform.position);

            if (dist < minimumSeparationDistance && dist > 0)
            {
                desired += (transform.position - item.transform.position).normalized;
                count++;
            }
        }

        if (count > 0) return (desired / count).normalized * _MaxSpeed;
        return Vector3.zero;
    }

    private Vector3 CalculateAlignment()
    {
        Vector3 desired = Vector3.zero;
        int count = 0;

        foreach (var item in allAgents)
        {
            if (item == this || item.isDead) continue; // Ignora agentes muertos
            if (Vector3.Distance(item.transform.position, transform.position) < floatmaximumDetectionRange)
            {
                desired += item._velocity;
                count++;
            }
        }

        if (count > 0) return (desired / count).normalized * _MaxSpeed;
        return Vector3.zero;
    }

    private Vector3 CalculateCohesion()
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        foreach (var item in allAgents)
        {
            if (item == this || item.isDead) continue; // Ignora agentes muertos
            if (Vector3.Distance(item.transform.position, transform.position) < alignment)
            {
                centerOfMass += item.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            centerOfMass /= count;
            return Seek(centerOfMass);
        }

        return Vector3.zero;
    }

    private Vector3 CalculateFuture(Agent target)
    {
        Vector3 direction = target.transform.position - transform.position;
        float distance = direction.magnitude;
        float prediction = distance / (_MaxSpeed + target.Velocity.magnitude);

        return target.transform.position + target.Velocity * prediction;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        _velocity = Vector3.zero;

        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        yield return new WaitForSeconds(10f);

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }

        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }

        isDead = false;


    }
}