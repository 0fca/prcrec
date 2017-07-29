# prcrec
_MacroPath(codename: prcrec)_ is a simple recorder which records user's actions taken using keyboard and mouse wherever 
on the screen, doesnt matter on what window. 

MacroPath is in BETA phase. It actually supports recording ASCII chars, it recognizes all system key like **_Alt_**, **_AltGr_**,**_Shift_** etc.

I used Win32 API to make all the application logic and a bit of reflection mechanism. The UI is built using modern WPF(Modern UI). 

The app can:
* record mouse x,y coodrinates according to foreground window,
* record keyboard input whenever it is typed(does not need to enter text to textbox, just type),
* detect what window is focused,
* make a screen capture of the area near to the mouse click(in theory now),
* save the macro to the CSV file,
* work in the background without showing on the taskbar.

_Main view of an app_

![Main view](https://github.com/Obsidiam/prcrec/blob/master/Screenshot_2017-07-29_14-46-07.png)

_App in action_

![App in action](https://github.com/Obsidiam/prcrec/blob/master/VirtualBox_Win10_29_07_2017_14_46_47.png)

**IMPORTANT NOTES**:
The architecture of the program causes it to save the keyboard input as it is so the Shift key is like a dynamic-state key and Alts are too.
Please, mind it when recording input from the keyboard because writing for example capital letters looks like:
Press Shift, release it and then press the chartacter key; **_DO NOT_** do it at the same time, it wont give any effect in the macro!



