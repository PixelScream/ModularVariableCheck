using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///	Setting for State Machine Controller
/// </summary>

[CreateAssetMenu(menuName=("xoio/State Machine/Settings"))]
public class ControllerSettings : ScriptableObject {

	[Header ("Scriptable Objects")]
	public StateBase[] States;

	// Scene references
	public StateMachineController Controller { get; set; } = null;

	// Runtime Properties
	public bool Grounded { get; set; } = false;


	[Header ("Settings")]
	[SerializeField] float _groundDistance = 2;
	public float GroundDistance { get { return _groundDistance; } }
	
	[SerializeField] LayerMask _collideWith;
	public LayerMask CollideWith { get { return _collideWith; } }

	
}
