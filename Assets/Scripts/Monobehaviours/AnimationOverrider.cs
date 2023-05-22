using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverrider : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        this.animator = GetComponent<Animator>();
    }

    public void SetAnimations(AnimatorOverrideController overrideController)
    {
        this.animator.runtimeAnimatorController = overrideController;
    }
}
