﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MountainRoadManager : MonoBehaviour
{
    public static MountainRoadManager Instance { get; private set; }

    [SerializeField] private GameObject participantsCar;
    [SerializeField] private GameObject terrain;
    [SerializeField] private GameObject roadNetwork;
    [SerializeField] private GameObject remainingAssets;

    private GameObject[] _sceneAssets;
    private bool _activateObjects;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _sceneAssets = new[] {terrain, roadNetwork, remainingAssets};
    }

    public void ActivateGameObjects(bool activationState)
    {
        _activateObjects = activationState;
        StartCoroutine(ActivateAssets());
    }
    
    IEnumerator ActivateAssets()
    {
        foreach (var asset in _sceneAssets)
        {
            yield return ActivateEachGameObject(asset);
        }    
    }

    IEnumerator ActivateEachGameObject(GameObject obj)
    {
        yield return null;
        obj.SetActive(true);
    }

    public GameObject GetParticipantsCar()
    {
        return participantsCar != null ? participantsCar : null;
    }
    
    public GameObject GetTerrain()
    {
        return terrain;
    }
    
    public GameObject GetRoadNetwork()
    {
        return roadNetwork;
    }
    
    public GameObject GetRemainingAssets()
    {
        return remainingAssets;
    }
}
