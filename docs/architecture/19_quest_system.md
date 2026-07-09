# Architectural Specification: Quest System

* **Status**: APPROVED
* **Date**: 2026-07-09
* **Engine Focus**: Unity 6 LTS

---

## 1. Design Intent & Requirements Traceability

The Quest System manages story progression, objective tracking, and reward payouts:

* **Pace & Biome Progress (Vision §3 & §11 & GDD §5.1)**: Support for four distinct quest types: Main Quests (driving the narrative acts), Side Quests (narrative character subplots), Daily Quests (rotated activities to encourage positive return loops without streaks), and Community Quests (async cooperative world-building milestones).
* **Mastery Engine Integration (Vision §3 & GDD §10.2 & §16.2)**: Puzzle configurations are not hardcoded. The Quest System must query the hidden **Mastery Engine** to dynamically adjust the difficulty parameters (e.g., fraction values, blend levels) of the quest's active puzzle nodes based on the child's demonstrated competence.
* **Offline-First Playability (Vision §5 & GDD §1.2 & §5.3)**: Quests must load, track objectives, and verify completion locally. When a player completes a Community Quest contribution while offline, progress is cached and synced asynchronously once connection is restored.

---

## 2. Quest Database Schema (JSON Spec)

Quest structures are defined in JSON configurations. Objectives specify concrete completion requirements.

```json
{
  "questId": "quest_cove_halfway_dock",
  "questType": "Main",
  "giverNpcId": "npc_mara_tidekeeper",
  "requiredQuestIds": ["quest_cove_washed_shore"],
  "subjectDomain": "Math",
  "adaptiveSkillId": "math_fraction_halves",
  "objectives": [
    {
      "objectiveId": "obj_01_talk_to_mara",
      "descriptionKey": "quest_obj_cove_halfway_talk_mara",
      "type": "TALK_TO_NPC",
      "targetNpcId": "npc_mara_tidekeeper"
    },
    {
      "objectiveId": "obj_02_build_half_planks",
      "descriptionKey": "quest_obj_cove_halfway_build_planks",
      "type": "CRAFT_ITEM",
      "targetItemId": "item_plank_half",
      "requiredQuantity": 2
    },
    {
      "objectiveId": "obj_03_bridge_the_gap",
      "descriptionKey": "quest_obj_cove_halfway_bridge_gap",
      "type": "SOLVE_PUZZLE",
      "targetPuzzleId": "puzzle_cove_halfway_gap"
    }
  ],
  "rewards": {
    "glimmerCount": 50,
    "unlockedCosmeticId": "charm_mara_hook_replica",
    "unlockedMaterialId": "material_reed_fiber",
    "unlockedMaterialQuantity": 5
  }
}
```

---

## 3. Quest System C# Interfaces & Mastery Engine Integration

The Quest System queries the `IMasteryEngine` to request parameters when initializing gameplay puzzles.

### 3.1 Interface Contracts

```csharp
using System;
using System.Collections.Generic;

namespace QuestBit.Systems.Quest
{
    public enum QuestState
    {
        NotStarted,
        Active,
        ConditionsMet, // Objectives complete, waiting for NPC turn-in
        Completed
    }

    public interface IQuestObjective
    {
        string ObjectiveId { get; }
        string DescriptionKey { get; }
        bool IsCompleted { get; }
        void EvaluateObjective(string eventTrigger, object payload);
    }

    public interface IQuestSystem
    {
        IReadOnlyList<string> CompletedQuests { get; }
        QuestState GetQuestState(string questId);
        
        void AcceptQuest(string questId);
        void EvaluateQuestObjectives(string eventTrigger, object payload);
        void TurnInQuest(string questId);

        // Community Quest Integration
        int GetCommunityAggregateProgress(string communityQuestId);
        void SubmitCommunityContribution(string communityQuestId, int count);
    }

    // Shared Educational Competency engine interface (GDD §10.2)
    public interface IMasteryEngine
    {
        int GetCompetencyStage(string skillId); // Returns 1 (Introduced) through 5 (Automatic)
        float GetAdaptiveGapWidth(string skillId); // Returns dynamic length based on competency
    }
}
```

### 3.2 Dynamic Objective Resolver (Adaptive Parameter Injection)

This gameplay script runs when initializing a quest puzzle (e.g. Tidewell Cove bridge gap), dynamically querying the Mastery Engine for configuration parameters.

