A 3D game in Unity where a bunny tries to escape from a maze, collecting coins and avoiding hyenas.

#Description

The player controls a bunny in a maze. The goal is to collect as many coins as possible without getting caught by the hyenas that patrol and track the player using an A* algorithm.

During the game:
- The hyenas track the player using an A* algorithm.
- If the player collides with the hyenas three times, the game restarts.

#Features

- Player control in 3D space
- Enemies with A* pathfinding
- Coin collection system
- UI with score 

#Technologies used

- Engine: Unity
- Language: C# (MonoBehaviour)
- UI: Unity Canvas System

#PROJECT STRUCTURE
Assets/
├── Scripts/
│ ├── PlayerController.cs
│ ├── HyenaController.cs
│ ├── AStar.cs
│ ├── MazeGenerator.cs
│ ├── PlayerHealth.cs
│ ├── CoinManager.cs
│ └── CameraShake.cs
├── Prefabs/
│ ├── Bunny.prefab
│ └── Hyena.prefab
├── UI/
│ ├── CoinText
