using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName=("xoio/Test/Variable Tester"))]
public class VariableTester : ScriptableObject {

	[SerializeField] bool _testBool = false;
	public bool TestBool { get { return _testBool; } }

	[SerializeField] float _testFloat;
	public float TestFloat { get { return _testFloat; } }

	[SerializeField]
	public ModularCheckBase modCheck;

	/// <summary>
	/// Called when the script is loaded or a value is changed in the
	/// inspector (Called in the editor only).
	/// </summary>
	void OnValidate()
	{
		((ModularCheckTest)modCheck).test = _testBool;
	}

	[ContextMenu("Test")]
	public void Check()
	{
		Debug.Log(modCheck.Check());
	}

	/// <summary>
	/// This function is called when the object becomes enabled and active.
	/// </summary>
	void OnEnable()
	{
		if(modCheck == null)
			Init();
	}

	void Init ()
	{
		ScriptableObject so = ScriptableObject.CreateInstance(typeof(ModularCheckTest));
		so.name = "Test check";
		AssetDatabase.AddObjectToAsset(so, this);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		modCheck = so as ModularCheckTest;
	}
}
