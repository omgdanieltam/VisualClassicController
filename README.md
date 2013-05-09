Visual Classic Controller
=======================

On screen display for the Classic Controller Pro to N64 Adapter

INTRODUCTION
=======================
Based on serial output of the Classic Controller Pro to N64 Arduino Adapter. Written in C# and used WinForms. The executable file is located in the Visual Classic Controller folder. Basic bugs have been taken care of, and more are being worked.

FORMATTING
=======================
Since the output of the adapter is set a certain way, the input is also set in a certain way. The order that is read is a special format as well.

The output from serial would be: 010001001 34 28 17 13

The format of this text is: A, B, L, START, R, X, Y, RZ, LZ, LEFT STTICK X, LEFT STICK Y, RIGHT STICK X, RIGHT STICK Y

So the above could would mean B, X, and LZ is pressed with the left stick at positions (34,28) and the right stick is at the position (17,13). 

The program will read this code and flash a red button or move the stick according to the output. There has been no reports of lag or any isses.
