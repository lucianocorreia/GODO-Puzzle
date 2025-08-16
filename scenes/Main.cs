using Godot;
using System;

namespace Game;

public partial class Main : Node2D
{
    public override void _Ready()
    {
        GD.Print("Main scene is ready!");
    }

    public override void _Process(double delta)
    {
        var mousePosition = GetGlobalMousePosition();
        var gridPosition = mousePosition / 64;
        gridPosition = gridPosition.Floor();
        GD.Print($"Mouse Position: {mousePosition}, Grid Position: {gridPosition}");

    }
}
