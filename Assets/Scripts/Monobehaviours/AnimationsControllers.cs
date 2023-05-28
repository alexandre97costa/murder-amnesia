using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsControllers : MonoBehaviour
{
    [SerializeField] private AnimatorOverrideController[] overrideControllers;
    [SerializeField] private AnimationOverrider overrider;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Set(int value) { overrider.SetAnimations(overrideControllers[value]); }

    public void IdleAction() { Set(15); animator.SetBool("isAction", false); }

    public void FallAction() { Set(0); animator.SetBool("isAction", true); }

    public void JumpAction() { Set(1); animator.SetBool("isAction", true); }

    public void RunBackAction() { Set(2); animator.SetBool("isAction", true); }

    public void RunLeftBackAction() { Set(3); animator.SetBool("isAction", true); }

    public void RunRightBackAction() { Set(4); animator.SetBool("isAction", true); }

    public void RunAction() { Set(5); animator.SetBool("isAction", true); }

    public void RunLeftAction() { Set(6); animator.SetBool("isAction", true); }

    public void RunRightAction() { Set(7); animator.SetBool("isAction", true); }

    public void SprintAction() { Set(8); animator.SetBool("isAction", true); }

    public void SprintLeftAction() { Set(9); animator.SetBool("isAction", true); }
    
    public void SprintRightAction() { Set(10); animator.SetBool("isAction", true); }

    public void StrafeLeftAction() { Set(11); animator.SetBool("isAction", true); }
    
    public void StrafeRightAction() { Set(12); animator.SetBool("isAction", true); }
    
    public void RunJumpAction() { Set(13); animator.SetBool("isAction", true); }

    public void SlideAction() { Set(14); animator.SetBool("isAction", true); }

    public void CrounchAction() { Set(16); animator.SetBool("isAction", true); }
    
    public void CrounchFrontAction() { Set(17); animator.SetBool("isAction", true); }

    public void CrounchLeftAction() { Set(18); animator.SetBool("isAction", true); }

    public void CrounchRightAction() { Set(19); animator.SetBool("isAction", true); }
}