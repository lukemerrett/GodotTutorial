using Godot;
using System;

public class Main : Node
{
	// This allows us to configure which scene is used as the enemy mob
	// It can be set on the Inspector of the parent scene
	// This is super powerful as we can use it to plug in different enemy scenes
	// For example I may code a Mob I find more interesting than the original one
	// then am able to add it through the inspector without modifying any of the scene code
	[Export]
	public PackedScene Mob;

	// Players current score
	private int _score;

	private Random _random = new Random();

	public override void _Ready()
	{
		
	}
	
	public void NewGame()
	{
		_score = 0;  // Reset the score

		// Place the player back at the start position
		// Call the "Start" method on the player to allow the scene to set itself up
		var player = GetNode<Player>("Player");
		var startPosition = GetNode<Position2D>("StartPosition");
		player.Start(startPosition.Position);

		// Start the count down for the game to start
		GetNode<Timer>("StartTimer").Start();
		
		// Set the score back to 0 on the HUD and show the get ready message
		var hud = GetNode<HUD>("HUD");
		hud.UpdateScore(_score);
		hud.ShowMessage("Get Ready!");
	}
	
	// Triggers when the start timer expires
	private void _on_StartTimer_timeout()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}

	// Triggers every time the score timer loops
	// Increments the players score
	private void _on_ScoreTimer_timeout()
	{
		_score++;
		
		// Update score on HUD
		GetNode<HUD>("HUD").UpdateScore(_score);
	}
	
	// Triggers every time the mob timer loops
	// Spawns new mobs onto the screen
	private void _on_MobTimer_timeout()
	{
		// Choose a location along the mob path based on where the following
		// point currently is, then add a jitter to it so it's not predictable
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.Offset = _random.Next();

		// Create a new mob scene and add it to the Main scene
		var mobInstance = (RigidBody2D)Mob.Instance();
		AddChild(mobInstance);

		// Set the mob's position to where we've decided it should start
		mobInstance.Position = mobSpawnLocation.Position;

		// Set the mob's direction perpendicular to the path direction
		// So it flies away from the edge of the screen
		// Then add some randomness into it so it's not always exactly perpendicular
		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;
		direction += RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mobInstance.Rotation = direction;
		
		// Choose the speed the mob should move, using its configured min and max speed.
		var mob = mobInstance as Mob;
		mobInstance.LinearVelocity = new Vector2(RandRange(mob.MinSpeed, mob.MaxSpeed), 0).Rotated(direction);
	}

	// Called when the player is hit by an enemy
	private void GameOver()
	{
		// Stop the timers so Mobs stop spawning and the score stops incrementing
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		
		// HUD will show when it is game over
		GetNode<HUD>("HUD").ShowGameOver();
	}
	
	// Creates a random float between the values defined
	private float RandRange(float min, float max)
	{
		return (float)_random.NextDouble() * (max - min) + min;
	}
}
