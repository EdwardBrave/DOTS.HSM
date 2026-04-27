# EB.DOTS.HSM
![GitHub package.json version](https://img.shields.io/github/package-json/v/EdwardBrave/DOTS.HSM)
![Unity 2022.3+](https://img.shields.io/badge/unity-2022.3+-blue.svg)
![DOTS](https://img.shields.io/badge/DOTS-Entities-orange.svg)
![GitHub License](https://img.shields.io/github/license/EdwardBrave/DOTS.HSM)

A lightweight, performance-oriented Hierarchical State Machine (HSM) designed specifically for Unity DOTS (ECS). It allows for complex game logic management while maintaining high performance and clean system isolation by automatically managing the enabled state of ECS systems.

## Key Features
*   **Hierarchical Logic**: Support for nested states and sub-states using a clean, type-safe API.
*   **ECS Integration**: Driven by `HsmPreUpdateSystem` and `HsmLateUpdateSystem`, fitting perfectly into the ECS update loop.
*   **Automatic System Management**: The "Killer Feature"—automatically enable or disable specific ECS Systems based on the active state using `RequiredSystems`.
*   **Editor Friendly**: Includes an `HsmBoot` component with a custom inspector for easy initialization and state selection from the Unity Scene.
*   **Scene Transitions**: Built-in `SceneTransitionState` for handling asynchronous scene loading between states.
*   **Zero-Boilerplate Transitions**: simple `SetState` and `SetSubState` methods for fluid state changes.

## Installation
1. Open the Unity Package Manager (`Window > Package Manager`).
2. Click the `+` icon and select `Add package from git URL...`.
3. Enter the repository URL: `https://github.com/edwardbrave/com.edwardbrave.dots.hsm.git` (or your local path).

## Core Concepts
The package is built on three main pillars:
*   **HsmRoot**: The entry point and singleton component (`IComponentData`) that holds the state machine in the ECS World.
*   **BaseState<TSelf>**: The base class for defining states. It handles sub-state management and `RequiredSystems`.
*   **BaseSubState<TSelf, TParent>**: Used for creating states that belong to a parent state, allowing for deep hierarchies.

## Getting Started

### 1. Define Your States
Define your states by inheriting from `BaseSubState`. Use `RequiredSystems` to specify which ECS systems should run only when this state is active.

```csharp
using EB.DOTS.HSM;
using System;
using System.Collections.Generic;
using Unity.Entities;

public class GameplayState : BaseSubState<GameplayState, HsmRoot>
{
    // These systems will be Enabled when entering GameplayState 
    // and Disabled when exiting it.
    protected override List<Type> RequiredSystems => new() 
    { 
        typeof(PlayerMovementSystem),
        typeof(EnemyAiSystem)
    };

    protected override void OnUpdate(SystemBase system)
    {
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Escape))
        {
            // Transition to another state under the same parent (HsmRoot)
            SetState(new PauseMenuState());
        }
    }
}
```

### 2. Initialize the HSM
You can initialize the HSM in two ways:

#### Via Editor (Recommended)
Add the `HsmBoot` component to a GameObject in your scene. Use the "Initial State" dropdown to select your starting state. This will automatically call `HsmTools.InitHsm` when the scene starts.

#### Via Code
```csharp
using EB.DOTS.HSM;

// Initialize with a starting state
HsmTools.InitHsm(new GameplayState());
```

## Advanced Usage

### Nested States
You can create sub-states of any state, not just `HsmRoot`.

```csharp
public class CombatState : BaseSubState<CombatState, GameplayState>
{
    protected override void OnEnter(SystemBase system)
    {
        UnityEngine.Debug.Log("Entered Combat!");
    }
}
```

### Scene Transitions
Use the built-in `SceneTransitionState` to handle loading screens.

```csharp
// Transition to "Level1" scene, then enter GameplayState
SetState(new SceneTransitionState("Level1", new GameplayState()));
```

## Technical Details
*   **Update Order**: HSM updates during `InitializationSystemGroup` (Pre-Update) and `LateSimulationSystemGroup` (Late-Update/Transitions).
*   **Requirements**: Requires `com.unity.entities` version 1.0.0 or higher.
*   **Compatibility**: Tested with Unity 2022.3+ and Unity 6.

## License
Developed by [Edward Brave](https://www.youtube.com/@EdwardBrave).
License: MIT.
