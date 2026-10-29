using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] private float velocidad;
    [SerializeField] private float danio;

    private void Update()
    {
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}