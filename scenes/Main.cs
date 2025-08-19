using Game.Manager;
using Godot;

namespace Game;

public partial class Main : Node
{
    private GridManager gridManager;
    private Sprite2D cursor;
    private PackedScene buildingScene;
    private Button placeBuildingButton;

    private Vector2I? hoveredGridCell;

    public override void _Ready()
    {
        buildingScene = GD.Load<PackedScene>("uid://b8h1x7wl0fhqj");
        gridManager = GetNode<GridManager>("GridManager");

        cursor = GetNode<Sprite2D>("Cursor");

        cursor.Visible = false;

        placeBuildingButton = GetNode<Button>("PlaceBuildingButton");
        placeBuildingButton.Pressed += OnButtonPressed;
    }

    public override void _Process(double delta)
    {
        var gridPosition = gridManager.GetMouseGridCellPosition();
        cursor.GlobalPosition = gridPosition * 64;

        if (cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPosition))
        {
            hoveredGridCell = gridPosition;
            gridManager.HighlightExpandedBuildableTiles(hoveredGridCell.Value, 3);
        }
    }

    public override void _UnhandledInput(InputEvent evt)
    {
        if (hoveredGridCell.HasValue && evt.IsActionPressed("left_click") && gridManager.IsTilePositionBuildable(hoveredGridCell.Value))
        {
            PlaceBuildingAtHoverdCellPosition();
            cursor.Visible = false;
        }
    }

    private void PlaceBuildingAtHoverdCellPosition()
    {
        if (!hoveredGridCell.HasValue) return;

        var building = buildingScene.Instantiate<Node2D>();
        AddChild(building);

        building.GlobalPosition = hoveredGridCell.Value * 64;

        hoveredGridCell = null;
        gridManager.ClearHilightedTiles();
    }

    private void OnButtonPressed()
    {
        cursor.Visible = true;
        gridManager.HighlightBuildableTiles();
    }

}
