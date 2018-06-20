using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CharacterAttributes", menuName = "CharacterAttributes", order = 0)]
[Serializable]
public class CharacterAttributes : ScriptableObject
{
    [Header("Attributes")]
    public IdleStateProperties idleState;
    public WalkStateProperties walkState;
    public JumpStateProperties jumpState;
    public FallStateProperties fallState;
    public RunStateProperties runState;
    public RunSkidStateProperties runSkidState;
    public GrindStateProperties grindStateProperties;
}
[Serializable]
public struct IdleStateProperties
{
    [Range(0, 1)]
    public float idleThreshold;
    [Range(0, 1)]
    public float walkThreshold;
    [Range(0, 1)]
    public float runThreshold;
    [Range(.90f, 1)]
    public float deaccel;
    [Range(0, 1)]
    public float rotsPerFrame;
}
[Serializable]
public struct JumpStateProperties
{
    [Header("Jump Stuff")]
    public float initialJumpForce;
    public float jumpForce;
    public float jumpDuration;
    public float jumpTimer;
}
[Serializable]
public struct FallStateProperties
{
    public float fallSpeed;
}
[Serializable]
public struct RunStateProperties
{
    [Header("Run Stuff")]
    [Range(0, 1)]
    public float idleThreshold;
    [Range(0, 1)]
    public float walkThreshold;
    [Range(0, 1)]
    public float runThreshold;
    public float accel;
    public float turnRate;
    public float maxRunSpeed;
    public float deaccel;
    public float runSkidWindow;
    public float runSkidTime;

    [Range(0,360)]
    public float breakAngle;
}
[Serializable]
public struct RunSkidStateProperties
{
    public float runSkidSpeed;
    public float runSkidDecay;
}
[Serializable]
public struct WalkStateProperties
{
    [Header("Walk Stuff")]
    [Range(0, 1)]
    public float idleThreshold;
    [Range(0, 1)]
    public float walkThreshold;
    [Range(0, 1)]
    public float runThreshold;
    public float maxWalkSpeed;
    public float walkAccel;

}
[Serializable]
public struct GrindStateProperties
{

}
[Serializable]
public struct ModfiableAttributes
{
    public int health;
    public int maxSprayCan;
    public float tagSpeed;

    public float fallSpeed;
    public float jumpFloatTime;
    public int jumpSquat;
    public int landingLag;

    public float grindStartingAcceleration;
    public float airAcceleration;
    public float airDecceleration;
    public float boostDashSpeed;
    public float forwardAccelerationSpeed;
    public float cornering;
    public float corneringAccel;
    public float corneringDeaccel;
    public float corneringBoost;
    public float landingBoostAccelCornering;
    public float halfPipeCornering;
    public float airRotateControl;
    public bool airControl;


}