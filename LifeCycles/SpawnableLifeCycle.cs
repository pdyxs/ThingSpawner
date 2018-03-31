using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDYXS.ThingSpawner;
using UnityEngine.Events;

public interface ILifecycle
{
	void Initialise();
	void Teardown();
}

[RequireComponent(typeof(EntityControllerBehaviour))]
public class SpawnableLifeCycle : 
	FSMWrapper<SpawnableLifeCycle.Events, SpawnableLifeCycle.States>,
	IFSMGlobalEventSpecifier<SpawnableLifeCycle.Events>,
	IFSMEventRouteSpecifier<SpawnableLifeCycle.Events, SpawnableLifeCycle.States>,
	IFSMStateHandler<SpawnableLifeCycle.States>,
	ILifecycle
{
	public enum Events
	{
		Initialise,
		Teardown
	}

	public enum States
	{
		Ready,
		Teardown,
		Recycle
	}

	public Events[] GlobalEvents()
	{
		return new Events[] { Events.Initialise };
	}

	public Events[] EventsFrom(States state)
	{
		switch (state)
		{
			case States.Ready:
				return new Events[] {Events.Teardown};
		}

		return new Events[] { };
	}

	public void StateEntered(States state)
	{
		switch (state)
		{
			case States.Ready:
				OnReady.Raise(this);
				break;
			case States.Recycle:
				this.Recycle();
				break;
		}
	}

	public GameEvent OnReady;

	public void Initialise()
	{
		SendEvent(Events.Initialise);
	}

	public void Teardown()
	{
		SendEvent(Events.Teardown);
	}
}
