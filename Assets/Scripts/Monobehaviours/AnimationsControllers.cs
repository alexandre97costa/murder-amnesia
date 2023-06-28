using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsControllers : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private AnimatorOverrideController[] overrideControllers;
    [SerializeField] private AnimationOverrider overrider;
    private Animator animator;

    [Space(10)]
    [Header("Sounds")]
    public List<AudioClip> listAudios;
    public AudioSource audioSource;
    private AudioClip auxAudioClip;
    private float maxPlayerSpeed = 10f;
    private float maxPitchMultiplier = 2f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        animator = GetComponent<Animator>();
    }

    public void Set(int value) { overrider.SetAnimations(overrideControllers[value]); }

    public void IdleAction() { Set(15); animator.SetBool("isAction", false); IdleSound(); }

    public void FallAction() { Set(0); animator.SetBool("isAction", true); DisableAllSounds(); }

    public void JumpAction() { Set(1); animator.SetBool("isAction", true); DisableAllSounds(); EnabledSound("jump01"); }

    public void RunBackAction() { Set(2); animator.SetBool("isAction", true); DisableAllSounds(); EnabledSound("runSprint"); }

    public void RunLeftBackAction() { Set(3); animator.SetBool("isAction", true); DisableAllSounds(); EnabledSound("runSprint"); }

    public void RunRightBackAction() { Set(4); animator.SetBool("isAction", true); DisableAllSounds(); EnabledSound("runSprint"); }

    public void RunAction() { Set(5); animator.SetBool("isAction", true); DisableAllSounds(); EnabledSound("runSprint"); }

    public void RunLeftAction() { Set(6); animator.SetBool("isAction", true); DisableAllSounds(); EnabledSound("runSprint"); }

    public void RunRightAction() { Set(7); animator.SetBool("isAction", true); DisableAllSounds(); EnabledSound("runSprint"); }

    public void SprintAction() { Set(8); animator.SetBool("isAction", true); DisableAllSounds(); EnabledSound("runSprint"); }

    public void SprintLeftAction() { Set(9); animator.SetBool("isAction", true); DisableAllSounds(); EnabledSound("runSprint"); }
    
    public void SprintRightAction() { Set(10); animator.SetBool("isAction", true); DisableAllSounds(); EnabledSound("runSprint"); }

    public void StrafeLeftAction() { Set(11); animator.SetBool("isAction", true); DisableAllSounds(); EnabledSound("runSprint"); }
    
    public void StrafeRightAction() { Set(12); animator.SetBool("isAction", true); DisableAllSounds(); EnabledSound("runSprint"); }
    
    public void RunJumpAction() { Set(13); animator.SetBool("isAction", true); DisableAllSounds(); }

    public void SlideAction() { Set(14); animator.SetBool("isAction", true); DisableAllSounds(); }

    public void CrounchAction() { Set(16); animator.SetBool("isAction", true); DisableAllSounds(); EnabledSound("crouch"); }
    
    public void CrounchFrontAction() { Set(17); animator.SetBool("isAction", true); DisableAllSounds(); EnabledSound("crouch"); }

    public void CrounchLeftAction() { Set(18); animator.SetBool("isAction", true); DisableAllSounds(); EnabledSound("crouch"); }

    public void CrounchRightAction() { Set(19); animator.SetBool("isAction", true); DisableAllSounds(); EnabledSound("crouch"); }

    public void JumpMidAirAction() { Set(20); animator.SetBool("isAction", true); DisableAllSounds(); }
    
    public void LandAction() { Set(21); animator.SetBool("isAction", true); DisableAllSounds(); }

    private void EnabledSound(string soundIndex)
    {
        int index = listAudios.FindIndex(x => x.name == soundIndex);

        if(index == -1) { Debug.LogError("Audio não existe"); return; }

        auxAudioClip = listAudios[index];
        audioSource.clip = listAudios[index];
        audioSource.Play();
    }

    private void DisableAllSounds()
    {
        if(audioSource.clip != auxAudioClip)
        {
            audioSource.Stop();
        }
    }

    private void IdleSound()
    {
        audioSource.Stop();
        Debug.Log("Idle");
    }
}
