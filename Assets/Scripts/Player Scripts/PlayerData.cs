using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    //Gives player a gravity force and the amount of power needed to bring it down
    [Header("Gravity")]
    [HideInInspector] public float gravityStrength;
    [HideInInspector] public float gravityScale;

    [Space(5)]
    public float fallGravityMulti; //Multiplyer to the players gravity scale when falling
    public float maxFallSpeed; //The maximum speed of how fast the player can fall

    [Space(5)]
    public float fastFallGravityMulti; //Multiplyer for how fast the player can make them themselves fall
    public float maxFastFallSpeed; //Maximum speed they can reach with that multiplyer

    [Space(20)]
    [Header("Run")]
    public float runMaxSpeed;
    public float runAccelration; //The speed that gets the player to max speed
    [HideInInspector] public float runAccelAmount; //The force that is applied to the player

    public float runDecelration; //The speed that makes the player decelrate from their current speed
    [HideInInspector] public float runDecelAmount; //The force applied to the player

    //Accelrator and deccelrator for player when in the air
    [Space(5)]
    [Range(0f, 1)] public float accelInAir;
    [Range(0f, 1)] public float decelInAir;

    [Space(5)]
    public bool doConserveMomentum = true;

    [Space(20)]
    [Header("Jump")]
    public float jumpHeight;
    public float jumpTimeToApex; //The time between the applying the jump force and reaching the the full jump height
    [HideInInspector] public float jumpForce; //The force that is applied to the player when jumping

    [Header("Jump Extra")]
    public float jumpCutGravityMulti; //Multipler to increase gravity if the player lets go of the jump button
    [Range(0f, 1)] public float jumpHangMulti; //Reduces gravity when player is close to their apex
    public float jumpHangTimeThreshold; //The time the player will experice the hang in the air

    [Space(0.5f)]
    public float jumpHangAccelMulti; //How fast the player moves in the hang
    public float jumpHangMaxSpeedMulti; //Maximum speed they can reach in the hang

    [Space(10)]
    [Header("Wall Jump")]
    public Vector2 wallJumpForce; //The force that will get player off the wall
    [Space(5)]
    [Range(0f, 1f)] public float wallJumpRunLerp; //Reduces the players movement while wall jumping
    [Range(0f, 1f)] public float wallJumpTime; //How long the player is slowed after wall jumping
    public bool canWallJump; //Make the player turn and face the way they are wall jumping

    [Space(20)]
    [Header("Wall Slide")]
    public float slideSpeed;
    public float slideAccel;

    [Space(20)]
    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime; //This is for after the player leaves a platform they still have time to input jump
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime; //The period of time where the player can press the jump input and it will be performed before needing to touch the ground


    private void OnValidate()
    {
        //Calculate the gravities strength
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

        //Calculate the rigidbody's gravity scale
        gravityScale = gravityStrength / Physics2D.gravity.y;

        //Calculate the run accelration and decelration forces
        runAccelAmount = (50 * runAccelration) / runMaxSpeed;
        runDecelAmount = (50 * runDecelAmount) / runMaxSpeed;

        //Calculate the jump force
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

        runAccelration = Mathf.Clamp(runAccelration, 0.01f, runMaxSpeed);
        runDecelration = Mathf.Clamp(runDecelration, 0.01f, runMaxSpeed);
    }
}
