using UnityEngine;

public class LightTurning : MonoBehaviour
{
    [SerializeField] float speed;
    void Update()
    {
        Vector3 rotation = new Vector3(-speed * Time.deltaTime, 0, 0);
        transform.Rotate(rotation, Space.Self);
    }
}
