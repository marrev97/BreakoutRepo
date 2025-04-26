using UnityEngine;

[System.Serializable]

public class PaddleController : MonoBehaviour
{
    public class KeyBindings
    {
        public KeyCode moveLeft = KeyCode.LeftArrow;
        public KeyCode moveRight = KeyCode.RightArrow;
    }

    public float speed = 10f;
    private float horizontalInput;
    private Rigidbody2D rb;

    public KeyBindings keyBindings;

    //Done instead of relying on rigid body boundaries
    [Header("Wall References")]
    [SerializeField] private Transform leftWall;
    [SerializeField] private Transform rightWall;

    private float leftBound;
    private float rightBound;
    private float paddleHalfWidth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        keyBindings = new KeyBindings();

        //Fix bug where the paddle is being pushed down by the ball every time it hits the paddle
        GetComponent<Rigidbody2D>().isKinematic = true;

        // Calculate bounds once at startup
        paddleHalfWidth = GetComponent<BoxCollider2D>().size.x * transform.localScale.x / 2f;

        UpdateBounds();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        float moveInput = 0f;

        if (Input.GetKey(keyBindings.moveLeft))
        {
            moveInput = -1f;
        }
        else if (Input.GetKey(keyBindings.moveRight))
        {
            moveInput = 1f;
        }

        Vector2 newPosition = rb.position + Vector2.right * horizontalInput * speed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);

        newPosition.x = Mathf.Clamp(newPosition.x, leftBound, rightBound);

        rb.MovePosition(newPosition);
    }

    void UpdateBounds()
    {
        if (leftWall && rightWall)
        {
            // Account for paddle width when calculating bounds
            leftBound = leftWall.position.x + leftWall.GetComponent<Collider2D>().bounds.extents.x + paddleHalfWidth;
            rightBound = rightWall.position.x - rightWall.GetComponent<Collider2D>().bounds.extents.x - paddleHalfWidth;
        }
    }
}