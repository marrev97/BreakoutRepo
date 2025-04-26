using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class Ball : MonoBehaviour
{
    public enum VisualState { Normal, Waiting, Launching, Bonus }

    [Header("Physics Settings")]
    public float initialSpeed = 5f;
    public float maxSpeed = 50f;
    public float minSpeed = 3f;
    public float currentSpeed;
    public float deathBoundaryY = -5f;

    [Header("Visual Feedback")]
    public Color normalColor = Color.white;
    public Color waitingColor = new Color(1, 1, 1, 0.5f);
    public Color launchingColor = Color.yellow;
    public Color cloneColor = Color.blue;
    public GameObject readyIndicator;

    [Header("Collision Settings")]
    public float activationDelay = 0.1f;

    // Component references
    [System.NonSerialized] public Rigidbody2D rb;
    private Collider2D ballCollider;
    private SpriteRenderer spriteRenderer;

    // State variables
    private Vector2 lastVelocity;
    private VisualState currentState;

    private BallEngine ballEngine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ballCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Critical physics setup
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        currentSpeed = initialSpeed;
    }

    public void Initialize(BallEngine be)
    {
        ballEngine = be;
    }

    public void SetVisualState(VisualState state)
    {
        currentState = state;

        switch (state)
        {
            case VisualState.Normal:
                spriteRenderer.color = normalColor;
                if (readyIndicator != null) readyIndicator.SetActive(false);
                break;

            case VisualState.Waiting:
                spriteRenderer.color = waitingColor;
                if (readyIndicator != null) readyIndicator.SetActive(true);
                break;

            case VisualState.Launching:
                spriteRenderer.color = launchingColor;
                if (readyIndicator != null) readyIndicator.SetActive(false);
                break;

            case VisualState.Bonus:
                spriteRenderer.color = cloneColor;
                if (readyIndicator != null) readyIndicator.SetActive(false);
                break;
        }
    }

    public void Activate()
    {
        SetVisualState(VisualState.Launching);
        ballCollider.enabled = true;
        rb.isKinematic = false;
        rb.simulated = true;
    }

    public void Launch(bool immediate = false)
    {
        if (currentState == VisualState.Waiting || immediate)
        {
            Activate();
            StartCoroutine(LaunchSequence(immediate));
        }
    }

    private IEnumerator LaunchSequence(bool immediate)
    {
        if (!immediate)
        {
            yield return new WaitForSeconds(activationDelay);
        }

        // Ensure physics is fully active
        if (rb.isKinematic)
        {
            rb.isKinematic = false;
            yield return null; // Wait one frame
        }

        // Apply velocity with random direction;  -1 for launching it downward

        if (immediate)

        {
            //spawned bonus balls speed are slow and launched upwards
            Vector2 dir = new Vector2(Random.Range(-0.5f, 0.5f), 1).normalized;
            rb.velocity = (dir * currentSpeed) / 4;
            SetVisualState(VisualState.Bonus);
        }

        else
        {
            Vector2 dir = new Vector2(Random.Range(-0.5f, 0.5f), -1).normalized;
            rb.velocity = dir * currentSpeed;
            SetVisualState(VisualState.Normal);
        }
    }

    void Update()
    {
        if (currentState == VisualState.Normal)
        {
            lastVelocity = rb.velocity;
            ClampVelocity();

            if (transform.position.y < deathBoundaryY)
            {
                HandleBallLoss();
            }
        }
    }

    private void HandleBallLoss()
    {
        ballEngine.BallLost(this);
        Destroy(gameObject);
    }

    public void Deactivate()
    {
        SetVisualState(VisualState.Waiting);

        transform.position = ballEngine.ballSpawnPoint.position;
        
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
    }

    void ClampVelocity()
    {
        float speed = rb.velocity.magnitude;
        if (speed < minSpeed || speed > maxSpeed)
        {
            Vector2 direction = rb.velocity.normalized;
            rb.velocity = direction * Mathf.Clamp(speed, minSpeed, maxSpeed);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState != VisualState.Normal) return;

        Vector2 normal = collision.contacts[0].normal;
        Vector2 direction = Vector2.Reflect(lastVelocity.normalized, normal);
        direction = Quaternion.Euler(0, 0, Random.Range(-5f, 5f)) * direction;

        rb.velocity = direction * lastVelocity.magnitude;
    }
}