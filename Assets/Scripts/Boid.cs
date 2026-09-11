using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using RangeAttribute = UnityEngine.RangeAttribute;

public class AdvanceAgent : Agent
{
    [Header("Stats")]
    [SerializeField] private float _maxSpeed = 3f;
    [SerializeField] private float _maxStreering = 3f;
    [SerializeField] private float _slowingDistance = 3f;
    [SerializeField] private float _minDistance = 0.1f;
    //[SerializeField] private float _maxDistance = 5f;

    private static List<Agent> allAgents = new List<Agent>();
    [SerializeField] private float _separetionRadius = 2f;
    [SerializeField] private float _alignmentRadius = 2f;
    [SerializeField] private float _cohesionRadius = 2f;

    [SerializeField, Range(0f, 3f)] private float _separetionWeight = 1f;
    [SerializeField, Range(0f, 3f)] private float _alignmentWeight = 1f;
    [SerializeField, Range(0f, 3f)] private float _cohesionWeight = 1f;

    [Header("References")]
    [SerializeField] private Agent _target;
    [SerializeField] private Transform _targetTrap;

    public enum SteeringModes { Seek, Flee, Arrive, Pursuit, Evade, Flocking }
    public SteeringModes currentSteering;

    private void Awake()
    {
        allAgents.Add(this);
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        _velocity += randomDirection.normalized * _maxSpeed;

    }

    private void Update()
    {
        _velocity += SteeringVector();

        transform.position += _velocity * Time.deltaTime;

        if (_velocity != Vector3.zero)
        {
            transform.forward = _velocity;
        }


        transform.position = Bounds.Instance.OutOfBounds(transform.position);
    }

    private Vector3 SteeringVector()
    {
        switch (currentSteering)
        {
            case SteeringModes.Seek:
                return seek(_target.transform.position);

            case SteeringModes.Flee:
                return Flee(_target.transform.position);

            case SteeringModes.Arrive:
                return Arrive(_targetTrap.transform.position);

            case SteeringModes.Pursuit:
                return Pursuit(_target);

            case SteeringModes.Evade:
                return Evade(_target);

            case SteeringModes.Flocking:
                return Flocking();

            default:
                return Vector3.zero;
        }
    }

    private Vector3 Flocking()
    {
        return CalculateSeparetion(allAgents, _separetionRadius) * _separetionWeight
            + CalculateAlignment(allAgents, _alignmentRadius) * _alignmentWeight
            + CalculateCohesion(allAgents, _cohesionRadius) * _cohesionWeight;
    }

    private Vector3 CalculateSeparetion(List<Agent> list, float radius)
    {
        Vector3 dessired = default;
        int count = 0;

        foreach (var item in list)
        {
            if (item == this) continue;

            if (InRange(item.transform.position, radius))
            {
                dessired += (item.transform.position - transform.position);
                count++;
            }

        }

        if (count == 0) return Vector3.zero;

        dessired /= count;

        return CalculateSteering(-dessired.normalized * _maxSpeed);
    }
    private Vector3 CalculateAlignment(List<Agent> list, float radius)
    {
        Vector3 dessired = default;
        int count = 0;

        foreach (var item in list)
        {
            if (item == this) continue;

            if (InRange(item.transform.position, radius))
            {
                dessired += item.Velocity;
                count++;
            }

        }

        if (count == 0) return Vector3.zero;

        dessired /= count;

        return CalculateSteering(dessired.normalized * _maxSpeed);
    }

    private Vector3 CalculateCohesion(List<Agent> list, float radius)
    {
        Vector3 dessired = default;
        int count = 0;

        foreach (var item in list)
        {
            if (item == this) continue;

            if (InRange(item.transform.position, radius))
            {
                dessired += item.transform.position;
                count++;
            }

        }

        if (count == 0) return Vector3.zero;

        dessired /= count;

        return seek(dessired);
    }

    private bool InRange(Vector3 pos, float radius)
    {
        return (pos - transform.position).sqrMagnitude <= radius * radius;
    }

    private Vector3 CalculateSteering(Vector3 desired)
    {
        Vector3 steering = desired - _velocity;

        steering = Vector3.ClampMagnitude(steering, _maxStreering * Time.deltaTime);

        return steering;
    }

    private Vector3 DesiredVector(Vector3 traget)
    {
        Vector3 desired = (traget - transform.position).normalized;
        desired *= _maxSpeed;

        return desired;
    }

    private Vector3 seek(Vector3 target)
    {
        var desired = DesiredVector(target);

        return CalculateSteering(desired);


    }

    private Vector3 Flee(Vector3 target)
    {
        var desired = DesiredVector(target);

        return CalculateSteering(-desired);

    }

    private Vector3 Arrive(Vector3 target)
    {
        Vector3 direction = target - transform.position;

        float distance = direction.magnitude;

        if (distance < _minDistance)
        {
            return Vector3.zero;
        }

        float targetSpeed = _maxSpeed * (distance / _slowingDistance);
        float desiredSpeed = Mathf.Min(targetSpeed, _maxSpeed);

        Vector3 desired = direction.normalized * desiredSpeed;
        Vector3 steering = CalculateSteering(desired);

        return CalculateSteering(desired);
    }

    private Vector3 CalculateFuture(Agent target)
    {
        Vector3 direction = target.transform.position - transform.position;

        float distance = direction.magnitude;

        var prediction = distance / (_maxSpeed + _target.Velocity.magnitude);

        Vector3 futurePosition = target.transform.position + _target.Velocity * prediction;

        return futurePosition;
    }

    private Vector3 Pursuit(Agent target)
    {
        var futurePosition = CalculateFuture(target);

        return seek(futurePosition);
    }

    private Vector3 Evade(Agent target)
    {
        var futurePosition = CalculateFuture(target);

        return Flee(futurePosition);
    }
}
