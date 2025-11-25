# GEMINI Project Analysis: Asteroids Game

This document provides a comprehensive overview of the "Asteroids" project, intended to guide future development and analysis.

## 1. Project Overview

This is a classic Asteroids arcade game clone developed using the **Godot Engine (v4.4)** with **C# (.NET 8)**.

*   **Core Gameplay:** Players control a spaceship that can thrust, rotate, and shoot projectiles to destroy incoming asteroids. The game ends when the player's ship is destroyed.
*   **Technology:** The project is built entirely within the Godot ecosystem. It leverages C# for scripting game logic.
*   **Architecture:** The game uses a standard Godot node-based architecture. A central `Main.cs` script acts as the main game controller, managing game state, and the spawning of objects. A globally-loaded singleton `Global.cs` stores shared configuration data like speeds, scales, and colors, which can be easily modified from the Godot editor.
*   **Graphics:** The game features a retro vector-style aesthetic, with all game objects (ship, asteroids, projectiles) rendered using `Line2D` nodes rather than sprites.

## 2. Key Files and Components

*   **`Main.tscn` / `Main.cs`:** The entry point and heart of the game. It manages the game loop, tracks the score, spawns asteroids and the player, and handles the `GameOver` state.
*   **`PlayerShipSleek1.tscn` / `PlayerShip.cs`:** Defines the player's ship. `PlayerShip.cs` handles player input for movement (thrust, rotation) and shooting. It is a `RigidBody2D`, so its movement is physics-based.
*   **`Asteroid_Large.tscn`, `Asteroid_Medium.tscn`, `Asteroid_Small.tscn`:** These are scene "templates" for the different asteroid sizes.
*   **`Asteroid.cs`:** Contains the logic for all asteroids. It handles their movement, collision detection, and destruction. When an asteroid is hit, it may show "crack" visuals or be destroyed, increasing the player's score. New asteroids are spawned by duplicating and customizing the template scenes.
*   **`Global.cs`:** A globally accessible singleton (autoloaded by Godot) that holds all major configuration parameters for the game, such as player speed, asteroid speeds, object scales, and line colors. This file is critical for game balancing.
*   **`project.godot`:** The main Godot project file. It defines project settings, scenes, and autoload scripts like `Global.cs`.

## 3. Building and Running

### Running the Project from the shell

1. **Execute the Godot Editot:** Run the 'godot' command in the projects root.

### Running the Project in the ide

1.  **Open the Godot Editor:** This project must be opened with Godot Engine version 4.4 or newer.
2.  **Run the Game:** Press the "Play" button (▶️) in the top-right of the editor. The main scene is configured to be `Main.tscn`.

### Building the Project

This is a standard Godot C# project. There are no custom build scripts.

1.  Navigate to **Project -> Export...** in the Godot editor.
2.  Add and configure an export preset for your desired platform (e.g., Windows Desktop, Linux/X11, macOS).
3.  Click **Export Project** to generate the executable.

## 4. Development Conventions

*   **Object Instantiation:** Game objects like asteroids and projectiles are not instantiated directly. Instead, template scenes (`.tscn` files) are loaded, and their nodes are duplicated and added to the scene tree.
*   **Physics:** All moving objects (`PlayerShip`, `Asteroid`) are `RigidBody2D` nodes, meaning their movement is controlled by the 2D physics engine (applying forces, torque, and impulses).
*   **Screen Wrapping:** A static utility function `Main.ScreenWrap(Node2D node)` is used to ensure that objects that fly off one side of the screen reappear on the opposite side.
*   **Configuration:** To adjust game balance (e.g., "make the ship faster" or "make asteroids bigger"), modify the `[Export]` properties within `Global.cs` using the Godot editor's Inspector panel.
