using UnityEngine;

public class Road : MonoBehaviour
{
    private float _speed = 7f;
    private float _destroyZ = -30f;

    void Update()
    {
        transform.Translate(Vector3.back * _speed * Time.deltaTime);
        if (transform.position.z < _destroyZ)
        {
            Destroy(gameObject);
        }
    }
}
