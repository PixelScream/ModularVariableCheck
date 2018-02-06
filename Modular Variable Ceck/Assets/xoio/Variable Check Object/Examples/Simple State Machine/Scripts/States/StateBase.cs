using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for States of the State Machine
/// </summary>
public abstract class StateBase : ScriptableObject {

	public StateMachine_State StateRef { get; set; }
	
	// Scene reference to SM Controller 
	protected StateMachineController Controller { get; set; }

	// States to transition to and the check to make that happen
	public StateBase[] transitionTo;
	public VariableCheckObject[] transitions;

	public virtual void Init(StateMachineController con)
	{
		Controller = con;
		
		// Check there's a transition per state
		if(transitionTo.Length != transitions.Length)
		{
			Debug.LogError("transitions length doesn't match", this);
			return;
		}

		for(int i = 0; i < transitions.Length; i++)
		{
			// create a transition and assign it's comparison
			StateRef.AddTransition(new StateMachine_Transition(transitionTo[i].StateRef))
				.Test = transitions[i].CheckFunction;
		}
	}
}
