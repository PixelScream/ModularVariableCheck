﻿using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Reflection;

namespace ModularVariableCheck
{
/// <summary>
/// Custom Editor for Modular Variable Object
/// </summary>

[CustomEditor(typeof(ModularVariableObject))]
public class ModularVariableEditor : Editor
{
	public PropertyInfo[] props;

	public string[] options, propNames;

    public int index = 0;

	protected ModularVariableObject variableObject;

	SerializedProperty checkAgainstFloat, compBool, compFloat, scriptRefProp, propName, dirty;


	void OnEnable()
	{
		propName = serializedObject.FindProperty("_propName");
		checkAgainstFloat = serializedObject.FindProperty("checkAgainstFloat");
		compBool = serializedObject.FindProperty("compBool");
		compFloat = serializedObject.FindProperty("compFloat");
		scriptRefProp = serializedObject.FindProperty("scriptRef");
		dirty = serializedObject.FindProperty("_dirty");
		Init();
	}

	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();

		//GUI.enabled = !Application.isPlaying;



		EditorGUI.BeginChangeCheck();

		using (var checkObjectRef = new EditorGUI.ChangeCheckScope())
        {
			EditorGUILayout.PropertyField(scriptRefProp);
            if (checkObjectRef.changed)
            {
				Apply();
				GetOptions();
				//Debug.Log("checked object reference to " + scriptRefProp.name);
			}
		}
		

		int v = -1;
		if(variableObject.scriptRef != null)
		{
			using (var checkSettings = new EditorGUI.ChangeCheckScope())
        	{
				v = EditorGUILayout.Popup(index, options);

				if(variableObject.propType == typeof(bool))
				{
					EditorGUILayout.PropertyField(compBool);
				}
				else if(variableObject.propType == typeof(float))
				{
					EditorGUILayout.PropertyField(compFloat );
					EditorGUILayout.PropertyField(checkAgainstFloat, new GUIContent("Check Against"));
				}
				
				if(checkSettings.changed)
				{
					if(v != -1 && v != index)
					{
						Set(v);
					}
					Apply();

					variableObject.UpdateProperty();
				}
			}

			if(!Application.isPlaying && variableObject.Dirty && GUILayout.Button("Cook"))
			{
				EditorWindow window = EditorWindow.GetWindow(typeof(VariableBuilder));
				VariableBuilder builder = window as VariableBuilder;
				builder.variableObject = variableObject;
				builder.Create();
			}
		}
		else
		{
			EditorGUILayout.LabelField("Asign Controller Setting");
		}
		
		GUILayout.Label(GetResultString());
	}

	void Apply()
	{
		dirty.boolValue = true;
		//variableObject.Dirty = true;
		EditorUtility.SetDirty( target );
		serializedObject.ApplyModifiedProperties();
		
	}

	/// <summary>
	/// Initalize, populate list, find correct place in list
	/// </summary>
	void Init()
	{
		variableObject = ((ModularVariableObject) target) ;
		

		GetOptions();

		// if property name assigned set correct point in list
		if(!variableObject.Asigned)
			return;

		for(int i = 0; i < options.Length; i++)
		{
			if(props[i].Name == variableObject.PropName)
			{
				index = i + 1;
				return;
			}
		}

	}

	/// <summary>
	/// Reflects over script reference and returns all properties into list
	/// </summary>
	[ContextMenu ("Update Options")]
	public void GetOptions()
	{
		if(variableObject.scriptRef == null)
			return;
		props = variableObject.scriptRef.GetType().GetProperties();

		// loop properties for their names + a blank slot at the begining;
		options = new string[props.Length + 1];
		options[0] = "--Choose Property--";
		for(int i = 0; i < props.Length; i ++)
		{
			options[i + 1] = props[i].Name + " | " 
						+ props[i].GetValue(variableObject.scriptRef).GetType();
		}
		
	}

	/// <summary>
	/// Sets the property out of the list
	/// </summary>
	void Set(int v)
	{
		index = v;

		// if the index is empty slot reset transition
		if(index == 0)
		{
			propName.stringValue = "";
			//variableObject.PropName = "";
			return;
		}
		
		System.Type t = props[index - 1].GetValue(variableObject.scriptRef).GetType();

		if(t == typeof(bool) || t == typeof(float))
		{
			propName.stringValue = props[index - 1].Name;
			//variableObject.PropName = props[index - 1].Name;
			variableObject.propType = t;
		}
		else
		{
			propName.stringValue = "";
			//variableObject.PropName = "";
			Debug.LogWarning("Variable Transition only supports bools and floats at the mo, sorry\n"
					+ " tried type was " + variableObject.propType, this);	
		}
		//variableObject.UpdateProperty();
	}

	/// <summary>
	/// For supported types returns if the variable will return true or false
	/// </summary>
	string GetResultString()
	{
		if(variableObject.Asigned )
		{
			

			if(variableObject.propType == typeof(bool) || variableObject.propType == typeof(float))
			{
				
				return 	variableObject.PropName + " is currently:" + 
						(variableObject.prop.GetValue(variableObject.scriptRef)) + 
						", \nand this check would return:" + variableObject.ReflectionCheck();
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

}