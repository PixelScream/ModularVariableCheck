using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;


/// <summary>
/// Modular Variable Check system
/// 
/// Does a comparison against an arbitrary scriptable object's property 
///
/// currently supports Bools and Floats
/// TODO: support more variable types
/// TODO: stored cooked info instead of recooking every time
/// TODO: fix reflection check
/// </summary>

[DefaultExecutionOrder(+100)]
[CreateAssetMenu (menuName="xoio/Variable/Variable Object")]
public class ModularVariableObject : ScriptableObject  {

	public ScriptableObject scriptRef;
	[SerializeField] ModularCheckBase _modularCheck;
	public ModularCheckBase ModularCheck
	{
		get
		{
			return _modularCheck;
		}
		#if UNITY_EDITOR
			set
			{
				
				if(_modularCheck != null)
				{
					Debug.Log("destroying " + _modularCheck.name);
					DestroyImmediate( _modularCheck, true);
				}
				_modularCheck = value;
				//if(_modularCheck != value)
				//{
					
				//}
				Dirty = false;
			}
		#endif
	}

	/// <summary>
	/// Checks against cooked Variable Check if one exists,
	/// otherwise uses reflection to cheese it.
	/// The idea being you'd cook them all before or at buildtime,
	/// but for faster iteration you can take the performance hit
	/// of a couple reflection calls
	/// </summary>
	public bool Check
	{
		get
		{
			#if UNITY_EDITOR
			if(!Dirty)
			{
				return _modularCheck.Check();
			}
			else
			{
				Debug.LogWarning(name + " isn't cooked, using relfection", this);
				return ReflectionCheck();
			}
			#else
				return _modularCheck.Check();
			#endif
		}
	}


#if UNITY_EDITOR

	/// <summary>
	/// The property name from the referenced object
	/// </summary>
	[SerializeField] public  string _propName;
	public string PropName 
	{
		get
		{
			 return _propName;
		}
		#if UNITY_EDITOR
			set 
			{
				Debug.Log("settings prop to: " + value);
				if(_propName != value)
				{
					
					_propName = value;
					UpdateProperty();
				}
			}
		#endif
	}

	public PropertyInfo prop;
	public Type propType;

	// bool info
	public enum ComparisonsBool
	{
		True,
		False
	}
	public ComparisonsBool compBool = ComparisonsBool.True;
	// float info
	public float checkAgainstFloat = 0;
	public enum ComparisonsFloat
	{
		Equal,
		NotEqual,
		Greater,
		Less
	}
	public ComparisonsFloat compFloat = ComparisonsFloat.Equal;

	// Caches the answer just incase it's called multiple times a frame
	// in their origonal design it shouldn't be,
	// but emergent uses of these transitions have proved it useful
	bool cachedAnswer;
	int lastCalled = -1;

	/// <summary>
	///	 Uses reflection to compare the stored info against the 
	///  reference object's property
	/// </summary>
	public bool ReflectionCheck()
	{
		
		if(Time.frameCount == lastCalled)
			return cachedAnswer;

		if(!Asigned)
		{
			Debug.LogError(this.name + " doesn't have a property name defined!", this);
			return false;
		}
			
		bool tempBool = false;
		if(propType == typeof(bool))
		{
			bool v = (bool)prop.GetValue(scriptRef);
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
			float v = (float)prop.GetValue(scriptRef);
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

    public void OnEnable()
    {
		if(Asigned)
		{
			UpdateProperty();
		}
    }

	public void UpdateProperty()
	{
		prop = scriptRef.GetType().GetProperty(PropName);
		propType = prop.GetValue(scriptRef).GetType();
	}

	public bool Asigned 
	{
		get
		{
			return PropName != null && PropName != "";
		}
	}

	[SerializeField] private bool dirty = false;
	public bool Dirty
	{
		get
		{
			return _modularCheck == null || dirty;
		}
		set
		{
			Debug.Log(value);
			dirty = _modularCheck == null || value;
			
		}
	}

#else
	/// <summary>
	/// On enable checks if the modular check is assigned
	/// </summary>
	void OnEnable()
	{
		if(_modularCheck != null)
		{
			Debug.LogError(name + " isn't cooked!! will error", this);
		}
	}
#endif

}
