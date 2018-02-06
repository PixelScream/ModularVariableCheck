using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// a super simple state machine + states/transitions
/// </summary>

public class StateMachine 
{
	StateMachine_State _currentState;
	
	public StateMachine_State CurrentState
	{
		get { return _currentState; }
        set { _currentState = value; }
	}
	
	public StateMachine_State AddState(string stateName)
	{
		return new StateMachine_State(this, stateName);
	}

	public void UpdateStateMachine()
	{
		_currentState.UpdateState();
	}

}





public delegate void StateMachineDelegate();
public delegate bool StateTransitionDelegate();

public class StateMachine_Transition
{
	StateMachine_State _targetState;

	public StateMachine_State TargetState { get { return _targetState; } }

	public StateMachine_Transition(StateMachine_State target)
	{
		_targetState = target;
	} 

	public StateTransitionDelegate Test = delegate { return false; };


}




public class StateMachine_State 
{
	StateMachine _stateMachine;
	
	public string name { get; private set; }

	public StateMachine_State(StateMachine machine, string n)
	{
		_stateMachine = machine;
		name = n;
	}

	public StateMachineDelegate BeforeEntering = delegate {};
	public StateMachineDelegate Loop = delegate {};
	public StateMachineDelegate BeforeLeaving = delegate {};

	List<StateMachine_Transition> transitions = new List<StateMachine_Transition>();

	public StateMachine_Transition AddTransition(StateMachine_Transition t)
	{
		transitions.Add(t);
		return t;
	}

	public void UpdateState()
	{
		for(int i = 0; i < transitions.Count; i++)
		{
			if(transitions[i].Test.Invoke())
			{
				//Debug.Log("transitioning from:" + name + ",  to:" + transitions[i].TargetState.name);
				BeforeLeaving.Invoke();
				_stateMachine.CurrentState = transitions[i].TargetState;
				transitions[i].TargetState.BeforeEntering.Invoke();
				return;
			}
		}

		Loop.Invoke();
	}
}
