using System.Collections.Generic;
using Godot;
namespace StarterKit3DPlatformerCSharp.scripts;

public partial class AudioManager : Node
{
	// Code adapted from KidsCanCode
	private int _numPlayers = 12;
	private string _bus = "master";
	private readonly List<AudioStreamPlayer> _available = [];  // The available players.
	private readonly Queue<string> _queue = new();  // The queue of sounds to play.
	
	public override void _Ready()
	{
		for (var i = 0; i < _numPlayers; i++)
		{
			var p = new AudioStreamPlayer();
			AddChild(p);
            
			_available.Add(p);
            
			p.VolumeDb = -10;
			p.Finished += () => OnStreamFinished(p);
			p.Bus = _bus;
		}
	}
	private void OnStreamFinished(AudioStreamPlayer stream)
	{
		_available.Add(stream);
	}
	
	public void Play(string soundPath)
	{
		_queue.Enqueue(soundPath);
	}

	public override void _Process(double delta)
	{
		if (_queue.Count <= 0 || _available.Count <= 0) return;
		_available[0].Stream = GD.Load<AudioStream>(_queue.Dequeue());
		_available[0].Play();
		_available[0].PitchScale = (float)GD.RandRange(0.9f, 1.1f);
		_available.RemoveAt(0);
	}
}