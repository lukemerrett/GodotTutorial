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
	
	private Player _player;
	private HUD _hud;
	private Position2D _startPosition;
	private PathFollow2D _mobSpawnLocation;
	private Timer _startTimer;
	private Timer _mobTimer;
	private Timer _scoreTimer;
	private AudioStreamPlayer _music;
	private AudioStreamPlayer _deathSound;

	private Random _random = new Random();

	public override void _Ready()
	{
		_player = GetNode<Player>("Player");
		_hud = GetNode<HUD>("HUD");
		_startPosition = GetNode<Position2D>("StartPosition");
		_mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		_startTimer = GetNode<Timer>("StartTimer");
		_mobTimer = GetNode<Timer>("MobTimer");
		_scoreTimer = GetNode<Timer>("ScoreTimer");
		_music = GetNode<AudioStreamPlayer>("Music");
		_deathSound = GetNode<AudioStreamPlayer>("DeathSound");
	}
	
	public void NewGame()
	{
		_score = 0;  // Reset the score

		// Place the player back at the start position
		// Call the "Start" method on the player to allow the scene to set itself up
		_player.Start(_startPosition.Position);

		// Start the count down for the game to start
		_startTimer.Start();
		
		// Set the score back to 0 on the HUD and show the get ready message
		_hud.UpdateScore(_score);
		_hud.ShowMessage("Get Ready!");
		
		// Start playing music
		_music.Play();
	}
	
	// Triggers when the start timer expires
	private void _on_StartTimer_timeout()
	{
		_mobTimer.Start();
		_scoreTimer.Start();
	}

	// Triggers every time the score timer loops
	// Increments the players score
	private void _on_ScoreTimer_timeout()
	{
		_score++;
		_hud.UpdateScore(_score);
	}
	
	// Triggers every time the mob timer loops
	// Spawns new mobs onto the screen
	private void _on_MobTimer_timeout()
	{
		// Choose a location along the mob path based on where the following
		// point currently is, then add a jitter to it so it's not predictable
		_mobSpawnLocation.Offset = _random.Next();

		// Create a new mob scene and add it to the Main scene
		var mobInstance = (RigidBody2D)Mob.Instance();
		AddChild(mobInstance);

		// Set the mob's position to where we've decided it should start
		mobInstance.Position = _mobSpawnLocation.Position;

		// Set the mob's direction perpendicular to the path direction
		// So it flies away from the edge of the screen
		// Then add some randomness into it so it's not always exactly perpendicular
		float direction = _mobSpawnLocation.Rotation + Mathf.Pi / 2;
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
		_mobTimer.Stop();
		_scoreTimer.Stop();
		
		// Delete all Mobs by calling their group name as configured on the Mob scene
		GetTree().CallGroup("mobs", "queue_free");
		
		_hud.ShowGameOver();
		_music.Stop();
		_deathSound.Play();
	}
	
	// Creates a random float between the values defined
	private float RandRange(float min, float max)
	{
		return (float)_random.NextDouble() * (max - min) + min;
	}
}
