# Production-Grade Systems Framework (Prototype for Project ALF)

---

## ⚠️ NDA COMPLIANCE NOTICE

**This repository contains proprietary intellectual property under strict Non-Disclosure Agreement.** All gameplay-specific terminology has been abstracted into standardized enterprise engineering terminology to protect core game mechanics while showcasing architectural implementation patterns.

---

[![Portfolio](https://img.shields.io/badge/Portfolio-Live_Demo-00C5FF?style=for-the-badge&logo=react&logoColor=white)](https://roadslayer0823.github.io/2026-portfolio-website/)

---

## 🌐 Live Demonstration

This system has been fully integrated and deployed into my centralized production portfolio. You can interact with the live build, review architecture nodes, and explore interactive modules directly via the link below:

🔗 **[Access the Interactive Live Demo Here](https://roadslayer0823.github.io/2026-portfolio-website/)**

---

## Section 1: Core Engineering & Architectural Achievements

### BattleDistanceManager.cs
**Architectural Pattern: State Machine with Encapsulated Business Logic**

- **State Management**: Implements enum-based state machine (DistanceType) for runtime spatial relationship tracking between entities
- **Separation of Concerns**: Decouples distance calculation logic from UI rendering through dedicated BattleDistancePanel reference
- **Data-Driven Design**: Uses out parameters (out List<string>) for logging system integration without side effects
- **Performance Optimization**: State transitions are O(1) operations using switch statements, avoiding nested conditionals
- **Extensibility**: Clean interface with getter/setter methods enables future state persistence and serialization

### PlayerDashboard.cs
**Architectural Pattern: Event-Driven UI with Dependency Injection**

- **Callback Pattern**: Leverages C# Action delegates for loose coupling between UI events and business logic (onExecuteButtonClickedCallback)
- **Dependency Injection**: Initialize method pattern enables testable component composition and runtime dependency resolution
- **Resource Management**: Uses const string literals for audio/animation asset references, reducing runtime string allocation overhead
- **State Encapsulation**: Private callback field prevents external manipulation of event handlers
- **Interface Segregation**: Exposes only necessary methods (GetAtlSlotListPanelV3, GetCharacterInfoPanelV2) following Interface Segregation Principle

### SkillInfoPanel.cs
**Architectural Pattern: Data-Driven UI with Conditional Rendering**

- **Modern C# Features**: Utilizes switch expressions (C# 8.0) for concise conditional logic in UI state management
- **Component-Based Architecture**: Heavily leverages Unity's [SerializeField] attribute for editor-driven dependency injection
- **Dynamic UI Generation**: Conditionally activates/deactivates UI elements based on runtime data payload (skill attributes)
- **Shader Material Manipulation**: Direct material property updates for performance-critical visual effects
- **State Pattern**: Multiple UI states (ActiveSkillInfoPanel, BackendSkillInfoPanel) with dedicated transition methods

### BattleResultPanelV2.cs
**Architectural Pattern: Singleton Integration with Scene Management**

- **Scene Lifecycle Management**: Integrates with Unity SceneManager for clean scene transitions and resource cleanup
- **Post-Processing Pipeline**: Leverages visual effect manager for runtime shader effects (blur) demonstrating graphics pipeline integration
- **Audio System Integration**: Singleton pattern usage (AudioManager.Instance) for centralized audio state management
- **State Isolation**: Separate methods for victory/defeat states prevent state contamination
- **Resource Cleanup**: Explicit audio cleanup (StopBackgroundMusic) prevents memory leaks in scene transitions

### PassiveSkillCategorySelectionPanel.cs
**Architectural Pattern: Gesture-Based Input with Custom Hit Detection**

- **Custom Input Processing**: Implements vector-based gesture recognition using angle calculations for directional input
- **Polygon Hit Testing**: Integrates with PolygonChecker for non-rectangular UI hit areas, demonstrating advanced collision detection
- **Animation Choreography**: Complex LeanTween callback chains for sequenced visual effects
- **State Machine**: Multiple state tracking (currentPassiveSkillType, highlightedPassiveSkillType, isPointerDown) for robust input handling
- **Performance Optimization**: Early return patterns in gesture detection to minimize unnecessary calculations

### ActiveSkillSlotListPanelV2.cs
**Architectural Pattern: Complex Animation System with Circular Buffer**

- **Circular Data Structure**: Implements wheel-based UI with modular arithmetic for index wrapping
- **Animation State Management**: Boolean flag (isAnimationRunning) prevents animation queue buildup and race conditions
- **Recursive UI Operations**: SetActiveRecursively method for hierarchical GameObject state management
- **Performance Optimization**: LeanTween.cancel() prevents memory leaks from orphaned animations
- **Event-Driven Architecture**: Action<SkillSlotV2,bool> callback pattern for decoupled slot selection events

### BackendSkillSlotListPanel.cs
**Architectural Pattern: Strategy Pattern with Type-Based Allocation**

- **Strategy Pattern**: Switch-based skill slot allocation based on BackendSkillType enum demonstrates strategy selection
- **Pattern Matching**: Advanced C# pattern matching with property patterns for concise conditional logic
- **Dynamic State Management**: Runtime skill slot state updates based on game character state
- **Collection Management**: Efficient skill slot lookup with O(n) search optimization for small datasets
- **Conditional Rendering**: Dynamic QTE button visibility based on runtime game state

### EnemyCharacterInfoBox.cs
**Architectural Pattern: Observer Pattern with Real-Time Data Binding**

- **Observer Pattern**: Event-driven UI updates via callback registration (AddOnInitializedCallback, AddOnCharacterInfoUpdatedCallback)
- **Real-Time Position Tracking**: Update() method for frame-by-frame position synchronization with game entities
- **Shader-Level Optimization**: Direct material.SetFloat calls bypass GameObject overhead for performance-critical updates
- **Animation Chaining**: Complex LeanTween callback sequences for multi-stage visual effects
- **State-Based Rendering**: Conditional UI element activation based on entity state (break status)

---

## Section 2: Script Dependency Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                    BattleGameManager (Core)                      │
│  - Central orchestration layer                                  │
│  - Scene lifecycle management                                    │
└────────────┬────────────────────────────────────────────────────┘
             │
             ├──► BattleDistanceManager
             │     ├──► BattleDistancePanel (UI)
             │     └──► BattleAnimationManager
             │
             ├──► BattleUiManager (UI Coordinator)
             │     ├──► PlayerDashboard
             │     │     ├──► ATLSlotListPanelV3
             │     │     └──► CharacterInfoPanelV2
             │     │
             │     ├──► SkillInfoPanel
             │     │     └──► SpecialSkillInfoBox
             │     │
             │     ├──► ActiveSkillSlotListPanelV2
             │     │     └──► SkillSlotV2[]
             │     │
             │     ├──► BackendSkillSlotListPanel
             │     │     ├──► SkillSlotV2[]
             │     │     └──► QTE Skill Slot
             │     │
             │     ├──► PassiveSkillCategorySelectionPanel
             │     │     └──► PassiveSkillSlot[]
             │     │
             │     ├──► EnemyCharacterInfoBox
             │     │     ├──► EnemyCharacterInfoBox_UI
             │     │     └──► EnemyCharacterInfoBox_UI_V2
             │     │
             │     └──► BattleResultPanelV2
             │           └──► BattleVisualEffectManager
             │
             └──► SkillSelectionPanelV2
                   ├──► SkillSelectionBoxV2[]
                   └──► PreparationSection

┌─────────────────────────────────────────────────────────────────┐
│              Supporting Systems (Cross-Cutting)                  │
│  - AudioManager (Singleton)                                      │
│  - BattleLogicManagerV2 (Business Logic)                         │
│  - DatabaseManager (Data Access)                                 │
│  - TerminologyManager (Localization)                             │
└─────────────────────────────────────────────────────────────────┘
```

**Architectural Principles Demonstrated:**
- **Layered Architecture**: Clear separation between core logic, UI coordination, and presentation layers
- **Dependency Inversion**: High-level modules depend on abstractions (callbacks, interfaces) rather than concrete implementations
- **Single Responsibility**: Each component has a focused, well-defined purpose
- **Open/Closed Principle**: Extensible through callback registration without modifying core logic

---

## Section 3: Professional Growth Summary

### Technical Competencies Demonstrated

**Software Architecture:**
- Implemented event-driven UI patterns using C# delegates and Actions
- Applied state machine patterns for complex game state management
- Demonstrated dependency injection patterns for testable, maintainable code
- Utilized modern C# features (switch expressions, pattern matching) for clean, concise code

**Performance Optimization:**
- Shader-level material manipulation for performance-critical rendering
- Animation state management to prevent memory leaks (LeanTween.cancel)
- O(1) state transitions using enum-based switch statements
- Efficient collection management with appropriate data structure selection

**UI/UX Engineering:**
- Custom gesture recognition systems with vector mathematics
- Complex animation choreography with callback chains
- Real-time data binding using observer pattern
- Dynamic UI generation based on runtime data payloads

**System Integration:**
- Singleton pattern implementation for centralized service management
- Scene lifecycle management with proper resource cleanup
- Audio system integration with centralized state management
- Post-processing pipeline integration for visual effects

### Engineering Philosophy

- **Decoupling**: Consistent use of callback patterns to minimize component dependencies
- **Encapsulation**: Private fields with controlled public interfaces
- **Performance Awareness**: Shader-level optimizations and animation state management
- **Maintainability**: Clear naming conventions, const literals, and separation of concerns
- **Modern Practices**: Leveraging latest C# language features for cleaner code

---
## 📂 Authored Subsystems Summary
For cross-referencing during technical review, the specific concrete implementations entirely written by me within this repository include:
* `BattleDistanceManager.cs` (Spatial State Architecture)
* `PlayerDashboard.cs` & `EnemyCharacterInfoBox.cs` (Reactive View Layers)
* `SkillInfoPanel.cs` (Dynamic Conditional Layout Parser)
* `BattleResultPanelV2.cs` (Lifecycle Integration)
* `PassiveSkillCategorySelectionPanel.cs` (Vector Gesture Parsing)
* `ActiveSkillSlotListPanelV2.cs` & `BackendSkillSlotListPanel.cs` (Dynamic Matrix Layouts)
