using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class MoventsPlayer : MonoBehaviour
{
    private AnimationsControllers animationsControllers;
    private StarterAssetsInputs keyInputs;
    private PlayerMovement playerMovement;

    [SerializeField] private float velocitySlide = 7;

    //Stand By
    private CharacterController characterController;


    void Start()
    {
        // Inicialize a vari�vel animationsControllers aqui
        animationsControllers = GetComponent<AnimationsControllers>();
        keyInputs = GetComponent<StarterAssetsInputs>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    /*------------HORIZONTAL------------*/
    /*
        keyInputs.move.x.Equals(-1) -> Esquerda
        keyInputs.move.x.Equals(1) -> Direita
    */

    /*------------VERTICAL------------*/
    /*
        keyInputs.move.y.Equals(-1) -> Trás
        keyInputs.move.y.Equals(1) -> Frente
    */

    void Update()
    {
        //Slide & Crounch
        if(playerMovement.isCrouched) { AnimationCrouch(); return; }

        //Air
        if(keyInputs.jump) { AnimationJump(); return; }

        //Sprints
        AnimationSprint();

        //Run
        if(keyInputs.move.y.Equals(1)) { AnimationRun(); return; }

        //Back
        if (keyInputs.move.y.Equals(-1)) { AnimationBack(); return; }


        //Strafe
        if (keyInputs.move.x.Equals(-1)) { animationsControllers.StrafeLeftAction(); return; }

        if (keyInputs.move.x.Equals(1)) { animationsControllers.StrafeRightAction(); return; }

        animationsControllers.IdleAction();
    }

    private void AnimationCrouch()
    {
        //Verificar a velocidade
        if (keyInputs.move.y.Equals(1) && playerMovement.TotalSpeed > velocitySlide) { animationsControllers.SlideAction(); return; }

        if (keyInputs.move.x.Equals(-1) && keyInputs.move.y.Equals(1)) { animationsControllers.CrounchLeftAction(); return; }

        if (keyInputs.move.x.Equals(1) && keyInputs.move.y.Equals(1)) { animationsControllers.CrounchRightAction(); return; }

        if (keyInputs.move.y.Equals(1)) { animationsControllers.CrounchFrontAction(); return; }

        if (keyInputs.move.x.Equals(-1)) { animationsControllers.CrounchLeftAction(); return; }

        if (keyInputs.move.x.Equals(1)) { animationsControllers.CrounchRightAction(); return; }

        animationsControllers.CrounchAction(); return;
    }

    private void AnimationJump()
    {
        if (keyInputs.move.y.Equals(1)) { animationsControllers.RunJumpAction(); return; }

        animationsControllers.JumpAction();
    }

    private void AnimationSprint()
    {
        //if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.LeftShift)) { animationsControllers.SprintLeftAction(); return; }

        //if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftShift)) { animationsControllers.SprintRightAction(); return; }

        //if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftShift)) { animationsControllers.SprintAction(); return; }
    }

    private void AnimationRun()
    {
        if (keyInputs.move.x.Equals(-1)) { animationsControllers.RunLeftAction(); return; }

        if (keyInputs.move.x.Equals(1)) { animationsControllers.RunRightAction(); return; }

        animationsControllers.RunAction();
    }

    private void AnimationBack()
    {
        if (keyInputs.move.x.Equals(-1)) { animationsControllers.RunLeftBackAction(); return; }

        if (keyInputs.move.x.Equals(1)) { animationsControllers.RunRightBackAction(); return; }

        animationsControllers.RunBackAction();
    }
}
