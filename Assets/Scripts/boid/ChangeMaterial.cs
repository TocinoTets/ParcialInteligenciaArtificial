using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{

    private Boid _boid;

    [Header("Visual settings")]
    [SerializeField] private Material stunMaterial;

    [SerializeField] private Material deadMaterial;

    [SerializeField] private Material originalMaterial;



    private Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _boid = GetComponent<Boid>();

    }

    private void Update()
    {
        if (gameObject.tag == "Dead")
        {
            _renderer.material = deadMaterial;
        }else if(_boid.currentSteering == Boid.SteeringModes.Arrive)
        {
            _renderer.material = stunMaterial;
        }else
        {
            _renderer.material = originalMaterial;
        }
    }
}