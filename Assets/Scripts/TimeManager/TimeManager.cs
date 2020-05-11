﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    
    public static TimeManager Instance { get; private set; }
    
    private bool _experimentStarted;

    private float _timeSinceStart;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);       
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        _timeSinceStart = 0f;
    }

    private void FixedUpdate()
    {
        //maybe A coroutine is better
        
        if (_experimentStarted)
        {
            _timeSinceStart += Time.deltaTime;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _experimentStarted=true;
        }
    }
    
    public double GetCurrentUnixTimeStamp()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        return (System.DateTime.UtcNow - epochStart).TotalSeconds;
    }

    private double GetTimeSinceStartUp()
    {
        //TODO this is not smart, it should count the time from the experiment beginning. needs a workaround.
       // return Time.realtimeSinceStartup;

        
       return _timeSinceStart;
    }
    
    
}