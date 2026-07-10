\# Mobile Runner Prototype



\## Project Description



Mobile Runner Prototype is a 2D mobile lane runner created in Unity 6. The player moves between lanes to avoid falling obstacles. The prototype focuses on mobile touch controls, sensor feedback, readable mobile UI, and Android tablet testing.



This project was created for Week 2 of my mobile game development course. The goal was to make a playable mobile prototype that demonstrates advanced touch controls, sensor integration, mobile-appropriate design, and physical device testing.



\## Control Scheme



The game is designed for portrait orientation and simple mobile play.



\- Swipe left to move one lane left.

\- Swipe right to move one lane right.

\- Swipe up to jump.

\- Tap input displays feedback.

\- Tilt the tablet to make the player lean.

\- Status text gives visual feedback for player actions.



\## Mobile Features



\### Advanced Touch Controls



The prototype uses swipe detection through Unity's New Input System and Enhanced Touch support. Swipes are used for lane changes and jumping. The game also includes visual feedback through status text and player color changes.



\### Sensor Integration



The prototype uses accelerometer input to make the player visually lean when the tablet is tilted. The accelerometer value is smoothed using Mathf.Lerp so the sensor feedback feels more stable and less shaky.



\### Mobile-Appropriate Design



The game is locked to portrait orientation. The controls use large swipe gestures instead of small buttons, which makes the game easier to play on a mobile device. The UI was adjusted after simulator testing so the instruction text, status text, and FPS counter are easier to read on a phone-sized screen.



\### Performance Documentation



The prototype includes an FPS counter on screen. The game uses simple 2D sprites, basic obstacle spawning, and lightweight scripts to help maintain stable performance on mobile hardware.



\## Build Instructions



Platform: Android  

Orientation: Portrait  

Unity Version: Unity 6  

Input System: Unity New Input System  

Minimum Android Version: Android 8.0 or higher  

Main Scene: MobileRunnerPrototype  



To build the project:



1\. Open the project in Unity 6.

2\. Go to File > Build Settings.

3\. Select Android.

4\. Make sure the MobileRunnerPrototype scene is added to Scenes In Build.

5\. Make sure MobileRunnerPrototype is the first scene in the build list.

6\. Click Build or Build And Run.

7\. Save the APK to a folder on the computer.

8\. Install or run the APK on an Android phone or tablet.



\## Device Testing Notes



The prototype was tested using the Unity Device Simulator for screen layout and UI readability. The project is intended to be tested on an Android tablet for real touch and accelerometer input. The biggest difference between editor testing and device testing is that swipe input can be tested with a mouse in the editor, but accelerometer input needs a physical device to judge how it actually feels.



\## Known Issues and Limitations



\- The game currently uses placeholder square sprites instead of final artwork.

\- The prototype does not have a score system yet.

\- The prototype does not have a start menu or restart screen yet.

\- Obstacles currently spawn randomly in lanes.

\- A future improvement would be making obstacles react to the player’s current lane to create more intentional challenge.

\- The accelerometer currently affects player lean instead of full movement to keep the prototype simple and readable.



\## Scripts



\- PlayerController.cs handles swipe input, lane movement, jumping, accelerometer lean, and player feedback.

\- ObstacleSpawner.cs spawns obstacles in random lanes.

\- ObstacleMovement.cs moves obstacles downward and destroys them after they leave the screen.

\- GameUIManager.cs handles instruction text and temporary status messages.

\- FPSCounter.cs displays the current FPS.

