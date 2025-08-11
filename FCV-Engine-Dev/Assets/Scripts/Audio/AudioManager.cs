using System;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Provides the functionality to easily change and implement new soundeffect and/or music into the game.
/// Implements a singleton pattern to ensure only one instance exists to avoid conflict and sound duplication.
/// </summary>
[ExecuteInEditMode]
public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance { get; private set; }
	[SerializeField] private AudioMixerGroup _audioMixerSoundEffects;
	[SerializeField] private AudioLibrary[] _soundEffects;
	[SerializeField] private AudioMixerGroup _audioMixerMusic;
	[SerializeField] private bool _playMusicAtStart;
	[SerializeField] private bool _MusicLoop = true;
	[SerializeField] private AudioLibrary _music;
	[SerializeField] private AudioMixerGroup _audioMixerExtras;
	[SerializeField] private AudioLibrary _extras;
	
#if UNITY_EDITOR
	/// <summary>
	/// Resizes and renames the sound effect SoundArray to inclide all (enum)SoundTypes.
	/// Excludes the last two enums (MUSIC & EXTRAS), since they are not part of the SoundArray array and are handled seperatly.
	/// </summary>
	private void OnEnable()
	{
		string[] names = Enum.GetNames(typeof(SoundType));
		Array.Resize(ref _soundEffects, names.Length - 2);
		
		for (int i = 0; i < _soundEffects.Length; i++)
			_soundEffects[i]._name = names[i];
	}
#endif
	
	/// <summary>
	/// Does a singleton check and if one already exists deletes itself.
	/// </summary>
	private void Awake()
	{
		SingletonCheck();
	}
	
	private void Start()
	{
		InitializeAudioSources();
		
		if(_playMusicAtStart)
			_music.audioSource.Play();
	}
	
	/// <summary>
	/// Run at Start.
	/// Checks if all (enum)SoundTypes has an AudioSource component. If not -> Creates a new AudioSource component, assign it, and hide it in the inspector.
	/// Also checks if all AudioSource components has a AudioMixreGroup and if there is one available.
	/// </summary>
	private void InitializeAudioSources()
	{
		AudioSource newAudioSource;
		
		// Sound effect:
		for (int i = 0; i < _soundEffects.Length; i++)
		{
			if (!_soundEffects[i].audioSource)
			{
				newAudioSource = gameObject.AddComponent<AudioSource>();
				newAudioSource.hideFlags = HideFlags.HideInInspector;
				_soundEffects[i].audioSource = newAudioSource;
			}
			
			if ((!_soundEffects[i].audioSource.outputAudioMixerGroup || _soundEffects[i].audioSource.outputAudioMixerGroup != _audioMixerSoundEffects) && _audioMixerSoundEffects)
				_soundEffects[i].audioSource.outputAudioMixerGroup = _audioMixerSoundEffects;
		}
		
		// Music:
		if (!_music.audioSource)
		{
			newAudioSource = gameObject.AddComponent<AudioSource>();
			newAudioSource.hideFlags = HideFlags.HideInInspector;
			_music.audioSource = newAudioSource;
		}
		
		if ((!_music.audioSource.outputAudioMixerGroup || _music.audioSource.outputAudioMixerGroup != _audioMixerSoundEffects) && _audioMixerSoundEffects)
				_music.audioSource.outputAudioMixerGroup = _audioMixerSoundEffects;
		
		_music.audioSource.loop = _MusicLoop;
		
		// Extras:
		if (!_extras.audioSource)
		{
			newAudioSource = gameObject.AddComponent<AudioSource>();
			newAudioSource.hideFlags = HideFlags.HideInInspector;
			_extras.audioSource = newAudioSource;
		}
		
		if ((!_extras.audioSource.outputAudioMixerGroup || _extras.audioSource.outputAudioMixerGroup != _audioMixerSoundEffects) && _audioMixerSoundEffects)
				_extras.audioSource.outputAudioMixerGroup = _audioMixerSoundEffects;
	}
	
	/// <summary>
	/// Plays the sound for the relevent SoundType Enum. If there are more then one sound in the array, then it picks randomly from the array.
	/// </summary>
	public void PlaySound(SoundType soundType, int index = -1)
	{
		AudioClip[] clips;
		AudioSource audioSource;
		int indexToPlay = 0;
		
		// Grab the relevent AudioSource and clips depending on the SoundType input:
		switch (soundType)
		{
			default:
				clips = _soundEffects[(int)soundType].GetAudioClips();
				audioSource = _soundEffects[(int)soundType].audioSource;
				break;
			case SoundType.Music:
				clips = _music.GetAudioClips();
				audioSource = _music.audioSource;
				break;
			case SoundType.Extras:
				clips = _extras.GetAudioClips();
				audioSource = _extras.audioSource;
				break;
		}
		
		// Check if there are no AudioClips in the array:
		if (clips.Length == 0)
		{
			Debug.Log("No corresponding audio file exists in the AudioManager.");
			return;
		}
		
		// Check if an index has an input. If not -> check if there are multiple clips and choose a random one:
		if (index != -1)
			indexToPlay = index;
		else 
			if (clips.Length > 1)
				indexToPlay = UnityEngine.Random.Range(0, clips.Length);
		
		// Check that there is a AudioClip at the index:
		if (indexToPlay > clips.Length)
		{
			Debug.Log("No Sound at index: "+indexToPlay);
			return;
		}
		
		audioSource.PlayOneShot(clips[indexToPlay]);
	}
	
	public void ChangeMusic(int index)
	{
		AudioClip[] clips = _music.GetAudioClips();
		AudioSource audioSource = _music.audioSource;
		
		audioSource.Stop();
		audioSource.clip = clips[index];
		audioSource.Play();
	}
	
	#region Singleton
	private void SingletonCheck()
	{
		if (Instance)
		{
			Destroy(this.gameObject);
			return;
		}
		Instance = this;
	}
	#endregion
}

#region Struct
/// <summary>
/// Basic structure to insure that all sounds have a dedicated AudioSource and an array of AudioClips.
/// </summary>
[Serializable]
public struct AudioLibrary
{
	[HideInInspector] public string _name;
	[HideInInspector] public AudioSource audioSource;
	[SerializeField] private AudioClip[] audioFiles;
	public AudioClip[] GetAudioClips() => audioFiles;
}
#endregion

#region Enum: SoundType
/// <summary>
/// Plays the sound for the relevent SoundType Enum. If there are more then one sound in the array, then it picks randomly from the array.
/// </summary>
[Serializable]
public enum SoundType
{
	SwordSwing,
	GunFire,
	BulletImpact,
	PlayerLand,
	PlayerJump,
	PlayerFootsteps,
	PlayerHurt,
	EnemyHurt,
	DoorOpen,
	
	
	[InspectorName(null)] Music, //not needed
	Extras
}
#endregion