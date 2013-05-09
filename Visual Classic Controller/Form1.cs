// Default
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// What we added
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Threading;
using System.IO.Ports;

namespace Visual_Classic_Controller
{
    public partial class Form1 : Form
    { 
        // etc varibles
        SerialPort serial = new SerialPort();
        bool portError = false;
        bool running = false;
        String portName;

        public Form1()
        {
            // etc
            InitializeComponent();
            this.DoubleBuffered = true;
            resetAnimation();
            
            // serial drop down menu
            String[] ports = SerialPort.GetPortNames();
            for (int i = 0; i < ports.Length; i++)
            {
                ToolStripMenuItem tempMenu = new ToolStripMenuItem();
                tempMenu.Text = ports[i];
                tempMenu.Click += new System.EventHandler(assignSerial);
                this.serialToolStripMenuItem1.DropDownItems.Add(tempMenu);
                tempMenu = null;
            }

        }

        public void runProgram()
        {
            // FPS
            double FPS = 30.0;
            long firstTick = 0;
            long secondTick = 0;
            double interval = (double)Stopwatch.Frequency / FPS;

            // Serial
            serial.Open();
            serial.ReadTimeout = 5000;

            // Etc
            String input = "";
            try
            {
                input = serial.ReadLine();
            }
            catch (TimeoutException)
            {
                selectedSerialText.Text = "ERROR: PORT INVALID";
                stopProgram();
            }

            bool doubleR = false;
            bool doubleL = false;

            // begininng of the loop
            while (!this.IsDisposed && !portError)
            {
                // etc
                this.Text = "Visual Classic Controller [Started]";
                selectedSerialText.Text = portName + " is in use";
                // Update the second counter
                secondTick = Stopwatch.GetTimestamp();

                if (secondTick >= firstTick + interval)
                {
                    // Read the input
                    // try with a low timeout to see if we need to reset the animation due to loading on the n64
                    try
                    {
                        serial.ReadTimeout = 100;
                        input = serial.ReadLine();
                    }
                    // must be loading a screen, reset the timeout to a reasonable rate and try reading it again
                    catch (TimeoutException)
                    {
                        resetButtonAnimation();
                        try
                        {
                            serial.ReadTimeout = 5000;
                            input = serial.ReadLine();
                        }
                        // guess we couldn't read anything at all, stop the program and let the user adjust
                        catch (TimeoutException)
                        {
                            selectedSerialText.Text = "ERROR: ERROR READING PORT";
                            stopProgram();
                            break;
                        }
                    }
                    String[] inputSplit = input.Split(' ');

                    // basic sizing check for input
                    if (inputSplit[0].Length < 8)
                    {
                        selectedSerialText.Text = "ERROR: INVALID INFORMATION";
                        stopProgram();
                        break;
                    }

                    // check to see if both R's are pressed because I'm lazy to do it another way
                    if (input[4] == '1' && input[7] == '1')
                    {
                        doubleR = true;
                        doubleR_pressed.Visible = true;
                    }
                    else
                    {
                        doubleR = false;
                        doubleR_pressed.Visible = false;
                    }
                    // same with L's now
                    if (input[2] == '1' && input[8] == '1')
                    {
                        doubleL = true;
                        doubleL_pressed.Visible = true;
                    }
                    else
                    {
                        doubleL = false;
                        doubleL_pressed.Visible = false;
                    }

                    // line breaks into a,b,l,start,r,x,y,rz,lz followed the control stick

                    // a pressed
                    if (input[0] == '1')
                    {
                        a_pressed.Visible = true;
                    }
                    else
                    {
                        a_pressed.Visible = false;
                    }
                    
                    // b pressed
                    if (input[1] == '1')
                    {
                        b_pressed.Visible = true;
                    }
                    else
                    {
                        b_pressed.Visible = false;
                    }

                    // l pressed
                    if (input[2] == '1' && !doubleL)
                    {
                        l_pressed.Visible = true;
                    }
                    else
                    {
                        l_pressed.Visible = false;
                    }

                    // start pressed
                    if (input[3] == '1')
                    {
                        start_pressed.Visible = true;
                    }
                    else
                    {
                        start_pressed.Visible = false;
                    }

                    // r pressed
                    if (input[4] == '1' && !doubleR)
                    {
                        r_pressed.Visible = true;
                    }
                    else
                    {
                        r_pressed.Visible = false;
                    }

                    // x pressed
                    if (input[5] == '1')
                    {
                        x_pressed.Visible = true;
                    }
                    else
                    {
                        x_pressed.Visible = false;
                    }

                    // y pressed
                    if (input[6] == '1')
                    {
                        y_pressed.Visible = true;
                    }
                    else
                    {
                        y_pressed.Visible = false;
                    }

                    // zr pressed
                    if (input[7] == '1' && !doubleR)
                    {
                        zr_pressed.Visible = true;
                    }
                    else
                    {
                        zr_pressed.Visible = false;
                    }

                    // zl pressed
                    if (input[8] == '1' && !doubleL)
                    {
                        zl_pressed.Visible = true;
                    }
                    else
                    {
                        zl_pressed.Visible = false;
                    }


                    // Grab the stick x and y, parse it and change the value then set the stick
                    int lStickX, lStickY, rStickX, rStickY;
                    try
                    {
                        lStickX = Convert.ToInt32(inputSplit[1]);
                        lStickY = Convert.ToInt32(inputSplit[2]);
                        rStickX = Convert.ToInt32(inputSplit[3]);
                        rStickY = Convert.ToInt32(inputSplit[4]);
                    }
                    catch (FormatException)
                    {
                        selectedSerialText.Text = "ERROR: INVALID INFORMATION";
                        stopProgram();
                        break;
                    }
                    catch (OverflowException)
                    {
                        selectedSerialText.Text = "ERROR: INVALID INFORMATION";
                        stopProgram();
                        break;
                    }


                    // Convert the values and adjust
                    lStickX = Convert.ToInt32(lStickX - 32);
                    lStickY = Convert.ToInt32((lStickY - 32) * -1);
                    rStickX = Convert.ToInt32((rStickX * 2) - 32);
                    rStickY = Convert.ToInt32(((rStickY * 2) - 32) *-1);

                    // get rid of the jittering if the stick barely moves
                    if (lStickX < 3 && lStickX > -3)
                        lStickX = 0;
                    if (lStickY < 3 && lStickY > -3)
                        lStickY = 0;
                    if (rStickX < 3 && rStickX > -3)
                        rStickX = 0;
                    if (rStickY < 3 && rStickY > -3)
                        rStickY = 0;

                    // move the stick around
                    // hide the stick, move the stick, and show the stick at the new location to remove graphical glitches
                    leftStick.Visible = false;
                    leftStick.Location = new Point((178+lStickX), (246+lStickY));
                    leftStick.Visible = true;
                    rightStick.Visible = false;
                    rightStick.Location = new Point((338 + rStickX), (246 + rStickY));
                    rightStick.Visible = true;

                    // Refresh the form
                    this.Invalidate();
                }

                Application.DoEvents();

                // Freeing up the CPU
                Thread.Sleep(1);
            }

            portError = false; // reset varible
            // Close serial
            serial.Close();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!running)
            {
                running = true;
                runProgram();
            }
        }

