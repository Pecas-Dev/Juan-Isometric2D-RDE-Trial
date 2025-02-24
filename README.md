# Isometric2D RDE Trial

 <br>

**Unity Developer Internship Trial - Runic Dices Entertainment**

 <br>
 
-----------------------------------------------------------------

## Overview

This repository contains the **Unity Developer Internship Trial** for **Runic Dices Entertainment**. The tasks in this trial assess my skills in Unity development, GitHub workflow, AI state machines, and core gameplay systems.

I aim to complete all tasks within the **5-day timeframe** while maintaining clean code, best practices, and efficient workflow management.

 -----------------------------------------------------------------
 
 <br>


![image](https://github.com/user-attachments/assets/1c618f86-055c-43e4-a337-e773b3154573)
_Project in Finalized State (Not at Runtime)_



 <br>
 
  -----------------------------------------------------------------

 <br>

## Controls

<br>

![controllerLayout](https://github.com/user-attachments/assets/49dc673f-fd23-4481-91b8-58f07c3d3d7d)

<br>

![keyboardLayout](https://github.com/user-attachments/assets/c82910a3-bac9-4626-98a9-51a74d9386c4)



### **Player Controls**
- **Move Player**: Left Joystick (Gamepad) or **WASD / Arrow Keys** (Keyboard).
- **Melee Attack (Slash)**: Left Click (Mouse) or Right Trigger **(R2 PlayStation / RT Xbox)**.
- **Projectile Attack**: Right Click (Mouse) or Left Shoulder **(L1 PlayStation / LB Xbox)**.

### **UI Controls**
- **Navigate Inventory**: D-Pad **(Up/Down) Gamepad** or **Mouse Pointer**.
- **Select/Enter Inventory Slot**: **Enter Key (Keyboard)** or Button South **(X PlayStation / A Xbox)**.
- **Toggle Inventory**: **"I" Key (Keyboard)** or Button North **(Triangle PlayStation / Y Xbox)**.

 <br>

 -----------------------------------------------------------------

 <br>

## Trial Tasks

### Task 1: Unity Project Setup & GitHub Integration [ (DevLog 1) ](<https://github.com/Pecas-Dev/Juan-Isometric2D-RDE-Trial/blob/main/Documents/Dev%20Logs/Task%201/Task%201%20DevLog%20(UNITY%20DEVELOPER%20INTERNSHIP%20TRIAL).pdf>)

**Goal**: Set up a Unity project with version control in GitHub.

- Initialize a Unity 2D URP project.
- Set up Git and a proper .gitignore file.
- Create a GitHub repository and push the project.
- Follow Git best practices with clear commit messages

<br>

### Task 2: Isometric Player Movement & Input System [ (DevLog 2) ](<https://github.com/Pecas-Dev/Juan-Isometric2D-RDE-Trial/blob/main/Documents/Dev%20Logs/Task%202/Task%202%20DevLog%20(UNITY%20DEVELOPER%20INTERNSHIP%20TRIAL).pdf>)

**Goal**: Implement isometric movement using Unity’s New Input System.

- Set up player movement using WASD or arrow keys.
- Use a dynamic Rigidbody2D component.
- Implement isometric camera view and smooth following.
- Ensure a configurable max speed & acceleration.

 <br>

### Task 3: AI State Machine for Enemy Behavior [ (DevLog 3 - 4.1) ](<https://github.com/Pecas-Dev/Juan-Isometric2D-RDE-Trial/blob/main/Documents/Dev%20Logs/Task%203-4/Task%203-4.1%20DevLog%20(UNITY%20DEVELOPER%20INTERNSHIP%20TRIAL).pdf>)

**Goal**: Implement an AI system using a state machine for enemy behavior.

- Create Idle, Patrol, Chase, and Attack states.
- Detect player position and switch states dynamically.
- Use colliders or raycasts for detection.
- Implement patrolling AI with dynamic patrol routes.

 <br>

### Task 4.1 - 4.2: Implement a Basic Combat System & Implement a Simple Inventory System [ (DevLog 3(Pathfinding) - 4.1- 4.2) ](<https://github.com/Pecas-Dev/Juan-Isometric2D-RDE-Trial/blob/main/Documents/Dev%20Logs/Task%203-4.1-4.2/Task%203(Pathfinding)-4.1-4.2%20DevLog%20(UNITY%20DEVELOPER%20INTERNSHIP%20TRIAL).pdf>)

**Goal 4.1**: Create a functional combat system with player and enemy interactions.

- Implement player attacks (projectile/melee) triggered by spacebar/mouse click
- Create projectile or melee mechanics with proper damage system
- Add health systems for both player and enemy
- Implement enemy attack ability with cooldown
- Create UI elements for health display
- Add visual feedback for combat interactions

 <br>

**Goal 4.2**: Create a functional inventory system with item management.

- Implement item collection and storage system
- Create UI-based inventory display with dynamic slots
- Add item pickup functionality with 'E' key
- Implement item usage system (e.g., consuming health potions)
- Create real-time UI updates for inventory changes

 <br>
 
-----------------------------------------------------------------

## Scripts Directory Structure

```glsl
Scripts
├── Characters
│   ├── Player           // Player character scripts (movement, actions, etc.).
│   │    ├── Animation
│   │    ├── Combat
│   │    ├── StateMachine
│   │
│   ├── Enemy            // Enemy behavior and AI scripts.
│        ├── Ability
│        ├── Animation
│        ├── StateMachine
│
├── Inventory            // Inventory system management, item handling, and UI logic
│
├── Objects              // Core game object implementations like items, projectiles, and health
│   ├── Health
│   ├── Items
│   ├── Projectiles
│
├── ScriptableObjects    // Scriptable Object logic, the SOs will be stored in another folder.
│   ├── GrassZone
│   ├── Health
│   ├── Input
│
├── UI                   // User interface components, HUD elements, and menu systems
│
├── Utility              // Shared utilities, helper functions, and core system implementations
    ├── StateMachine
    ├── GrassZone
    ├── Health
    ├── Pathfinding
    ├── VFX


/*Last Updated: February 23rd, 2025*/
```

---

 <br>
 
## Project Setup

- **Project name**: `Isometric2DGame`
- **Unity Version**: `6000.0.24f1 (2D URP)`
- **Repository URL**: `https://github.com/Pecas-Dev/Juan-Isometric2D-RDE-Trial.git`

 <br>

## Credits

This project was created by _**Juan F. Gutierrez**_.
