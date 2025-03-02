#Unity Racing Game

## Overview
This is a Unity-based racing game where players navigate through a procedurally generated road while avoiding obstacle cars. The game features a simple yet engaging gameplay loop with car spawning mechanics, player controls, and score tracking.

## Project Structure

### Core Components
- **GameManager**: Controls the game state (waiting, playing, paused, failed) and handles game events
- **Player**: Manages player movement, collision detection, and player state
- **Road System**: Procedurally generates the road environment as the player progresses
- **Car System**: Spawns obstacle cars that the player must avoid

### Key Features
- Procedurally generated endless road
- Dynamic obstacle car spawning
- Player movement and control system
- Game state management with events
- Object pooling for performance optimization

## Getting Started


### Installation
1. Clone this repository
2. Open the project in Unity
3. Open the MainScene located in Assets/Scenes
4. Press Play to test the game

## Development

### Project Organization
- **Assets/Scripts/Core**: Core game systems and managers
- **Assets/Scripts/Player**: Player-related scripts for movement and control
- **Assets/Scripts/Cars**: Vehicle spawning and behavior
- **Assets/Scripts/Road**: Road generation and management
- **Assets/Scripts/UI**: User interface elements
- **Assets/Scripts/Camera**: Camera control and following behavior
- **Assets/ScriptableObjects**: Data containers for game configuration

### Key Scripts
- **GameManager.cs**: Controls game flow and state
- **PlayerController.cs**: Handles player input and movement
- **RoadSpawner.cs**: Manages road generation
- **CarFactory.cs**: Handles spawning and recycling of obstacle cars


## Acknowledgments
- Unity Asset Store for any third-party assets used in the project 