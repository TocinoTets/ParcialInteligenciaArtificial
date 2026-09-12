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
        Debug.Log("entre IdleState");
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
                //falta crear el disparo y q el personaje vaya a el objeto muerto 
                Debug.Log("disparo");
                _useShoot = false;
                _data.timer2 = 0;
                StrateMachine.ChangeState(Estados.PatrolState);
            }
        }
    }

    /*esto lo puedo sacar, es para ver si anda bien el circulo */
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_data.transform.position, _maxDistansShoot);
    }


    public override void Exit()
    {
        Debug.Log("sali IdleState");
    }

}