```csharp
using UnityEngine;
using VContainer;
using QuestBit.Systems.Quest;

namespace QuestBit.Gameplay.Math
{
    public class AdaptiveBridgeGapInitializer : MonoBehaviour
    {
        [SerializeField] private string _questId = "quest_cove_halfway_dock";
        [SerializeField] private string _skillId = "math_fraction_halves";
        [SerializeField] private Transform _plankSpawnAnchor = null!;

        private IQuestSystem _questSystem = null!;
        private IMasteryEngine _masteryEngine = null!;

        [Inject]
        public void Construct(IQuestSystem questSystem, IMasteryEngine masteryEngine)
        {
            _questSystem = questSystem;
            _masteryEngine = masteryEngine;
        }

        private void Start()
        {
            // Initialize puzzle only if player is currently on the matching quest stage
            if (_questSystem.GetQuestState(_questId) == QuestState.Active)
            {
                InitializeAdaptivePuzzle();
            }
        }

        private void InitializeAdaptivePuzzle()
        {
            // 1. Query the Mastery Engine for parameters matched to the child's demonstrated skill level
            float gapWidth = _masteryEngine.GetAdaptiveGapWidth(_skillId);
            int competence = _masteryEngine.GetCompetencyStage(_skillId);

            // 2. Adjust physical gap width in the 3D scene dynamically
            AdjustVisualGap(gapWidth);

            // 3. Set the visual label overlays based on child's competency stage
            bool displaySymbols = competence >= 3; // Introduce fractional text labels at stage 3+ (GDD §2.4.1)
            SetLabelDisplay(displaySymbols);

            Debug.Log($"[QuestEngine] Injected Adaptive Puzzle config: Width={gapWidth}, LabelsEnabled={displaySymbols}");
        }

        private void AdjustVisualGap(float width)
        {
            // Physically offset the far bank platform
            Vector3 position = _plankSpawnAnchor.localPosition;
            position.x = width; // Translate along horizontal axis
            _plankSpawnAnchor.localPosition = position;
        }

        private void SetLabelDisplay(bool enabled)
        {
            // Code here enables or disables numeric text meshes, falling back to pure shapes
        }
    }
}
```

---

## 4. Offline State Persistence & Async Community Sync

1. **Local State Saving**: Quest state updates (e.g., transitioning `NotStarted` -> `Active`) serialize directly to the local save game data, ensuring offline-first availability.
2. **Community Dock Contributions (GDD §5.6.1)**:
   * When the player contributes to the "Long Dock" community quest while offline, the local database increments their offline contribution buffer (e.g. `cove_long_dock_spans_helped += 1`).
   * Once connectivity returns, the networking sync queue posts the local contribution count asynchronously to our first-party server.
   * The server aggregates the total spans submitted by all players and returns the new global length, which dynamically updates the visual length of the Long Dock in the child's local overworld scene.

---

## 5. Failure Modes & Edge Cases

### 1. Adaptive Out-of-Bounds Parameter Injection
* **Symptom**: Mastery Engine returns a float gap width that is physically impossible to bridge (e.g., width of 0, or width larger than total tray plank length combined).
* **Mitigation**: The `AdaptiveBridgeGapInitializer` validates the injected parameters against a hardcoded range before modifying the scene. If the returned width violates constraints (e.g., width must be between 1.0 and 8.0 units), it falls back to a safe default width of 3.0 units.

### 2. Missing Pre-requisite Loop Locks
* **Symptom**: Player accesses Tidewell Cove Act 2 without completing Act 1 due to save corruption or debug skips.
* **Mitigation**: When a biome loads, the Quest System verifies that all prerequisite quest IDs (defined in the JSON database) exist in `completedQuestIds`. If prerequisites are missing, it transitions the game state back to `HubState` and resets the biome progress.

---

## 6. Verification & Automated Tests

1. **Quest Objective Progress Unit Test**:
   Assert that accepting `quest_cove_halfway_dock` transitions its state to `Active`, and:
   * Simulating an `OnPlankPlacedEvent` updates the third objective (`obj_03_bridge_the_gap`).
   * When all objectives are marked complete, the quest state changes to `ConditionsMet`.
   * Calling `TurnInQuest` removes ingredients from inventory, issues rewards, and changes the state to `Completed`.

2. **Adaptive Difficulty Injection Verification**:
   Mock the `IMasteryEngine` to return a competency stage of 1 (pre-symbolic) and a gap width of 2.0. Initialize the scene and assert that the far platform position equals exactly 2.0 and symbols remain hidden.
