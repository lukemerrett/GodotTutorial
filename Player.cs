using Godot;
using System;

public class Player : Area2D
{
	// Creates a new event that other scenes can subscribe to
	// Player scene will fire the event when a condition is met
	// Other scenes can respond to it as the wish
	[Signal]
	public delegate void Hit();
	
	// Export allows us to modify the property in the inspector of the Scene associated with this script.
	// When using C#, it requires building the code using the MSBuild button on the bottom of the screen.
	[Export]
	public int Speed = 400;
	
	// Store the size of the game window
	private Vector2 _screenSize;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_screenSize = GetViewport().Size;
		
		// Hides the player scene and all child scenes
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		var velocity = CalculateVelocityFromInput();

		// Gets the sprit node that sits below the Player this script is attached to
		var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

		if (velocity.Length() > 0)
		{
			// "Normalized" ensures the vector's total length is 1.
			// So if the player is pressing right and up they don't move twice as fast.
			// Then we update the velocity based on the current speed setting for the player
			velocity = velocity.Normalized() * Speed;
			
			// Start playing whatever sprite is currently selected
			animatedSprite.Play();
		}
		else
		{
			animatedSprite.Stop();
		}
		
		ModifyPositionFromVelocity(velocity, delta);
		ChangeAnimationBasedOnVelocity(velocity, animatedSprite);
	}
	
	// Function to call when we start a new game
	public void Start(Vector2 pos)
	{
		Position = pos;  // Sets the initial position of the player
		Show();  // Show the player scene
		
		// Enable collision detection on the player
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}
	
	private Vector2 CalculateVelocityFromInput()
	{
		// A new vector we'll use to store the player's movement based on the current input
		var velocity = new Vector2();
		
		// We then change the vector based on whether right, left, down or up are currently being pressed by the player.

		if (Input.IsActionPressed("ui_right"))
		{
			velocity.x += 1;
		}

		if (Input.IsActionPressed("ui_left"))
		{
			velocity.x -= 1;
		}

		if (Input.IsActionPressed("ui_down"))
		{
			velocity.y += 1;
		}

		if (Input.IsActionPressed("ui_up"))
		{
			velocity.y -= 1;
		}
		
		return velocity;
	}
	
	private void ModifyPositionFromVelocity(Vector2 velocity, float delta)
	{
		// Firstly update the position so it has moved by the vector we have created times the delta, which is the time since we last calculated a vector.
		// Sometimes there will be a delay on when the _Process method is called so we can never assume the velocity is constant.
		Position += velocity * delta;
		
		// Then "clamp" the position, which ensures the current position is between a certain range.
		// In this case, we're stopping the player going off the edges of the game screen.
		Position = new Vector2(
			x: Mathf.Clamp(Position.x, 0, _screenSize.x),
			y: Mathf.Clamp(Position.y, 0, _screenSize.y)
		);
	}
	
	private void ChangeAnimationBasedOnVelocity(Vector2 velocity, AnimatedSprite animatedSprite)
	{
		if (velocity.x != 0) 
		{
			animatedSprite.Animation = "walk";  // Moving horizontally so play the walk animation
			animatedSprite.FlipV = false;  // No vertical flipping required as it's horizontal movement
			animatedSprite.FlipH = velocity.x < 0;  // If going left, flip animation to walk left
		}
		else if (velocity.y != 0)
		{
			animatedSprite.Animation = "up";  // Moving vertical so play the up animation
			animatedSprite.FlipV = velocity.y > 0;  // If going down, flip animation to move downwards
		}
	}
	
	private void _on_Player_body_entered(PhysicsBody2D body)
	{
		Hide();  // Hide the player scene when it's been hit
		EmitSignal("Hit");  // Send out our new signal for any interested scenes
		
		// Disables the collision detection on the player scene using a safe approach
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
	}
}
