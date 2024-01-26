using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Demo_InputSystem
{ 
    public class Command_Fireball : ICommand
    {
        private int tol = 20;

        public bool ProcessCommand(InputBuffer buffer, Animator animator)
        {
            // Confirm that 236 is pressed in given delay.
            int frame;
            InputButton btn = buffer.FindButton(InputButton.LIGHT, false, out frame, 0, true);

            if (btn == null || frame > tol)
            {
                // Button is not pressed or was pressed too late.
                return false;
            } 


            int prog = frame;
            InputButton fwd = buffer.FindButton(InputButton.FORWARD, false, out frame, prog);

            if (fwd == null || frame > tol)
            {
                return false;
            }

            prog += frame;

            // Get the down foward input.
            InputButton df = buffer.FindDiagonalButton(InputButton.DOWNFORWARD, out frame, prog);

            // If both are null, return false.
            if (df == null || frame > tol)
            {
                return false;
            }


            prog += frame;

            // lastly, get the down (or down back) button.
            InputButton dwn = buffer.FindButton(InputButton.DOWN, false, out frame, prog + 1);

            int frame2;
            InputButton db = buffer.FindDiagonalButton(InputButton.DOWNBACK, out frame2, prog + 1);

            if (((dwn == null) || frame > tol) && ((db == null) || frame2 > tol))
            {
                return false;
            }



            //Fireball command is casted.
            Debug.Log("Fireball is casted!");
            animator.SetTrigger("Fireball");
            buffer.Clear();


            return true;
        }

       
    }
}

