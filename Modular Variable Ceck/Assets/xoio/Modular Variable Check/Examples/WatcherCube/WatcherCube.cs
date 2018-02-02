using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class WatcherCube : MonoBehaviour {

	[SerializeField] ModularVariableObject _checkAgainst;
	Material _mat;

	Color _materialColor;
	Color MaterialColor {
		set
		{
			if(_materialColor != value)
			{
				_materialColor = value;
				_mat.color = _materialColor;
			}
		}
	}

	// Use this for initialization
	void Start () {
		_mat = GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
		MaterialColor = _checkAgainst.Check ? Color.green : Color.red;
	}
}
