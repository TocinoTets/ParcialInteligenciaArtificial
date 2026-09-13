using Unity.VisualScripting;
using UnityEngine;

public class StrokeState : State
{
    private PatrolData _data;
    [SerializeField] private float _maxDistansShoot = 5f;
    [SerializeField] private bool _useShoot = true;
    [SerializeField] private float _reutilizeShoot = 5f;





    public StrokeState(PatrolData data, FMS strateMachine) : base(strateMachine)
    //le tengo q pasar esto xq State no es MonoBehaviour para pasar el .transform.position del ajente
    {

        _data = data;
    }
    public override void Enter()
    {
        Debug.Log("entre stoke");
    }

    public override void Update()
    {
        _data.timer2 += Time.deltaTime;

        if (!_useShoot && _data.timer2 > _reutilizeShoot) 
        {
            _useShoot = true;
        }
            
        Collider[] vecinos = Physics.OverlapSphere(_data.transform.position, _maxDistansShoot);

        foreach (var col in vecinos)
        {
            if (col.CompareTag("Boid") && _useShoot)
            {
                Boid boid = col.GetComponent<Boid>();

                float distancia = (boid.transform.position - _data.transform.position).magnitude;
                float tiempo = distancia / 10f;
                Vector3 futuraPosicion = boid.transform.position + boid.Velocity * tiempo;

                Vector3 direccion = futuraPosicion - _data.transform.position;
                direccion.y = 0;

                Quaternion rotacion = Quaternion.LookRotation(direccion);

                GameObject.Instantiate(_data.bullet, _data.transform.position, rotacion);

                Debug.Log("disparo");

                _useShoot = false;
                _data.timer2 = 0;
                StrateMachine.ChangeState(Estados.PatrolState);
            }
        }
    }
    

    public override void Exit()
    {
        Debug.Log("sali stoke");
    }

}
