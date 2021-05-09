using Godot;
using System;

public class HUD : CanvasLayer
{
	// Emitted when the Start Game button has been pressed
	[Signal]
	public delegate void StartGame();

	private Timer _messageTimer;
	private Label _message;
	private Label _scoreLabel;
	private Button _startButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_messageTimer = GetNode<Timer>("MessageTimer");
		_message = GetNode<Label>("Message");
		_scoreLabel = GetNode<Label>("ScoreLabel");
		_startButton = GetNode<Button>("StartButton");
	}
	
	// Updates the text of the score
	public void UpdateScore(int score)
	{
		_scoreLabel.Text = score.ToString();
	}
	
	async public void ShowGameOver()
	{
		// Set a message for the player
		ShowMessage("Game Over");

		// Wait for the message timer to expire before continuing
		await ToSignal(_messageTimer, "timeout");

		// Shows the original message
		_message.Text = "Dodge the\nCreeps!";
		_message.Show();

		// Wait 1 second using an in line timer, then show the start button
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		_startButton.Show();
	}
	
	// Shows the text given, then sets a timer that will hide the text once expired
	public void ShowMessage(string text)
	{
		_message.Text = text;
		_message.Show();

		_messageTimer.Start();
	}
	
	// Triggered whenever the message timer expires
	private void _on_MessageTimer_timeout()
	{
		// Hide the message from view
		_message.Hide();
	}

	// Triggered whenever the player has pressed the start button
	private void _on_StartButton_pressed()
	{
		// Hide the start button
		_startButton.Hide();
		
		// Tell other scenes the game has been started by the player
		EmitSignal("StartGame");
	}
}
