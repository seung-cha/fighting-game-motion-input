using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Demo_InputSystem
{ 
    public interface ICommand
    {
        bool ProcessCommand(InputBuffer buffer, Animator animator);
    }
}

