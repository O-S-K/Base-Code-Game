using UnityEngine;

public class ObjectMove : MonoBehaviour
{
    public enum Direction
    {
        Horizontal,
        Verticle
    }

    public Direction direction = Direction.Horizontal;
    public float distanceMove = 5f; 
    public float speed = 3f;
    public float offset = 0f; 

    private bool isForward = true; 
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;

        switch (direction)
        {
            case Direction.Horizontal:
                transform.position += Vector3.right * offset;
                break;
            case Direction.Verticle:
                transform.position += Vector3.forward * offset;
                break;
        }
    }

    void Update()
    {
        switch (direction)
        {
            case Direction.Horizontal:
                MoveLoop(transform.position.x, startPos.x, Vector3.right);
                break;
            case Direction.Verticle:
                MoveLoop(transform.position.z, startPos.z, Vector3.forward);
                break;
        }
    }

    public void MoveLoop(float _position, float _startPos, Vector3 _dir)
    {
        if (isForward)
        {
            if (_position < _startPos + distanceMove)
            {
                transform.position += DirMove(_dir);
            }
            else isForward = false;
        }
        else
        {
            if (_position > _startPos)
            {
                transform.position -= DirMove(_dir);
            }
            else isForward = true;
        }
    }

    public Vector3 DirMove(Vector3 _dir)
    {
        return _dir * Time.deltaTime * speed;
    }
}
