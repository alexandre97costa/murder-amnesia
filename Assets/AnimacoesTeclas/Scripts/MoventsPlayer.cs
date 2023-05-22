using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoventsPlayer : MonoBehaviour
{
    private AnimationsControllers animationsControllers;

    void Start()
    {
        // Inicialize a variável animationsControllers aqui
        animationsControllers = GetComponent<AnimationsControllers>();
    }

    void Update()
    {
        //Slide & Crounch
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftShift)) { animationsControllers.SlideAction(); return; }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.UpArrow)) { animationsControllers.CrounchFrontAction(); return; }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftArrow)) { animationsControllers.CrounchLeftAction(); return; }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightArrow)) { animationsControllers.CrounchRightAction(); return; }

        if (Input.GetKey(KeyCode.LeftControl)) { animationsControllers.CrounchAction(); return; }


        //Air
        if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.UpArrow)) { animationsControllers.RunJumpAction(); return; }

        if (Input.GetKey(KeyCode.Space)) { animationsControllers.JumpAction(); return; }


        //Sprints
        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.LeftShift)) { animationsControllers.SprintLeftAction(); return; }

        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftShift)) { animationsControllers.SprintRightAction(); return; }
        
        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftShift)) { animationsControllers.SprintAction(); return; }


        //Run
        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow)) { animationsControllers.RunLeftAction(); return; }

        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow)) { animationsControllers.RunRightAction(); return; }

        if (Input.GetKey(KeyCode.UpArrow)) { animationsControllers.RunAction(); return; }


        //Back
        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow)) { animationsControllers.RunLeftBackAction(); return; }

        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow)) { animationsControllers.RunRightBackAction(); return; }

        if (Input.GetKey(KeyCode.DownArrow)) { animationsControllers.RunBackAction(); return; }


        //Strafe
        if (Input.GetKey(KeyCode.LeftArrow)) { animationsControllers.StrafeLeftAction(); return; }

        if (Input.GetKey(KeyCode.RightArrow)) { animationsControllers.StrafeRightAction(); return; }

        animationsControllers.IdleAction();
    }
}
