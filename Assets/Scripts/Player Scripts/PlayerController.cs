using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public PlayerData Data;

    public Rigidbody2D playerRigidBody { get; private set; }

    #region Bool Sets
    public bool isFacingRight { get; private set; }
    public bool isJumping { get; private set; }
    public bool isWallJumping { get; private set; }
    public bool isSliding { get; private set; }
    #endregion

    #region Timer Sets
    public float lastOnGroundTime { get; private set; }
    public float lastOnWallTime { get; private set; }
    public float lastOnWallRightTime { get; private set; }
    public float lastOnWallLeftTime { get; private set; }
    #endregion

    private bool isJumpCut;
    private bool isJumpFalling;

    private float wallJumpStartTime;
    private int lastWallJumpDir;

    private Vector2 moveInput;
    public float lastPressedJumpTime { get; private set; }

    #region Check Handlers
    [Space(10)]
    [Header("Checks")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(1, 0.17f);

    [Space(5)]
    [SerializeField] private Transform frontWallCheck;
    [SerializeField] private Transform backWallCheck;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.5f, 1f);
    #endregion

    #region Layer Set
    [Space(10)]
    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    #endregion

    #region Menus
    [Space(10)]
    [Header("Game Menus")]
    //[SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject deathMenu;
    #endregion

    private void Awake()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetGravityScale(Data.gravityScale);
        isFacingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        #region Timers
        lastOnGroundTime -= Time.deltaTime;
        lastOnWallTime -= Time.deltaTime;
        lastOnWallRightTime -= Time.deltaTime;
        lastOnWallLeftTime -= Time.deltaTime;

        lastPressedJumpTime -= Time.deltaTime;
        #endregion


        if (moveInput.x != 0)
        {
            CheckDirectionFacing(moveInput.x > 0);
        }

        HandleInput();

        PlayerCollisionChecks();

        HandleJumpInput();

        SettingGravity();

        SlideCheck();
    }

    private void FixedUpdate()
    {
        if(isWallJumping)
        {
            Run(Data.wallJumpRunLerp);
        }
        else
        {
            Run(1);
        }

        if(isSliding)
        {
            WallSlide();
        }
    }

    public void HandleInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
    }

    #region Collision Check
    public void PlayerCollisionChecks()
    {
        if (!isJumping)
        {
            if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer) && !isJumping)
            {
                lastOnGroundTime = Data.coyoteTime; //This sets the last time we are grounded to the Coyote time
            }

            if (((Physics2D.OverlapBox(frontWallCheck.position, wallCheckSize, 0, groundLayer) && isFacingRight)
                    || (Physics2D.OverlapBox(backWallCheck.position, wallCheckSize, 0, groundLayer) && !isFacingRight)) && !isWallJumping)
            {
                lastOnWallRightTime = Data.coyoteTime;
            }

            if (((Physics2D.OverlapBox(frontWallCheck.position, wallCheckSize, 0, groundLayer) && !isFacingRight)
                || (Physics2D.OverlapBox(backWallCheck.position, wallCheckSize, 0, groundLayer) && isFacingRight)) && !isWallJumping)
            {
                lastOnWallLeftTime = Data.coyoteTime;
            }

            lastOnWallTime = Mathf.Max(lastOnWallLeftTime, lastOnWallRightTime);
        }
    }

    //Using to actually see the collider checks, some reason they weren't showing up in editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);

        Gizmos.DrawWireCube(frontWallCheck.position, wallCheckSize);
        Gizmos.DrawWireCube(backWallCheck.position, wallCheckSize);
    }
    #endregion

    #region Gravity
    public void SetGravityScale(float scale)
    {
        playerRigidBody.gravityScale = scale;
    }

    //Method sets the gravity higher if player releases the jump input or falling
    public void SettingGravity()
    {
        if(isSliding)
        {
            SetGravityScale(0);
        }
        else if (playerRigidBody.velocity.y < 0 && moveInput.y < 0)
        {
            //Higher gravity if holding down
            SetGravityScale(Data.gravityScale * Data.fastFallGravityMulti);
            //Caps the fall speed so it doesn't go too high
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, Mathf.Max(playerRigidBody.velocity.y, -Data.maxFastFallSpeed));
        }
        else if (isJumpCut)
        {
            //Higher gravity if jump button is released
            SetGravityScale(Data.gravityScale * Data.jumpCutGravityMulti);
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, Mathf.Max(playerRigidBody.velocity.y, -Data.maxFallSpeed));
        }
        else if ((isJumping || isWallJumping ||isJumpFalling) && Mathf.Abs(playerRigidBody.velocity.y) < Data.jumpHangTimeThreshold)
        {
            SetGravityScale(Data.gravityScale * Data.jumpHangMulti);
        }
        else if (playerRigidBody.velocity.y < 0)
        {
            SetGravityScale(Data.gravityScale * Data.fallGravityMulti);
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, Mathf.Max(playerRigidBody.velocity.y, -Data.maxFallSpeed));
        }
        else
        {
            SetGravityScale(Data.gravityScale);
        }
    }
    #endregion

    #region Run Method
    private void Run(float lerpAmount)
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = moveInput.x * Data.runMaxSpeed;
        
        //Use Lerp() to help smooth changes in our direction and speed
        targetSpeed = Mathf.Lerp(playerRigidBody.velocity.x, targetSpeed, lerpAmount);

        float accelRate;

        //Get accleration value based on if player is accelerating or trying to decelerate
        if(lastOnGroundTime > 0)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDecelAmount;
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDecelAmount * Data.decelInAir;
        }

        //Increase the players accelertaion and max speed when at the apex of their jump (makes it feel bouncy)
        if((isJumping || isWallJumping || isJumpFalling) && Mathf.Abs(playerRigidBody.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelMulti;
            targetSpeed *= Data.jumpHangMaxSpeedMulti;
        }

        if(Data.doConserveMomentum && Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Abs(targetSpeed) && 
            Mathf.Sign(playerRigidBody.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && lastOnGroundTime < 0)
        {
            //What causes the momentum to stay with the player
            accelRate = 0;
        }

        //Calculates the difference between current velocity and desired velocity
        float speedDif = targetSpeed - playerRigidBody.velocity.x;

        //Calculate the force along x-axis to apply to the player
        float movement = speedDif * accelRate;

        //Need to put it into an if statement to not run into AddForce{NaN, NaN} Error
        if(moveInput.x != 0)
        {
            //Applys the force to the RigidBody
            playerRigidBody.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }
    }
    #endregion

    #region Jump Code
    public void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Jump"))
        {
            OnJumpUpInput();
        }

        if (isJumping && playerRigidBody.velocity.y < 0)
        {
            isJumping = false;

            if (!isWallJumping)
            {
                isJumpFalling = true;
            }
        }

        if (isWallJumping && Time.time - wallJumpStartTime > Data.wallJumpTime)
        {
            isWallJumping = false;
        }

        if (lastOnGroundTime > 0 && !isJumping && !isWallJumping)
        {
            isJumpCut = false;

            if (!isJumping)
            {
                isJumpFalling = false;
            }
        }

        //Where jump is called
        if (CanJump() && lastPressedJumpTime > 0)
        {
            isJumping = true;
            isWallJumping = false;
            isJumpCut = false;
            isJumpFalling = false;
            Jump();
        }

        //Where walljump is called
        else if (CanWallJump() && lastPressedJumpTime > 0)
        {
            isWallJumping = true;
            isJumping = false;
            isJumpCut = false;
            isJumpFalling = false;
            wallJumpStartTime = Time.time;
            lastWallJumpDir = (lastOnWallRightTime > 0) ? -1 : 1;

            WallJump(lastWallJumpDir);
        }
    }

    public void OnJumpInput()
    {
        lastPressedJumpTime = Data.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
        {
            isJumpCut = true;
        }
    }
    private void Jump()
    {
        //Makes sure player can't just multiple times from one button press
        lastPressedJumpTime = 0;
        lastOnGroundTime = 0;

        //Increases the force applied if the player is falling
        float force = Data.jumpForce;

        if(playerRigidBody.velocity.y < 0)
        {
            force -= playerRigidBody.velocity.y;
        }

        playerRigidBody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private void WallJump(int direction)
    {
        lastPressedJumpTime = 0;
        lastOnGroundTime = 0;
        lastOnWallRightTime = 0;
        lastOnWallLeftTime = 0;

        Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= direction; //Apply force in opposite direction of the wall player is on

        if(Mathf.Sign(playerRigidBody.velocity.x) != Mathf.Sign(force.x))
        {
            force.x -= playerRigidBody.velocity.x;
        }

        //Checks if the player is falling, if they are we counteract the force of gravity. This ensures that they will push how they want
        if(playerRigidBody.velocity.y < 0)
        {
            force.y -= playerRigidBody.velocity.y;
        }

        playerRigidBody.AddForce(force, ForceMode2D.Impulse);
    }
    #endregion

    #region Wallslide Methoids
    public void SlideCheck()
    {
        if(CanWallSlide() && ((lastOnWallLeftTime > 0 && moveInput.x < 0) || (lastOnWallRightTime > 0 && moveInput.x > 0)))
        {
            isSliding = true;
        }
        else
        {
            isSliding = false;
        }
    }

    private void WallSlide()
    {
        float speedDif = Data.slideSpeed - playerRigidBody.velocity.y;
        float movement = speedDif * Data.slideAccel;

        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), 
            Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        playerRigidBody.AddForce(movement * Vector2.up);
    }
    #endregion

    #region Character Facing Methods
    private void Turn()
    {
        Vector3 scale = transform.localScale;

        scale.x *= -1;
        transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }

    public void CheckDirectionFacing(bool isMovingRight)
    {
        if (isMovingRight != isFacingRight)
        {
            Turn();
        }
    }
    #endregion

    #region Bool Methods
    private bool CanJump()
    {
        return lastOnGroundTime > 0 && !isJumping;
    }

    private bool CanJumpCut()
    {
        return isJumping && playerRigidBody.velocity.y > 0;
    }

    private bool CanWallJump()
    {
        return lastPressedJumpTime > 0 && lastOnWallTime > 0 && lastOnGroundTime <= 0 && (!isWallJumping ||
            (lastOnWallRightTime > 0 && lastWallJumpDir == 1) || (lastOnWallLeftTime > 0 && lastWallJumpDir == -1));
    }

    private bool CanWallJumpCut()
    {
        return isWallJumping && playerRigidBody.velocity.y > 0;
    }

    private bool CanWallSlide()
    {
        if(lastOnWallTime > 0 && !isJumping && !isWallJumping && lastOnGroundTime <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D playerCollision)
    {
        if (playerCollision.gameObject.tag == "Grounded Enemy" || playerCollision.gameObject.tag == "Air Enemy" 
            || playerCollision.gameObject.tag == "Spike" || playerCollision.gameObject.tag == "Pitfall")
        {
            gameObject.SetActive(false);
            deathMenu.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D playerCollision)
    {
        if (playerCollision.gameObject.tag == "Finish Line")
        {
            gameObject.SetActive(false);
            winMenu.SetActive(true);
        }
    }
}
