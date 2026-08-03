\# Mobile Runner Prototype



\## Project Overview



Mobile Runner Prototype is a 2D endless runner developed in Unity 6 as part of a Mobile Game Development course. Throughout the semester, the project has gradually evolved from a simple touch input demonstration into a mobile prototype featuring swipe controls, sensor integration, responsive user interfaces, performance optimization, and Unity Analytics.



The project is being developed following professional game development practices, including GitHub version control, documented optimization, and organized project structure.



\---



\# Unity Version



Unity 6



\---



\# Platform



Android



\---



\# Project Features



\- Three-lane endless runner gameplay

\- Swipe left and right lane switching

\- Swipe up to jump

\- Accelerometer tilt feedback

\- Obstacle spawning

\- Object pooling

\- Responsive UI

\- Main Menu

\- Settings Panel

\- In-game HUD

\- Performance optimization

\- Unity Analytics



\---



\# Controls



| Action | Input |

|---------|-------|

| Move Left | Swipe Left |

| Move Right | Swipe Right |

| Jump | Swipe Up |

| Open Settings | Settings Button |

| Start Game | Play Button |



\---



\# Week 1 - Mobile Setup



\## Completed



\- Configured Unity for Android development.

\- Implemented touch input detection.

\- Tested touch input using both the Unity Editor and a physical Android tablet.

\- Connected the project to GitHub.



\---



\# Week 2 - Prototype Development



\## Gameplay Features



\- Three lane movement

\- Swipe controls

\- Jump mechanic

\- Accelerometer integration

\- Obstacle spawning

\- Player feedback messages

\- Mobile testing



\---



\# Week 3 - Mobile UI System



\## UI Features



Implemented a responsive UI system consisting of:



\- Main Menu

\- Settings Panel

\- Gameplay HUD



\### Responsive Design



The interface was tested at:



\- 16:9

\- 19.5:9

\- 4:3



Canvas Scaler was configured using:



\- Scale With Screen Size

\- Reference Resolution: 1080 x 1920

\- Match Value: 0.5



\### Accessibility



Implemented:



\- Text Scaling

\- High Contrast Mode



\---



\# Week 4 - Optimization



\## Optimizations



\### Object Pooling



Replaced repeated Instantiate() and Destroy() calls with reusable pooled obstacles.



\### Centralized Obstacle Processing



Reduced unnecessary Update() calls by simplifying obstacle behavior.



\### Player Controller Improvements



Optimized player processing by:



\- Caching references

\- Reducing unnecessary Update work

\- Replacing Invoke() with timer-based feedback



\---



\## Performance Comparison



| Metric | Before | After |

|---------|--------|-------|

| FPS | 17.4 | 43.3 |

| Frame Time | 57.4 ms | 23.1 ms |

| Batches | 7 | 6 |

| SetPass | 6 | 4 |



\---



\# Week 5 - Unity Analytics



\## Analytics Events



Implemented Unity Analytics to record:



\- game\_started

\- settings\_opened

\- lane\_changed

\- player\_jumped

\- obstacle\_hit



\## Key Performance Indicators



The analytics implementation measures:



\- Gameplay sessions

\- Lane changes

\- Jump frequency

\- Obstacle collisions

\- Settings usage



These metrics help evaluate player behavior and improve future gameplay balancing.



\---



\# Device Testing



The project has been tested using:



\- Unity Editor

\- Android Tablet



Testing included:



\- Swipe controls

\- Accelerometer

\- UI interaction

\- Analytics events

\- Performance profiling



\---



\# Known Issues



\- Placeholder art is still being used.

\- Additional balancing is needed for obstacle difficulty.

\- Analytics dashboard updates may be delayed.

\- A complete Game Over system has not yet been implemented.



\---



\# Future Improvements



Planned improvements include:



\- Score system

\- High score saving

\- Coins

\- Better graphics

\- Audio

\- Animations

\- Increasing game speed

\- Rewarded advertisements

\- Leaderboards

\- Game Over screen



\---



\# GitHub Repository



https://github.com/r3dd89/MobileGamePrototype.git

