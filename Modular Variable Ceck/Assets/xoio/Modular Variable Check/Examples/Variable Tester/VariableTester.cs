using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName=("xoio/Test/Variable Tester"))]
public class VariableTester : ScriptableObject {

	[SerializeField] bool _testBool = false;
	public bool TestBool { get { return _testBool; } }

	[SerializeField] float _testFloat;
	public float TestFloat { get { return _testFloat; } }


}
