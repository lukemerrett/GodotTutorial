using Godot;
using System;

public class Mob : RigidBody2D
{
	// Export allows us to modify these properties in the Inspector of the scene this script is attached to.
	// Provides an easy way to tweak properties when refining the game without digging through scripts.
	[Export]
	public int MinSpeed = 150; // Minimum speed range.

	[Export]
	public int MaxSpeed = 250; // Maximum speed range.
	
	// To help us randomly choose a speed, position and animation for the Mob.
	private static Random _random = new Random();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Select the animated sprite child node that lives under the parent Mob
		var animSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		
		// Get all possible animations we have created
		var mobTypes = animSprite.Frames.GetAnimationNames();
		
		// Choose one at random to play, this gives the mobs some variety
		animSprite.Animation = mobTypes[_random.Next(0, mobTypes.Length)];
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

	// Bound to the "screen_exited" signal of the Visibility Notifier
	// This will trigger when the Mob leaves the screen
	// Once it does we delete this mob using "QueueFree"
	private void _on_VisibilityNotifier2D_screen_exited()
	{
		QueueFree();
	}
}
