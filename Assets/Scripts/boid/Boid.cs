using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private bool isDead = false;
    private TrapFinder trapFinder;
    private Trap currentTargetTrap;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("Targets")]
    [SerializeField] private Agent _targetHunter;
    [SerializeField] private float _HunterDetectionRange = 30f;
    [SerializeField] private float _TrapDetectionRange = 5f;

    [Header("Movement Settings")]
    [SerializeField] private float _MaxSpeed = 5f;
    [SerializeField] private float _MaxSteering = 5f;
    [SerializeField] private float _SlowingDistance = 1f;
    [SerializeField] private float _MinDistance = 0.1f;

    [Header("Flocking settings")]
    private static List<Boid> allAgents = new List<Boid>();
    [SerializeField] private float floatmaximumDetectionRange = 4f;
    [SerializeField] private float minimumSeparationDistance = 1.5f;
    [SerializeField] private float alignment = 2f;

    [Header("Flocking weights")]
    [SerializeField, Range(0f, 5f)] private float SeparationWeight = 1f;
    [SerializeField, Range(0f, 5f)] private float AlignmentWeight = 1f;
    [SerializeField, Range(0f, 5f)] private float CohesionWeight = 1f;
    [SerializeField, Range(0f, 10f)] private float EvadeWeight = 3f;

  

    [SerializeField] private Vector3 _velocity;
    public Vector3 Velocity => _velocity;
    public enum SteeringModes { Seek, Flee, Arrive, Pursuit, Evade, Flocking }
    public SteeringModes currentSteering = SteeringModes.Flocking;

    private void Awake()
    {
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        _velocity = randomDirection.normalized * _MaxSpeed;

        allAgents.Add(this);
        trapFinder = GetComponent<TrapFinder>();

    }

    private void Update()
    {
        if (isDead)  return; 


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
        ReleaseTrap();
        allAgents.Remove(this);
    }

    private void ReleaseTrap()
    {
        if (currentTargetTrap != null)
        {
            currentTargetTrap.Release(this);
            currentTargetTrap = null;
        }
    }

    private Vector3 GetSteeringForce()
    {
        switch (currentSteering)
        {
            case SteeringModes.Flee:
                ReleaseTrap();
                if (_targetHunter == null) return Flocking();
                return CalculateSteering(Flee(_targetHunter.transform.position));

            case SteeringModes.Arrive:
                if (currentTargetTrap == null)
                {
                    currentTargetTrap = trapFinder.FindNearestTrap(this);
                    if (currentTargetTrap != null) currentTargetTrap.Claim(this);
                }

                if (currentTargetTrap == null)
                {
                    currentSteering = SteeringModes.Flocking;
                    return Flocking();
                }

                return CalculateSteering(Arrive(currentTargetTrap.transform.position, currentTargetTrap));

            case SteeringModes.Pursuit:
                ReleaseTrap();
                if (_targetHunter == null) return Vector3.zero;
                return CalculateSteering(Pursuit(_targetHunter));

            case SteeringModes.Evade:
                ReleaseTrap();
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
        Vector3 calculateSeparation = CalculateSeparation();
        Vector3 calculateAlignment = CalculateAlignment();
        Vector3 calculateCohesion = CalculateCohesion();

        Vector3 desiredVelocity = calculateSeparation * SeparationWeight + calculateAlignment * AlignmentWeight + calculateCohesion * CohesionWeight;

        if (_targetHunter != null && Vector3.Distance(transform.position, _targetHunter.transform.position) <= _HunterDetectionRange)
        {
            desiredVelocity += Evade(_targetHunter) * EvadeWeight;
        }

        Trap nearestTrap = trapFinder.FindNearestTrap(this);
        if (nearestTrap != null && Vector3.Distance(transform.position, nearestTrap.transform.position) <= _TrapDetectionRange)
        {
            if (nearestTrap.Claim(this))
            {
                currentTargetTrap = nearestTrap;
                currentSteering = SteeringModes.Arrive;
            }
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

    private Vector3 Seek(Vector3 target) => (target - transform.position).normalized * _MaxSpeed;
    private Vector3 Flee(Vector3 target) => (transform.position - target).normalized * _MaxSpeed;

    private Vector3 Arrive(Vector3 target, Trap trap)
    {
        Vector3 direction = target - transform.position;
        float distance = direction.magnitude;
        float velocity = _MaxSpeed;

        if (distance <= _SlowingDistance)
        {
            if (distance <= _MinDistance)
            {
                trap.DestroyTrap();
                ReleaseTrap();
                return Vector3.zero;
            }
        }

        return direction.normalized * velocity;
    }

    private Vector3 Pursuit(Agent target) => Seek(CalculateFuture(target));
    private Vector3 Evade(Agent target) => Flee(CalculateFuture(target));

    private Vector3 CalculateSeparation()
    {
        Vector3 desired = Vector3.zero;
        int count = 0;

        foreach (var item in allAgents)
        {
            if (item == this || item.isDead) continue;
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
            if (item == this || item.isDead) continue;
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
            if (item == this || item.isDead) continue;
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

        ReleaseTrap();
        isDead = true;
        _velocity = Vector3.zero;
        gameObject.tag = "Dead";
        
    }

    public void kill()
    {
        Die();
        StartCoroutine(Respawn());
    }



    private IEnumerator Respawn()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Renderer renderer in renderers) renderer.enabled = false;
        foreach (Collider collider in colliders) collider.enabled = false;

        yield return new WaitForSeconds(10f);

        if (spawnPoints != null && spawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Count);
            transform.position = spawnPoints[randomIndex].position;
        }

        foreach (Renderer renderer in renderers) renderer.enabled = true;
        foreach (Collider collider in colliders) collider.enabled = true;

        isDead = false;
        gameObject.tag = "Boid";
        _velocity = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * _MaxSpeed;
    }
}