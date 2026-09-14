using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float Velocity = 10f;

    private void Update()
    {
        transform.position += transform.forward * Velocity * Time.deltaTime;
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Boid"))
        {
            Boid boid = col.GetComponent<Boid>();
            if (boid != null)
            {
                Debug.Log("Le di al Boid");
                boid.Die();
            }
            Destroy(gameObject);
        }
    }
}