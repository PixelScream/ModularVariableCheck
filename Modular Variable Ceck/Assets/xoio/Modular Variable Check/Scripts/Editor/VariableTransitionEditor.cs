using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Reflection;

[CustomEditor(typeof(VariableTransition))]
public class VariableTransitionEditor : Editor
{
	public PropertyInfo[] props;
	public string[] options;

    public int index = 0;

	protected VariableTransition transition;

	SerializedProperty checkAgainstFloat, compBool, compFloat, conTings;


	void OnEnable()
	{

		checkAgainstFloat = serializedObject.FindProperty("checkAgainstFloat");
		compBool = serializedObject.FindProperty("compBool");
		compFloat = serializedObject.FindProperty("compFloat");
		conTings = serializedObject.FindProperty("tings");
		Init();
	}

	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();

		//GUI.enabled = !Application.isPlaying;
/*
		GameEvent e = target as GameEvent;
		if (GUILayout.Button("Raise"))
			e.Raise();
*/


		EditorGUI.BeginChangeCheck();

		using (var checkObjectRef = new EditorGUI.ChangeCheckScope())
        {
            // Block of code with controls
            // that may set GUI.changed to true
			EditorGUILayout.PropertyField(conTings);
            if (checkObjectRef.changed)
            {
				EditorUtility.SetDirty( target );
				serializedObject.ApplyModifiedProperties();
				GetOptions();
				Debug.Log("checked object reference to " + conTings.name);
			}
		}
		

		int v = -1;
		if(transition.tings != null)
		{
			v = EditorGUILayout.Popup(index, options);

			if(transition.propType == typeof(bool))
			{
				EditorGUILayout.PropertyField(compBool);
			}
			else if(transition.propType == typeof(float))
			{
				EditorGUILayout.PropertyField(compFloat );
				EditorGUILayout.PropertyField(checkAgainstFloat, new GUIContent("Check Against"));
			}

		}
		else
		{
			EditorGUILayout.LabelField("Asign Controller Setting");
		}

		if(EditorGUI.EndChangeCheck())
		{
			
			if(v != -1 && v != index)
			{
				Set(v);
			}
			EditorUtility.SetDirty( target );
			serializedObject.ApplyModifiedProperties();
		}
		
		GUILayout.Label(GetResultString());
	}

	void Init()
	{
		transition = ((VariableTransition) target) ;
		

		GetOptions();

		if(!transition.Asigned)
			return;

		for(int i = 0; i < props.Length; i++)
		{
			if(props[i].Name == transition.propName)
			{
				index = i + 1;
				return;
			}
		}

	}

	[ContextMenu ("Update Options")]
	public void GetOptions()
	{
		if(transition.tings == null)
			return;
		props = transition.tings.GetType().GetProperties();

		// loop properties for their names + a blank slot at the begining;
		options = new string[props.Length + 1];
		options[0] = "--Choose Property--";
		for(int i = 0; i < props.Length; i ++)
		{
			options[i + 1] = props[i].Name + " | " 
						+ props[i].GetValue(transition.tings).GetType();
		}
		
	}

	void Set(int v)
	{
		index = v;

		// if the index is empty slot reset transition
		if(index == 0)
		{
			transition.propName = "";
			return;
		}
		
		System.Type t = props[index - 1].GetValue(transition.tings).GetType();

		if(t == typeof(bool) || t == typeof(float))
		{
			transition.propName = props[index - 1].Name;
			transition.propType = t;
		}
		else
		{
			transition.propName = "";
			Debug.LogWarning("Variable Transition only supports bools and floats at the mo, sorry\n"
					+ " tried type was " + transition.propType, this);	
		}


	}

	string GetResultString()
	{
		if(transition.Asigned)
		{
			if(transition.propType == typeof(bool))
			{
				return 	transition.propName + " is currently:" + ((bool)transition.prop.GetValue(transition.tings)) +
						", and this check would return:" + transition.ReflectionCheck();
			}
			else
			{
				return "will handle things other than bools momentarily";
			}
		}
		else
		{
			return "unasigned";
		}
	}


}