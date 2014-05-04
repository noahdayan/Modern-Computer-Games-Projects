using UnityEngine;
using System.Collections;
using System;

public class BehaviourTree : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	// Selector Node for Behaviour Tree
	public static Action Selector(Func<bool> cond, Action ifTrue, Action ifFalse)
	{
		return () => {

			if(cond())
			{
				ifTrue();
			}
			else
			{
				ifFalse();
			}

		};
	}

	// Sequencer Node for Behaviour Tree
	public static Action Sequencer(Action a, Action b)
	{
		return () => {

			a();
			b();

		};
	}
}
