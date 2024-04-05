# Project README

## Controls and Mechanics

This project uses a variety of player movement settings and mechanics for navigating the game world.

### Further Information  
During this project, I ventured into a 2D environment for the first time. As I am more accustomed to 3D, it presented a learning curve. Despite the challenges, 
I found the project enjoyable and it sparked my interest in further pursuing this type of thing in the future.
While I didn't manage to implement every aspect I initially planned, I am satisfied with the progress I made and the knowledge acquired throughout this journey.
This project also reminded me of the energetic atmosphere of a game jam. It's been a while since I was required to construct a project under such a tight schedule. 
I believe this project provides a good showcase of my coding style. However, it's important to note that under normal circumstances, 
I dedicate more time to the architecture and design phases before diving into coding.


### Navigating the project 
- Framework/ - The root directory for the project 
- Framework/Game/ - Content that is specific to the game / design 
- Framework/Global/ - Configurations and settings that are global to the project 
- Framework/Thirdparty/ - External libraries and assets used in the project 
- Framework/Scripts/Common/ - Common scripts that are carried over between projects that i've worked on (With the exception of AwaitExtensions module, which is a custom extension for using async as an enumerator) 
- Framework/Scripts/Core/ - Core scripts that are used in the project.
- Framework/Scripts/Editor/ - Editor scripts that are used in the project. 

### Basic Controls 
- `WASD` keys for movement 
- `Space` key for jumping 
- `Left Mouse Click` - Attack
- `Right Mouse Click` - Dash 

### What I implemented 
- Vanishing platform with shader 
- AI enemies with different interchangable attack strategies that use object pool factory pattern 
- Basic Movement
- Airborne Movement (including control while jumping and falling)
- Fast Fall Mechanics (Allowing a controlled fast descent)
- Jump Mechanics (Including variable jump height)
- Wall Jump and Wall Slide Mechanics
- Dash Mechanics
- ~~Climbing Mechanics~~
- Ground and Wall Checking using Raycast Checks

### What didn't make it 
- Didn't have time to completely implement the boss fight, the animations for the boss were broken and i didn't have time to fix them 
- I had the intention of adding climbing mechanics to the game / swinging mechanics 
- I had the intention of adding a bunch of different weapons to the game 
- 

### What broke at the last minute 
- The axe throw strategy seems to be broken
- A bunch of the animations that i had configured for the enemy and boss got corrupted and i had not had the time to fix them 

### Movement Settings

- **Maximum Walking Speed:** Defines the top speed the player can reach while walking.
- **Maximum Running Speed:** Defines the top speed player can reach while running.
- **Running Acceleration:** The rate at which player gains speed when starting to run.
- **Running Deceleration:** The rate at which player loses speed when stopping their run.

### Falling Settings

Control how the player falls:
- **Normal Fall Gravity Multiplier:** The acceleration due to gravity when the player is falling normally.
- **Maximum Normal Fall Speed:** The maximum speed the player can reach during a normal fall.
- **Quick Fall Gravity Multiplier:** The acceleration due to gravity when the player is falling quickly.
- **Maximum Quick Fall Speed:** The maximum speed the player can reach during a quick fall.

### Jump Settings

- **Jump Peak Height, Jump Rise Duration:** These two settings control the arc and duration of the player's jump.
- **Mid Air Hang Time Limit:** If the player is in the air for this amount of time without jumping, they will begin to fall.

### WallJump

- **WallJump Force, WallJumpTime, WallJumpMovementSmoothingFactor:** These settings control the force and time of a wall jump and the smoothing factor of the wall jump movement.

### Wall Slide Settings

- **MaxWallSlideSpeed, WallSlideAcceleration:** These settings control the maximum speed and acceleration for sliding down a wall.

### Dash Settings

Dashing lets the player move quickly in a direction for a short amount of time.
- **DashMaxSpeed, DashSleepTime, DashEngageDuration, DashEndTime:** These settings control speed, sleep duration, engage time and end time of the dash.
- **MaximumNumberOfDashes:** The maximum number of consecutive dashes the player can execute.

### Climbing

- **ClimbingSpeed, ClimbGrabbingTime:** These settings control the speed of climbing and the time it takes to grab onto a climbable surface.

### Raycast Checks

Raycasting is used for several checks such as the distance of the ground and wall from the player, whether the player is touching the ground or the wall, etc.
- **GroundCheckScale, WallCheckScale:** These settings control the scale for ground and wall check.

### Tilting

- **MaxTilt, TiltSpeed:** These settings control the maximum angle for tilt and the speed of tilting.
