using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectorVariableContainer : MonoBehaviour
{
    /// <summary>
    /// The ships each player gets at the start.
    /// </summary>
    public GameObject[] startingShipLoadout;
    /// <summary>
    /// The time table for secondary battle switch times.
    /// </summary>
    public float[] secondaryBattleTimetable;
    /// <summary>
    /// The velocities of cannon shells.
    /// </summary>
    public float[] shellVelocities;
    /// <summary>
    /// The prefabs for cannon shells.
    /// </summary>
    public GameObject[] cannonShellPrefabs;
    /// <summary>
    /// The gravity coefficient.
    /// </summary>
    public float gravity;
    /// <summary>
    /// The distance of each board from the center in secondary battles.
    /// </summary>
    public float secondaryBattleBoardDistance;
    /// <summary>
    /// The distance of each board from the center in main battles.
    /// </summary>
    public float mainBattleBoardDistance;
    /// <summary>
    /// The thickness of graphical lines on board grids.
    /// </summary>
    public float boardLineThickness;
    /// <summary>
    /// The default dimensions for secondary battle boards.
    /// </summary>
    public int defaultSecondaryBattleBoardDimensions;
    /// <summary>
    /// All of the UI elements.
    /// </summary>
    public UIElement[] UIElements;
    /// <summary>
    /// All of the visual modules given to each board.
    /// </summary>
    public string[] boardVisualModules;
    /// <summary>
    /// The material for the ship placement indicator.
    /// </summary>
    public Material shipPlacementIndicatorMaterial;
    /// <summary>
    /// The material for player tag backgrounds.
    /// </summary>
    public Material playerTagBackgroundMaterial;
    /// <summary>
    /// The active player tag background material.
    /// </summary>
    public Material activePlayerTagBackgroundMaterial;
    /// <summary>
    /// The targeting unconfirmed material.
    /// </summary>
    public Material targetingUnconfirmedMaterial;
    /// <summary>
    /// The targeting confirmed material.
    /// </summary>
    public Material targetingConfirmedMaterial;
    /// <summary>
    /// The destroyed ship highlight material.
    /// </summary>
    public Material destroyedShipHighlightMaterial;
    /// <summary>
    /// The intact ship highlight material.
    /// </summary>
    public Material intactShipHighlightMaterial;
}
