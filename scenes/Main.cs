using Godot;
using System.Collections.Generic;

namespace Game;

public partial class Main : Node
{
    private Sprite2D cursor;
    private PackedScene buildingScene;
    private Button placeBuildingButton;
    private TileMapLayer highlightTileMapLayer;

    private Vector2? hoveredGridCell;
    private HashSet<Vector2> occupiedCells = new();

    public override void _Ready()
    {
        buildingScene = GD.Load<PackedScene>("uid://b8h1x7wl0fhqj");
        cursor = GetNode<Sprite2D>("Cursor");
        highlightTileMapLayer = GetNode<TileMapLayer>("HighLightTileMapLayer");

        cursor.Visible = false;

        placeBuildingButton = GetNode<Button>("PlaceBuildingButton");
        placeBuildingButton.Pressed += OnButtonPressed;
    }

    public override void _Process(double delta)
    {
        var gridPosition = GetMouseGridCellPosition();
        cursor.GlobalPosition = gridPosition * 64;

        if (cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPosition))
        {
            hoveredGridCell = gridPosition;
            UpdateHighLightTileMapLayer();
        }
    }

    public override void _UnhandledInput(InputEvent evt)
    {
        if (hoveredGridCell.HasValue && evt.IsActionPressed("left_click") && !occupiedCells.Contains(hoveredGridCell.Value))
        {
            PlaceBuildingAtHoverdCellPosition();
            cursor.Visible = false;
        }
    }

    private Vector2 GetMouseGridCellPosition()
    {
        var mousePosition = highlightTileMapLayer.GetGlobalMousePosition();
        var gridPosition = mousePosition / 64;
        gridPosition = gridPosition.Floor();
        return gridPosition;
    }

    private void PlaceBuildingAtHoverdCellPosition()
    {
        if (!hoveredGridCell.HasValue) return;

        var building = buildingScene.Instantiate<Node2D>();
        AddChild(building);

        building.GlobalPosition = hoveredGridCell.Value * 64;
        occupiedCells.Add(hoveredGridCell.Value);

        hoveredGridCell = null;
        UpdateHighLightTileMapLayer();
    }

    private void UpdateHighLightTileMapLayer()
    {
        highlightTileMapLayer.Clear();

        if (!hoveredGridCell.HasValue)
        {
            return;
        }

        for (var x = hoveredGridCell.Value.X - 3; x <= hoveredGridCell.Value.X + 3; x++)
        {
            for (var y = hoveredGridCell.Value.Y - 3; y <= hoveredGridCell.Value.Y + 3; y++)
            {
                highlightTileMapLayer.SetCell(new Vector2I((int)x, (int)y), 0, Vector2I.Zero);
            }
        }
    }

    private void OnButtonPressed()
    {
        cursor.Visible = true;
    }
}
