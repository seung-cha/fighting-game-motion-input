using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Demo_InputSystem
{
    public class CommandChain
    {
        private List<ICommand> commands;

        public CommandChain()
        {
            commands = new List<ICommand>();
        }

        public void AddCommand(ICommand command)
        {
            commands.Add(command);
        }

        public void Update(InputBuffer buffer, Animator animator)
        {
            foreach(ICommand command in commands)
            {
                if(command.ProcessCommand(buffer, animator))
                {
                    break;
                }

            }
        }


    }

}
