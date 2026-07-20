\# Mobile Runner Prototype



\## Project Overview



Mobile Runner Prototype is a 2D endless runner created in Unity for Android devices. The project includes touch controls, accelerometer input, obstacle spawning, a responsive main menu, a settings panel, and an in-game user interface.



\## UI Screens



The project includes the following interface screens:



\- Main Menu

\- Settings Panel

\- In-Game HUD



The interface was tested across multiple mobile aspect ratios to confirm that the layout remains readable and usable.



\## Canvas Configuration



\- UI Scale Mode: Scale With Screen Size

\- Reference Resolution: 1080 x 1920

\- Screen Match Mode: Match Width Or Height

\- Match Value: 0.5



These settings allow the user interface to scale across phones and tablets while maintaining a consistent portrait layout.



\## Anchoring Decisions



The main menu buttons are centered so they remain balanced across different screen sizes. The game interface elements are anchored near the edges of the screen based on their purpose. The FPS counter is positioned near the top of the display, while gameplay instructions and status information remain visible without covering the player.



\## Touch Target Design



The menu buttons were designed with large touch areas and spacing between each option. This makes the controls easier to select on mobile devices and reduces accidental input.



\## Accessibility Features



The settings panel includes options that improve readability and usability.



\### Text Scaling



The text scaling option allows the user to adjust the size of interface text.



\### High Contrast Mode



The high contrast option improves the visibility of interface elements by increasing the difference between text and background colors.



\## Responsive Design Testing



The user interface was tested at the following aspect ratios:



\- Standard phone: 16:9

\- Modern tall phone: 19.5:9

\- Tablet: 4:3



The Canvas Scaler and anchor settings helped keep the interface positioned correctly across each display size.



\## Challenges and Solutions



One challenge was keeping the menu and interface elements aligned across different aspect ratios. This was addressed by using anchors, centered layouts, and the Canvas Scaler.



Another challenge was making sure the settings panel could open and close without overlapping the main menu. Separate panels and a UI control script were used to switch between the menu states.



\## Known Issues



\- Some interface visuals still use placeholder artwork.

\- Additional spacing adjustments may be needed for unusual device sizes.

\- The project may require further testing on additional Android devices.



\## Controls



\- Swipe left or right to change lanes.

\- Swipe up to jump.

\- Tilt the device to activate the accelerometer-based movement effect.

\- Use the on-screen menu buttons to start the game and open settings.



\## Unity Version



Created using Unity 6.



\## Platform



Android

