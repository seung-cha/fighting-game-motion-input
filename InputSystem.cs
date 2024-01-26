using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Demo_InputSystem
{
    using Button = System.Byte;


    public class InputSystem : MonoBehaviour
    {
        private InputBuffer inputBuffer;
        public InputBuffer Buffer { get { return inputBuffer; } }


        // Start is called before the first frame update
        void Start()
        {
            inputBuffer = new InputBuffer();
            Application.targetFrameRate = 60;
        }

        // Update is called once per frame
        void Update()
        {
            // Hard-coded input logic
            inputBuffer.NewFrame();
            int axisX = 0, axisY = 0;

            // Can also use Input.GetAxis()

            if (Input.GetKey(KeyCode.W))
            {
                axisY++;
            }

            if (Input.GetKey(KeyCode.S))
            {
                axisY--;
            }

            if (Input.GetKey(KeyCode.A))
            {
                axisX--;
            }

            if (Input.GetKey(KeyCode.D))
            {
                axisX++;
            }

            if (Input.GetKey(KeyCode.J))
            {
                inputBuffer.AddButton(InputButton.LIGHT, false);
            }

            if (Input.GetKey(KeyCode.K))
            {
                inputBuffer.AddButton(InputButton.MEDIUM, false);
            }

            if (Input.GetKey(KeyCode.L))
            {
                inputBuffer.AddButton(InputButton.HEAVY, false);
            }



            if (axisX == 1)
            {
                inputBuffer.AddButton(InputButton.FORWARD, false);
            }
            else if (axisX == -1)
            {
                inputBuffer.AddButton(InputButton.BACK, false);
            }
            else if (axisY == 0)
            {
                inputBuffer.AddButton(InputButton.NEUTRAL, false);
            }

            if (axisY == 1)
            {
                inputBuffer.AddButton(InputButton.UP, false);
            }
            else if (axisY == -1)
            {
                inputBuffer.AddButton(InputButton.DOWN, false);
            }

        }

        private void OnGUI()
        {
            int width = 0;
            int height = 0;


            // Iterate through the input system and display the log
            for (int i = 0; i < inputBuffer.Buffer.Count; i++)
            {
                InputButton[] button = inputBuffer.Buffer[i];

                Rect rect = new Rect(width, height + 30, 30, 30);
                Rect frameRect = new Rect(rect.x + 30, height, 30, 30);
                Rect attackRect = new Rect(frameRect.x + 30, height, 30, 30);

                for(int j = 0; j < InputButton.LEN; j++)
                {
                    rect.x += 30;
                    if (button[j].Released)
                        continue;

                    GUI.Label(rect, button[j].Transitioned ? "*" + button[j].Frame.ToString() : button[j].Frame.ToString());

                }
                height += 10;








            }
        }


    }


    public class InputButton
    {
        public const Button NEUTRAL = 0;
        public const Button UP = 1;
        public const Button DOWN = 2;
        public const Button FORWARD = 3;
        public const Button BACK = 4;
        public const Button LIGHT = 5;
        public const Button MEDIUM = 6;
        public const Button HEAVY = 7;

        public const Button DOWNFORWARD = 8;
        public const Button DOWNBACK = 9;
        public const Button UPFORWARD = 10;
        public const Button UPBACK = 11;


        /// <summary>
        /// Number of buttons
        /// </summary>
        public const Button LEN = 8;



        public int Frame = 1;
        public bool Released = false;
        public bool Consumed = false;
        public bool Transitioned = false;   // True if Released changed from the previous frame.
    }




    public class InputBuffer
    {
        /// <summary>
        /// How long the buffer should store input?
        /// </summary>
        public const int BUFFERLEN = 60;



        private List<InputButton[]> buffer;
        public List<InputButton[]> Buffer { get { return buffer; } }


        public InputBuffer()
        {
            buffer = new List<InputButton[]>();
        }

        /// <summary>
        /// Call this to receive a fresh set of input.
        /// All inputs are initialised as released.
        /// </summary>
        public void NewFrame()
        {
            buffer.Insert(0, new InputButton[InputButton.LEN]);

            // Add buttons, as released.
            AddButton(InputButton.NEUTRAL, true);
            AddButton(InputButton.UP, true);
            AddButton(InputButton.DOWN, true);
            AddButton(InputButton.FORWARD, true);
            AddButton(InputButton.BACK, true);

            AddButton(InputButton.LIGHT, true);
            AddButton(InputButton.MEDIUM, true);
            AddButton(InputButton.HEAVY, true);




            // Discard the oldest set of input if there is one.
            if (buffer.Count > BUFFERLEN)
            {
                buffer.RemoveAt(buffer.Count - 1);
            }


        }

        /// <summary>
        /// Add a button input to current frame.
        /// Multiple calls with the same argument override the button with the last call.
        /// </summary>
        /// <param name="button">Button to add, obtained from the Input class</param>
        /// <param name="released">Is the button to add released in this frame?</param>
        public void AddButton(Button button, bool released)
        {
            InputButton input = new InputButton();
            input.Released = released;

            // Check if the button is being held (or released)
            if (buffer.Count > 1)
            {
                InputButton prev = buffer[1][button];
                if (prev.Released == released)
                {
                    // Same button is being held (or released). Increment the frame counter.
                    input.Frame = prev.Frame + 1;
                }
                else
                {
                    // Button held state changed
                    input.Transitioned = true;
                }
            }


            buffer[0][button] = input;
        }

        /// <summary>
        /// Wipe out the buffer.
        /// </summary>
        public void Clear()
        {
            buffer.Clear();
        }

        /// <summary>
        /// Find the non-diagonal button in the buffer with given conditions.
        /// If button is directional, strictly find the button (i.e ignore the button if other directional buttons are pressed on the same frame)
        /// </summary>
        /// <param name="button">Button to find</param>
        /// <param name="released">Whether the button should be released or not.</param>
        /// <param name="frame">How long ago this button was received?</param>
        /// <param name="strict">If true, find the button whose Transitioned field is true (released to pressed or pressed to released). This is always true if released is true.</param>
        /// <param name="delay">If given, ignore buttons more recent than this.</param>
        /// <returns>Object if found, null otherwise.</returns>
        public InputButton FindButton(Button button, bool released, out int frame, int delay = 0, bool strict = false)
        {
            frame = delay;

            for (int i = delay; i < buffer.Count; i++)
            {

                InputButton inputButton = buffer[i][button];


                if ((inputButton.Transitioned && inputButton.Released == released) || (!strict && !inputButton.Released))
                {
                        if (!released && button <= 4)
                        {
                            // See if other keys are pressed
                            bool other = false;

                            for (int j = 0; j <= 4; j++)
                            {
                                if (button == j)
                                    continue;

                                if (!buffer[i][j].Released)
                                {

                                    other = true;
                                    break;
                                }
                            }


                        if (other)  // Other key is pressed. Do not return this.
                        {
                            continue;
                        }

                    }

                    return inputButton;
                }

                frame++;
            }

            return null;
        }



        /// <summary>
        /// Strictly find the frame at which the provided diagonal button was pressed.
        /// Returns the button with the minimum frame if exists, null otherwise.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="frame"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public InputButton FindDiagonalButton(Button button, out int frame, int delay = 0)
        {
            Button btn1, btn2;
            frame = 0;

            switch (button)
            {
                case InputButton.DOWNFORWARD:
                    btn1 = InputButton.DOWN;
                    btn2 = InputButton.FORWARD;
                    break;
                case InputButton.DOWNBACK:
                    btn1 = InputButton.DOWN;
                    btn2 = InputButton.BACK;
                    break;
                case InputButton.UPFORWARD:
                    btn1 = InputButton.UP;
                    btn2 = InputButton.FORWARD;
                    break;
                case InputButton.UPBACK:
                    btn1 = InputButton.UP;
                    btn2 = InputButton.BACK;
                    break;

                default:
                    return null;
            }

            int frame1, frame2;

            InputButton button1 = FindEarliestButton(btn1, false, out frame1, delay);
            InputButton button2 = FindEarliestButton(btn2, false, out frame2, delay);


            while (frame1 != frame2 && button1 != null && button2 != null)
            {
                if (frame1 > frame2)
                {
                    button2 = FindEarliestButton(btn2, false, out frame2, frame1);
                }
                else
                {
                    button1 = FindEarliestButton(btn1, false, out frame1, frame2);
                }
            }

            frame = frame1;

            if (button1 == null || button2 == null)
            {
                return null;
            }

            return button1.Frame > button2.Frame ? button2 : button1;
        }


        /// <summary>
        /// Get the frame of the earliest occurrence of given button.
        /// </summary>
        /// <returns></returns>
        private InputButton FindEarliestButton(Button button, bool released, out int frame, int delay = 0)
        {
            frame = delay;
            for (int i = delay; i < buffer.Count; i++)
            {
                InputButton[] btn = buffer[i];

                if (btn[button].Released == released)
                    return btn[button];

                frame++;
            }

            return null;
        }





    }
}