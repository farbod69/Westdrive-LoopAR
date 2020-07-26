﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CriticalEventController: MonoBehaviour
{
    #region Fields

    [Space]
    [Header("Consistent Event Objects")]
    [SerializeField] private TrafficEventTrigger startTrigger;
    [SerializeField] private TrafficEventTrigger endTrigger;
    [SerializeField] private GameObject consistentEventObjects;

    [Space]
    [Header("Event Objects")]
    [Tooltip("The gameObject which is the parent of the event object")]
    [SerializeField] private GameObject eventObjectParent;
    [SerializeField] private List<GameObject> eventObjects;
    [Tooltip("Should the event objects be active or not when experiment begins")] 
    [SerializeField] private GameObject respawnPoint;

    [Space] [Header("Event Setting")]
    [Tooltip("Time the car needs from informing the driver to giving them the control. (0 - 15 seconds)")]
    [Range(0,15)] [SerializeField] private float startEventDelay = 2.5f;
    [Tooltip("Time the car needs from informing the driver to taking back the control. (0 - 10 seconds)")]
    [Range(0,10)] [SerializeField] private float endEventDelay = 1f;
    [Tooltip("End the event automatically after given (0 - 120) seconds in case the participant stays idle.")] 
    [Range(0,120)] [SerializeField] private float eventIdleDuration = 10f;
    [SerializeField] private bool eventObjectActive;
    
    
    private RestrictedZoneTrigger[] _restrictedZoneTriggers;
    private GameObject _targetedCar;
    private bool _activatedEvent;
    private MeshRenderer[] _meshRenderers;

    #endregion
    
    #region Private methods
    private void Start()
    {
        if (PersistentTrafficEventManager.Instance != null)
        {
            _targetedCar = PersistentTrafficEventManager.Instance.GetParticipantsCar();
        }

        // todo do a cleanup here
        startTrigger.TargetVehicle(_targetedCar);
        endTrigger.TargetVehicle(_targetedCar);
        
        startTrigger.SetController(this);
        endTrigger.SetController(this);
        
        _restrictedZoneTriggers = GetComponentsInChildren<RestrictedZoneTrigger>();

       DeactivateRestrictedZones();
       EventObjectsActivationSwitch(eventObjectParent);
       
       TurnOffMeshRenderers(consistentEventObjects);
    }
    
    private IEnumerator EndIdleEvent(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _activatedEvent = ExperimentManager.Instance.GetEventActivationState();
        // Debug.Log("<color=red>event is active: </color>" + _activatedEvent);
        if (_activatedEvent)
            ExperimentManager.Instance.ParticipantFailed();
    }
    
    private IEnumerator ActivateTheEvent()
    {
        // Debug.Log("<color=blue>Starting the event process is initiated!</color>");
        float t1 = Time.time;
        _targetedCar.gameObject.GetComponentInChildren<HUD_Advance>().DriverAlert();
        yield return new WaitForSeconds(startEventDelay);
        float t2 = Time.time;
        // Debug.Log("<color=blue>Giving the control to the driver after </color>" + (t2-t1) + "<color=blue> seconds</color>");
        ActivateRestrictedZones();
        eventObjectParent.SetActive(true);
        PersistentTrafficEventManager.Instance.InitiateEvent(eventObjects);
        ExperimentManager.Instance.SetRespawnPositionAndRotation(respawnPoint.transform.position, respawnPoint.transform.rotation);
    }
    
    private IEnumerator DeactivateTheEvent()
    {
        // Debug.Log("<color=red>Deactivating the event is initiated!</color>");
        float t1 = Time.time;
        PersistentTrafficEventManager.Instance.GetParticipantsCar().GetComponentInChildren<HUD_Advance>().DeactivateHUD();
        yield return new WaitForSeconds(endEventDelay);
        float t2 = Time.time;
        // Debug.Log("<color=red>Tacking back the control from the driver after </color>" + (t2-t1) + "<color=red> seconds</color>");
        DeactivateRestrictedZones();
        PersistentTrafficEventManager.Instance.FinalizeEvent();
        if (!eventObjectActive)
            eventObjectParent.SetActive(false);
    }
    
    private void ActivateRestrictedZones()
    {
        foreach (var restrictedZoneTrigger in _restrictedZoneTriggers)
        {
            restrictedZoneTrigger.gameObject.SetActive(true);
            restrictedZoneTrigger.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void DeactivateRestrictedZones()
    {
        foreach (var restrictedZoneTrigger in _restrictedZoneTriggers)
        {
            restrictedZoneTrigger.gameObject.SetActive(false);
        }
    }
    
    private void EventObjectsActivationSwitch(GameObject parent)
    {
        if (eventObjectActive)
            parent.SetActive(true);
        else
            parent.SetActive(false);
    }
    
    #endregion

    #region Public methods
    public void Triggered(bool state)
    {
        _activatedEvent = state;
        
        if (_activatedEvent)
        {
            StartCoroutine(ActivateTheEvent());
        }
        else
        {
            StartCoroutine(DeactivateTheEvent());
        }
        
        StartCoroutine(EndIdleEvent(eventIdleDuration));
    }
    
    public void TurnOffMeshRenderers(GameObject trigger)
    {
        _meshRenderers = trigger.GetComponentsInChildren<MeshRenderer>();

        foreach (var meshRenderer in _meshRenderers)
        {
            meshRenderer.enabled = false;
        }
    }
    
    public void SetEventActivationState(bool state)
    {
        _activatedEvent = state;
    }

    #endregion
}

