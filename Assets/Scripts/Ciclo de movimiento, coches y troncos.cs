using UnityEngine;

public class MoveCycle : MonoBehaviour
{
    public Vector2 direction = Vector2.right;
    public float speed = 2f;
    public int size = 2;

    private float leftEdgeX;
    private float rightEdgeX;

    private void Start()
    {
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
        leftEdgeX = leftEdge.x - size;
        rightEdgeX = rightEdge.x + size;
    }

    private void Update()
    {
        float positionX = transform.position.x;

        if ((direction.x > 0 && positionX > rightEdgeX) || (direction.x < 0 && positionX < leftEdgeX))
        {
            positionX = direction.x > 0 ? leftEdgeX : rightEdgeX;
        }

        transform.position = new Vector3(positionX, transform.position.y, transform.position.z);
        transform.Translate(speed * Time.deltaTime * direction);
    }
}
