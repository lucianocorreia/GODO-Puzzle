using System;
using System.Collections.Generic;
using System.Linq;
using Game.Autoloads;
using Game.Components;
using Godot;

namespace Game.Manager;

public partial class GridManager : Node
{
    private HashSet<Vector2I> validBuildableTiles = new();

    [Export]
    private TileMapLayer highlightTileMapLayer;

    [Export]
    private TileMapLayer baseTerrainTileMapLayer;

    public override void _Ready()
    {
        GameEvents.Instance.BuildingPlaced += OnBuildingPlaced;
    }

    public void ClearHilightedTiles()
    {
        highlightTileMapLayer.Clear();
    }

    public bool IsTilePositionValid(Vector2I tilePosition)
    {
        var customData = baseTerrainTileMapLayer.GetCellTileData(tilePosition);
        if (customData == null)
        {
            return false;
        }

        return (bool)customData.GetCustomData("buildable");
    }

    public bool IsTilePositionBuildable(Vector2I tilePosition)
    {
        return validBuildableTiles.Contains(tilePosition);
    }

    public Vector2I GetMouseGridCellPosition()
    {
        var mousePosition = highlightTileMapLayer.GetGlobalMousePosition();
        var gridPosition = mousePosition / 64;
        gridPosition = gridPosition.Floor();
        return new Vector2I((int)gridPosition.X, (int)gridPosition.Y);
    }

    public void HighlightBuildableTiles()
    {
        foreach (var tilePosition in validBuildableTiles)
        {
            highlightTileMapLayer.SetCell(tilePosition, 0, Vector2I.Zero);
        }
    }

    public void HighlightExpandedBuildableTiles(Vector2I rootCell, int radius)
    {
        ClearHilightedTiles();
        HighlightBuildableTiles();

        var validTiles = GetValidTilesInRadius(rootCell, radius).ToHashSet();
        var expandedTiles = validTiles.Except(validBuildableTiles).Except(GetOccupiedTiles());

        var atlasCoords = new Vector2I(1, 0);
        foreach (var tilePosition in expandedTiles)
        {
            highlightTileMapLayer.SetCell(tilePosition, 0, atlasCoords);
        }

    }

    #region Private Methods

    private void UpdateValidBuildableTiles(BuildingComponent buildingComponent)
    {
        var rootCell = buildingComponent.GetGridCellPosition();

        var validTiles = GetValidTilesInRadius(rootCell, buildingComponent.BuildableRadius);
        validBuildableTiles.UnionWith(validTiles);

        var buildingComponents = GetTree().GetNodesInGroup(nameof(BuildingComponent))
            .Cast<BuildingComponent>();
        // foreach (var component in buildingComponents)
        // {
        //     validBuildableTiles.Remove(component.GetGridCellPosition());
        // }
        validBuildableTiles.ExceptWith(GetOccupiedTiles());
    }

    private List<Vector2I> GetValidTilesInRadius(Vector2I rootCell, int radius)
    {
        var result = new List<Vector2I>();

        for (var x = rootCell.X - radius; x <= rootCell.X + radius; x++)
        {
            for (var y = rootCell.Y - radius; y <= rootCell.Y + radius; y++)
            {
                var tilePosition = new Vector2I(x, y);
                if (!IsTilePositionValid(tilePosition))
                    continue;
                result.Add(tilePosition);
            }
        }

        return result;
    }

    private IEnumerable<Vector2I> GetOccupiedTiles()
    {
        return GetTree().GetNodesInGroup(nameof(BuildingComponent))
            .Cast<BuildingComponent>()
            .Select(c => c.GetGridCellPosition());
    }

    #endregion

    #region Events 

    private void OnBuildingPlaced(BuildingComponent buildingComponent)
    {
        UpdateValidBuildableTiles(buildingComponent);
    }

    #endregion

}
