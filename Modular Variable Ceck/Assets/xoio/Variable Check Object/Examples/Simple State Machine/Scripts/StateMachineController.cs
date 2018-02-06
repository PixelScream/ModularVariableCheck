using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineController : MonoBehaviour {

	[SerializeField] ControllerSettings _settings;

	public Rigidbody M_Rigidbody { get; private set; }
	public Material M_Material {  get; private set; }

	public Vector3 Velocity 
	{
		get { return M_Rigidbody.velocity; }
		set { M_Rigidbody.velocity = value; }
	}


	// Use this for initialization
	void Start () {
		_settings.Controller = this;
		M_Rigidbody = GetComponent<Rigidbody>();
		M_Material = GetComponent<MeshRenderer>().material;
		InitStateMachine();
	}
	
	// Update is called once per frame
	void Update () {
		_stateMachine.UpdateStateMachine();
	}

	void OnGUI() {
        GUI.Label(new Rect(10, 10, 100, 20), _stateMachine.CurrentState.name);
    }

	void FixedUpdate()
	{
		_settings.Grounded = Physics.Raycast(transform.position, Vector3.down, _settings.GroundDistance, _settings.CollideWith);
		Debug.DrawRay (transform.position, Vector3.down * _settings.GroundDistance, Color.red);
	}


	StateMachine _stateMachine;
	void InitStateMachine()
	{
		_stateMachine = new StateMachine();

		// Create all the states
		for(int i = 0; i < _settings.States.Length; i++)
		{
			_settings.States[i].StateRef = _stateMachine.AddState(_settings.States[i].name);
		}

		// Initalize states
		for(int i = 0; i < _settings.States.Length; i++)
		{
			_settings.States[i].Init(this);
		}

		_stateMachine.CurrentState = _settings.States[0].StateRef;
		_stateMachine.CurrentState.BeforeEntering();
	}
}
