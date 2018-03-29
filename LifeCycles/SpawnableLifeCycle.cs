using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDYXS.ThingSpawner;
using UnityEngine.Events;

[RequireComponent(typeof(EntityControllerBehaviour))]
public class SpawnableLifeCycle : 
	FSMWrapper<SpawnableLifeCycle.Events, SpawnableLifeCycle.States>,
	IFSMGlobalEventSpecifier<SpawnableLifeCycle.Events>,
	IFSMEventRouteSpecifier<SpawnableLifeCycle.Events, SpawnableLifeCycle.States>,
	IFSMStateHandler<SpawnableLifeCycle.States>
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

	public EntityControllerBehaviour entityController
	{
		get
		{
			if (_entityController == null)
			{
				_entityController = GetComponent<EntityControllerBehaviour>();
			}

			return _entityController;
		}
	}

	private EntityControllerBehaviour _entityController;

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
				OnReady.Invoke();
				break;
			case States.Recycle:
				this.Recycle();
				break;
		}
	}
	
	public UnityEvent OnReady = new UnityEvent();

	private void OnEnable()
	{
		if (entityController.HasSpawned)
		{
			SendEvent(Events.Initialise);
		}
		else
		{
			entityController.OnInitialised.AddListener(OnControllerInitialised);
		}
	}

	private void OnControllerInitialised()
	{
		entityController.OnInitialised.RemoveListener(OnControllerInitialised);
		SendEvent(Events.Initialise);
	}

	public void Teardown()
	{
		SendEvent(Events.Teardown);
	}
}
