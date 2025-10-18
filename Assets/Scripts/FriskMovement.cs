using UnityEngine;

public class FriskMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    [Header("Sprites")]
    public Sprite idleDown;
    public Sprite idleUp;
    public Sprite idleLeft;
    public Sprite idleRight;
    public Sprite[] walkDown;
    public Sprite[] walkUp;
    public Sprite[] walkLeft;
    public Sprite[] walkRight;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector2 input;
    private string lastDirection = "Down";
    private float animationTimer;
    private int animationFrame;
    public float frameDuration = 0.2f; // how fast the walk animation swaps frames

    // ?? Added: static toggle to enable/disable player control
    public static bool CanMove = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0;     // No gravity in a top-down game
        rb.freezeRotation = true; // Prevent flipping on collision

        spriteRenderer.sprite = idleDown; // Start facing down
    }

    void Update()
    {
        // ?? If movement is disabled, stop all input and animation
        if (!CanMove)
        {
            rb.linearVelocity = Vector2.zero;
            switch (lastDirection)
            {
                case "Up": spriteRenderer.sprite = idleUp; break;
                case "Down": spriteRenderer.sprite = idleDown; break;
                case "Left": spriteRenderer.sprite = idleLeft; break;
                case "Right": spriteRenderer.sprite = idleRight; break;
            }
            return;
        }

        HandleInput();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (CanMove)
            MoveCharacter();
        else
            rb.linearVelocity = Vector2.zero;
    }

    void HandleInput()
    {
        float x = 0;
        float y = 0;

        if (Input.GetKey(KeyCode.UpArrow)) y += 1;
        if (Input.GetKey(KeyCode.DownArrow)) y -= 1;
        if (Input.GetKey(KeyCode.LeftArrow)) x -= 1;
        if (Input.GetKey(KeyCode.RightArrow)) x += 1;

        input = new Vector2(x, y).normalized;

        // Update last direction — prioritize horizontal when moving diagonally
        if (input != Vector2.zero)
        {
            if (x != 0)
            {
                // Prioritize horizontal movement for diagonal directions
                lastDirection = (x > 0) ? "Right" : "Left";
            }
            else if (y != 0)
            {
                lastDirection = (y > 0) ? "Up" : "Down";
            }
        }
    }

    void MoveCharacter()
    {
        // Use Rigidbody2D for proper collisions
        rb.linearVelocity = input * moveSpeed;
    }

    void UpdateAnimation()
    {
        if (input == Vector2.zero)
        {
            rb.linearVelocity = Vector2.zero;

            switch (lastDirection)
            {
                case "Up": spriteRenderer.sprite = idleUp; break;
                case "Down": spriteRenderer.sprite = idleDown; break;
                case "Left": spriteRenderer.sprite = idleLeft; break;
                case "Right": spriteRenderer.sprite = idleRight; break;
            }
        }
        else
        {
            animationTimer += Time.deltaTime;
            if (animationTimer >= frameDuration)
            {
                animationTimer = 0;
                animationFrame = (animationFrame + 1) % 2;
            }

            // Play correct walking animation based on direction
            switch (lastDirection)
            {
                case "Up":
                    spriteRenderer.sprite = walkUp[animationFrame];
                    break;
                case "Down":
                    spriteRenderer.sprite = walkDown[animationFrame];
                    break;
                case "Left":
                    spriteRenderer.sprite = walkLeft[animationFrame];
                    break;
                case "Right":
                    spriteRenderer.sprite = walkRight[animationFrame];
                    break;
            }
        }
    }
}
