using UnityEngine;

public static class Animations
{
    public static readonly int Idle = Animator.StringToHash("Idle");
    public static readonly int TurnLeft = Animator.StringToHash("TurnLeft");
    public static readonly int TurnRight = Animator.StringToHash("TurnRight");
    public static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    public static readonly int IsReloading = Animator.StringToHash("IsReloading");
    public static readonly int Death = Animator.StringToHash("Death");

}
