using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float Velocity = 10f;

    private void Update()
    {
        transform.position += transform.forward * Velocity * Time.deltaTime;
        Destroy(gameObject, 5f);
    }
}