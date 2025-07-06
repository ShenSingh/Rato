using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // === Public Variables (Inspector එකෙන් වෙනස් කළ හැක) ===
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float jumpForce = 7f;

    [Header("Manual Assignments")]
    public Animator anim; // Animator එක මෙතනට අතින් assign කරන්න

    // === Private Components & Variables ===
    private Rigidbody rb;
    private Joystick joystick;

    // --- State Variables ---
    private bool isGrounded;
    private bool canJump = true;

    // --- Start is called before the first frame update ---
    void Start()
    {
        // Physics component එක Parent ගෙන් ලබාගැනීම
        rb = GetComponentInParent<Rigidbody>();

        // Joystick එක ස්වයංක්‍රීයව සොයාගැනීම
        joystick = FindObjectOfType<Joystick>();
        
        // ගුරුත්වාකර්ෂණය මුලින් අක්‍රිය කිරීම
        if (rb != null)
        {
            rb.useGravity = false;
        }
    }

    // --- Update is called once per frame ---
    void Update()
    {
        // Script එක ක්‍රියාත්මක වීමට අවශ්‍ය දේ නැතිනම්, මෙතනින් නවතී
        if (rb == null || joystick == null) return;

        // ගුරුත්වාකර්ෂණය සක්‍රිය කිරීමේ ලොජික් එක
        ActivateGravityOnFirstTap();

        // Input සහ Jump වැනි ක්ෂණික දේ සඳහා Update() එක වඩා සුදුසුයි
        HandleJumping();
        HandleDebugLogging(); // Joystick එක test කිරීමට ලොග් කිරීම
    }

    // --- FixedUpdate is called on a fixed time interval (Physics සඳහා වඩා සුදුසුයි) ---
    void FixedUpdate()
    {
        // Script එක ක්‍රියාත්මක වීමට අවශ්‍ය දේ නැතිනම්, මෙතනින් නවතී
        if (rb == null || joystick == null || rb.useGravity == false) return;

        // චලනය වීමේ ලොජික් එක
        HandleMovement();
    }
    
    // --- Custom Methods ---

    void ActivateGravityOnFirstTap()
    {
        if (rb.useGravity == false && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                 rb.useGravity = true;
            }
        }
    }

    void HandleMovement()
    {
        // ජොයිස්ටික් එකෙන් input එක ලබාගැනීම
        float horizontalInput = joystick.Horizontal;
        
        // චරිතය චලනය කිරීම
        rb.linearVelocity = new Vector3(horizontalInput * moveSpeed, rb.linearVelocity.y, 0);

        // Animation පාලනය කිරීම
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(horizontalInput));
        }

        // චරිතය දිශාව මාරු කිරීම
        if (horizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void HandleJumping()
    {
        // බිම සිටීදැයි පරීක්ෂා කිරීම
        CheckIfGrounded();
        
        // ජොයිස්ටික් එක උඩට කර ඇත්නම් සහ පැනීමට හැකි නම්
        if (isGrounded && joystick.Vertical > 0.75f && canJump)
        {
            canJump = false; // පැනීමෙන් පසු, නැවත පැනීම තාවකාලිකව නැවැත්වීම
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (anim != null)
            {
                anim.SetTrigger("Jump");
            }
        }

        // ජොයිස්ටික් එක නැවත පහළට ගෙනා විට, නැවත පැනීමට හැකි කිරීම
        if (joystick.Vertical < 0.5f)
        {
            canJump = true;
        }
    }

    void CheckIfGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
    
    // --- ඔබගේ ඉල්ලීම පරිදි, Joystick එක Test කිරීමට ---
    void HandleDebugLogging()
    {
        if (joystick.Horizontal > 0.5f)
        {
            Debug.Log("Joystick: Right");
        }
        else if (joystick.Horizontal < -0.5f)
        {
            Debug.Log("Joystick: Left");
        }

        if (joystick.Vertical > 0.5f)
        {
            Debug.Log("Joystick: Up");
        }
        else if (joystick.Vertical < -0.5f)
        {
            Debug.Log("Joystick: Down");
        }
    }
}