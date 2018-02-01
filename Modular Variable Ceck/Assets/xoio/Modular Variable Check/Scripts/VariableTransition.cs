using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;


/// <summary>
/// Modular transition system
/// 
/// Does a comparison against an arbitrary scriptable object's property 
///
/// currently supports Bools and Floats
/// TODO support more 
/// </summary>

[DefaultExecutionOrder(+100)]
[CreateAssetMenu (menuName="xoio/Transitions/Variable")]
// ISerializationCallbackReceiver < if we ever want to try before and after serilaztion again ..
public class VariableTransition : ScriptableObject  {

	public ScriptableObject tings;
	ModularCheckBase _modularCheck;
	public ModularCheckBase ModularCheck
	{
		get
		{
			return _modularCheck;
		}
		set
		{
			if(_modularCheck != value)
			{
				DestroyImmediate( _modularCheck, true);
				_modularCheck = value;
			}
			Dirty = false;
		}
	}

	public bool Check
	{
		get
		{
			if(!Dirty)
			{
				return _modularCheck.Check();
			}
			else
			{
				return ReflectionCheck();
			}
		}
	}

#region Info
	public float checkAgainstFloat = 0;


	public string propName;
	public PropertyInfo prop;
	public Type propType, boolType;

	public enum ComparisonsFloat
	{
		Equal,
		NotEqual,
		Greater,
		Less
	}
	public ComparisonsFloat compFloat = ComparisonsFloat.Equal;
	public enum ComparisonsBool
	{
		True,
		False
	}
	public ComparisonsBool compBool = ComparisonsBool.True;

	// Caches the answer just incase it's called multiple times a frame
	// in their origonal design it shouldn't be,
	// but emergent uses of these transitions have proved it useful
	bool cachedAnswer;
	int lastCalled = -1;

	public bool ReflectionCheck()
	{
		
		if(Time.frameCount == lastCalled)
			return cachedAnswer;

		if(prop == null)
		{
			Debug.LogError(this.name + " doesn't have a transition property defined!", this);
			return false;
		}
			
		bool tempBool = false;
		if(propType == boolType)
		{
			bool v = (bool)prop.GetValue(tings);
			switch(compBool)
			{
				case ComparisonsBool.True:
					tempBool = v == true;
					break;
				case ComparisonsBool.False:
					tempBool = v == false;
					break;
			}
		}
		else
		{
			float v = (float)prop.GetValue(tings);
			switch(compFloat)
			{
				case ComparisonsFloat.Equal:
					tempBool = v == checkAgainstFloat;
					break;
				case ComparisonsFloat.NotEqual:
					tempBool = v != checkAgainstFloat;
					break;
				case ComparisonsFloat.Greater:
					tempBool = v > checkAgainstFloat;
					break;
				case ComparisonsFloat.Less:
					tempBool = v < checkAgainstFloat;
					break;
			}
		}
		cachedAnswer = tempBool;
		lastCalled = Time.frameCount;
		return cachedAnswer;
	}

	// for testing purposes, print the property name
	[ContextMenu("print prop")]
	public void PrintProp()
	{
		Debug.Log("prop is currently " + prop.Name + ", of type " + propType);
	
	}
/*
	// experimentation into bypassing unity serialization limits
	// currently not advisable
	public void OnBeforeSerialize()
    {
		// propertyinfos don't serialize so store the name as a string
		// and retrieve it at runtime
		propName = prop != null ? prop.Name : "";
    }
*/
    public void OnEnable()
    {
		if(Asigned)
		{
			prop = tings.GetType().GetProperty(propName);
			propType = prop.GetValue(tings).GetType();
			boolType = typeof(bool);
		}
    }

	public bool Asigned 
	{
		get
		{
			return propName == null && propName == "";
		}
	}


#endregion

	private bool dirty = false;
	public bool Dirty
	{
		get
		{
			return _modularCheck == null || dirty;
		}
		set
		{
			dirty = _modularCheck != null && value;
			
		}
	}
}
