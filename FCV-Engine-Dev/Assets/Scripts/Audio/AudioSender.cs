using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// An intermediate class/component that can be used to contact the AudioManager to play a sound via Unity Events.
/// </summary>
public class AudioSender : MonoBehaviour
{
	[Tooltip("Check if you want to change/start the music. Uncheck if you want to play soundeffects.")]
	[HideInInspector, SerializeField] private bool _changeMusic;
	
	[Tooltip("Selects from which soundeffect library that the sound shall be played from.")]
	[HideInInspector, SerializeField] private SoundType _soundType;
	
	[Tooltip("Show/hide the index field.")]
	[HideInInspector, SerializeField] private bool _useIndex = false;
	
	[Tooltip("The index to request a specific audio file from the soundeffect library.")]
	[HideInInspector, SerializeField] private int _index;
	
	
	
	public void SendAudio()
	{
		if (AudioManager.Instance != null)
		{
			if (_changeMusic) AudioManager.Instance.ChangeMusic(_index);
			else AudioManager.Instance.PlaySound(_soundType, _useIndex ? _index : -1);
		}
	}
}

#if UNITY_EDITOR
/// <summary>
/// Custom editor for the AudioSender class.
/// Changes the UI based on whether to play a soundeffect or change the music. Also shows/hides the index field based on relevance.
/// </summary>
[CustomEditor(typeof(AudioSender))]
public class AudioSenderEditor : Editor
{
	private SerializedProperty _sendMusicProperty;
	private SerializedProperty _soundTypeProperty;
	private SerializedProperty _checkIndexProperty;
	private SerializedProperty _indexProperty;
	private void OnEnable()
	{
		_sendMusicProperty = serializedObject.FindProperty("_changeMusic");
		_soundTypeProperty = serializedObject.FindProperty("_soundType");
		_checkIndexProperty = serializedObject.FindProperty("_useIndex");
		_indexProperty = serializedObject.FindProperty("_index");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		serializedObject.Update();

		EditorGUILayout.PropertyField(_sendMusicProperty);
		
		if (_sendMusicProperty.boolValue)
		{
			EditorGUILayout.HelpBox("Please specify which index to request the song from the music library:", MessageType.None);
			EditorGUILayout.PropertyField(_indexProperty);
		}
		else
		{
			EditorGUILayout.PropertyField(_soundTypeProperty);
			EditorGUILayout.PropertyField(_checkIndexProperty);
			
			if (_checkIndexProperty.boolValue)
			{
				EditorGUILayout.HelpBox("Please specify which index to request the audio from the above mentioned soundeffect library:", MessageType.None);
				EditorGUILayout.PropertyField(_indexProperty);
			}
		}


		serializedObject.ApplyModifiedProperties();
	}
}
#endif
