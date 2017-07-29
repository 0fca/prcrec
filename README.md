# prcrec
_MacroPath(codename: prcrec)_ is a simple recorderd which records user's actions taken using keyboard and mouse wherever 
on the screen, doesnt matter on what window. 

MacroPath is in BETA phase. It actually supports recording ASCII chars, it recognizes all system key like **_Alt_**, **_AltGr_**,**_Shift_** etc.

I used Win32 API to make all the application logic and a bit of Reflection mechanism. The UI is built using modern WPF. 

The app can:
* record mouse x,y coodrinates according to foreground window,
* record keyboard input whenever it is typed(does not need to enter text to textbox, just type),
* detect what window is focused,
* make a screen capture of the area near to the mouse click(in theory now),
* save the macro to the CSV file,
* work in the background without showing on the taskbar.

Notes:
The architecture of the program causes it to save the keyboard input as it is so the Shift key is like a dynamic-state key and Alts are too.
Please, mind it when recording input from the keyboard because writing for example capital letters looks like:
Press Shift, release it and then press the chartacter key; **_DO NOT_** do it at the same time, it wont givre any effect in the macro!



