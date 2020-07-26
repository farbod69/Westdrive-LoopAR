﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class CalibrationManager : MonoBehaviour
{
    #region Fields

    public static CalibrationManager Instance { get; private set; }
    
    private bool _wasMainMenuLoaded;
    private bool _uUIDGenerated;
    private bool _eyeTrackerCalibrationSuccessful;
    private bool _eyeTrackerValidationSuccessful;
    private bool _seatCalibrationSuccessful;
    private bool _testDriveSuccessful;
    
    private CalibrationData _calibrationData;
    private String _calibrationFilePath;

    #endregion

    #region PrivateMethods

    private void Awake()
    {
        _calibrationFilePath = GetPathForSaveFile("CalibrationData");

        if (File.Exists(_calibrationFilePath))
        {
            _calibrationData = LoadCalibrationFile(_calibrationFilePath);
        }
        else
        {
            _calibrationData = new CalibrationData();
        }
        
        //singleton pattern a la Unity
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
    
    private void StoreParticipantUuid(string iD)
    {
        _calibrationData.ParticipantUuid = iD;
        SaveCalibrationData();
    }
    
    private void DeleteCalibrationFile(string dataPath)
    {
        if(!File.Exists(dataPath))
        {
            Debug.Log("File not found, can not be deleted!");
        }
        else
        {
            File.Delete(dataPath);
        }
    }

    private void SaveCalibrationFile(CalibrationData calibrationData)
    {
        string jsonString = JsonUtility.ToJson(calibrationData);
        File.WriteAllText(_calibrationFilePath, jsonString);
    }
    
    private string GetPathForSaveFile(string saveFileName)
    {
        return Path.Combine(Application.persistentDataPath, saveFileName + ".txt");
    }
    
    private CalibrationData LoadCalibrationFile(string dataPath)
    {
        string jsonString;
        if(!File.Exists(dataPath))
        {
            Debug.Log("File not found!");
            return null;
        }
        else
        {
            Debug.Log("Found Calibration Data, loading...");
            jsonString = File.ReadAllText(dataPath);
            //Debug.Log(jsonString);
            return JsonUtility.FromJson<CalibrationData>(jsonString);
        }
    }

    #endregion

    #region PublicMethods

    public void GenerateID()
    { 
        string newParticipantId = System.Guid.NewGuid().ToString();
        StoreParticipantUuid(newParticipantId);
        _uUIDGenerated = true;
    }
    
    public void EyeCalibration()
    {
        EyetrackingManager.Instance.StartCalibration();
    }

    public void EyeCalibrationSuccessful()
    {
        _eyeTrackerCalibrationSuccessful = true;
    }
    
    public void EyeValidation()
    {
        SceneManager.LoadSceneAsync("EyetrackingValidation");
    }

    public void EyeValidationSuccessful()
    {
        _eyeTrackerValidationSuccessful = true;
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void SeatCalibration()
    {
        SceneLoadingHandler.Instance.SceneChange("SeatCalibrationScene");
        // SceneManager.LoadSceneAsync("SeatCalibrationScene");
    }

    public void SeatCalibrationSuccessful()
    {
        _seatCalibrationSuccessful = true;
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void StartTestDrive()
    {
        SceneLoadingHandler.Instance.SceneChange("TestDrive2.0");
        SceneManager.LoadSceneAsync("TestDrive2.0");
    }
    
    public void TestDriveSuccessState(bool state, int trials)
    {
        _testDriveSuccessful = state;
        // todo serialize the info
    }

    /*public void TestDriveFailed()
    {
        // todo save the failed data onto the calibration data
    }*/

    public void TestDriveEnded()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
    
    public void AbortExperiment()
    {
        SceneManager.LoadSceneAsync("MainMenu");
        MainMenu.Instance.ReStartMainMenu();
    }
    
    public void StoreSeatCalibrationData(Vector3 seatOffset)
    {
        _calibrationData.SeatCalibrationOffset = seatOffset;
        // StoreVRState(true);
        SaveCalibrationData();
    }
    
    public void StoreValidationErrorData(Vector3 validationError)
    {
        _calibrationData.EyeValidationError = validationError;
        SaveCalibrationData();
    }

    public void StoreVRState(bool vRMode)
    {
        _calibrationData.VRmode = vRMode;
        _wasMainMenuLoaded = true;
        SaveCalibrationData();
    }

    public void SaveCalibrationData()
    {
        SaveCalibrationFile(_calibrationData);
    }
    
    public void DeleteCalibrationData()
    {
        DeleteCalibrationFile(_calibrationFilePath);
    }

    #endregion
    
    #region Setters

    public void SetCameraMode(bool vrModeState)
    {
        if (vrModeState)
        {
            CameraManager.Instance.VRModeCameraSetUp();
        }
        else
        {
            CameraManager.Instance.NonVRModeCameraSetUp();
        }
    }

    #endregion
    
    #region Getters
    
    public bool GetWasMainMenuLoaded()
    {
        return _wasMainMenuLoaded;
    }

    public bool GetParticipantUUIDState()
    {
        return _uUIDGenerated;
    }

    public bool GetEyeTrackerCalibrationState()
    {
        return _eyeTrackerCalibrationSuccessful;
    }

    public bool GetEyeTrackerValidationState()
    {
        return _eyeTrackerValidationSuccessful;
    }

    public bool GetSeatCalibrationState()
    {
        return _seatCalibrationSuccessful;
    }

    public bool GetTestDriveState()
    {
        return _testDriveSuccessful;
    }

    public Vector3 GetSeatCalibrationOffset()
    {
        return _calibrationData.SeatCalibrationOffset;
    }

    public Vector3 GetValidationError()
    {
        return _calibrationData.EyeValidationError;
    }

    private bool GetVRModeState()
    {
        return _calibrationData.VRmode;
    }
        
    public bool GetVRActivationState()
    {
        return GetVRModeState();
    }
    
    #endregion
}