        private void assignSerial(object sender, EventArgs e)
        {
            if (!running)
            {
                ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
                serial.PortName = clickedItem.Text;
                selectedSerialText.Text = clickedItem.Text + " is selected";
                portName = clickedItem.Text;
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopProgram();
        }

        // make the program stop looping
        private void stopProgram()
        {
            if (running)
            {
                portError = true;
                this.Text = "Visual Classic Controller [Stopped]";
                resetAnimation();
                running = false;
            }
        }

        // reset only the button animations to how it started; this was made just so it'll look nicer between screens
        private void resetButtonAnimation()
        {
            // hide buttons
            y_pressed.Visible = false;
            a_pressed.Visible = false;
            x_pressed.Visible = false;
            b_pressed.Visible = false;
            r_pressed.Visible = false;
            zr_pressed.Visible = false;
            l_pressed.Visible = false;
            zl_pressed.Visible = false;
            start_pressed.Visible = false;
            doubleR_pressed.Visible = false;
            doubleL_pressed.Visible = false;
        }

        // reset the animation back to default like how it started
        private void resetAnimation()
        {
            resetButtonAnimation();

            leftStick.Visible = false;
            rightStick.Visible = false;
            leftStick.Location = new Point(178, 246);
            rightStick.Location = new Point(338, 246);
            leftStick.Visible = true;
            rightStick.Visible = true;

            this.Invalidate();
            Application.DoEvents();
        }
    }
}
