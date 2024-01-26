using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demo_InputSystem
{

    public class Command_DragonPunch : ICommand
    {
        private int tol = 28;

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


            InputButton df = buffer.FindDiagonalButton(InputButton.DOWNFORWARD, out frame, prog);

            if (df == null || frame - prog > tol)
            {
                return false;
            }

            prog += frame;

            // Get the down foward input.
            InputButton dn = buffer.FindButton(InputButton.DOWN, false, out frame, prog);

            // If both are null, return false.
            if (dn == null || frame - prog > tol)
            {
                return false;
            }


            prog += frame;


            // Return if back is pressed.
            InputButton counter = buffer.FindButton(InputButton.NEUTRAL, false, out frame, prog);


            counter = buffer.FindButton(InputButton.BACK, false, out frame, prog);

            if (counter != null && frame < tol)
            {
                return false;
            }



            // lastly, get the forward button.
            InputButton fwd = buffer.FindButton(InputButton.FORWARD, false, out frame, prog + 1);

            if ((fwd == null) || frame - prog > tol)
            {
                return false;
            }



            //Fireball command is casted.
            Debug.Log("Dragon Punch is casted!");
            animator.SetTrigger("DP");
            buffer.Clear();


            return true;
        }
    }



}
