using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Demo_InputSystem;

public class FighterController : MonoBehaviour
{
    [SerializeField]
    private InputSystem inputSystem;
    CommandChain chain;
    [SerializeField]
    private Animator animator;

    void Start()
    {
        chain = new CommandChain();

        chain.AddCommand(new Command_DragonPunch());
        chain.AddCommand(new Command_Fireball());

    }

    // Update is called once per frame
    void Update()
    {
        chain.Update(inputSystem.Buffer, animator);

    }
}
