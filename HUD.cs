using Godot;
using System;

public class HUD : CanvasLayer
{
	// Emitted when the Start Game button has been pressed
	[Signal]
	public delegate void StartGame();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}
	
	// Updates the text of the score
	public void UpdateScore(int score)
	{
		GetNode<Label>("ScoreLabel").Text = score.ToString();
	}
	
	async public void ShowGameOver()
	{
		// Set a message for the player
		ShowMessage("Game Over");

		// Wait for the message timer to expire before continuing
		var messageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(messageTimer, "timeout");

		// Shows the original message
		var message = GetNode<Label>("Message");
		message.Text = "Dodge the\nCreeps!";
		message.Show();

		// Wait 1 second using an in line timer, then show the start button
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		GetNode<Button>("StartButton").Show();
	}
	
	// Shows the text given, then sets a timer that will hide the text once expired
	public void ShowMessage(string text)
	{
		var message = GetNode<Label>("Message");
		message.Text = text;
		message.Show();

		GetNode<Timer>("MessageTimer").Start();
	}
	
	// Triggered whenever the message timer expires
	private void _on_MessageTimer_timeout()
	{
		// Hide the message from view
		GetNode<Label>("Message").Hide();
	}

	// Triggered whenever the player has pressed the start button
	private void _on_StartButton_pressed()
	{
		// Hide the start button
		GetNode<Button>("StartButton").Hide();
		
		// Tell other scenes the game has been started by the player
		EmitSignal("StartGame");
	}


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
