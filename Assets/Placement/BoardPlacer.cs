using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class BoardPlacer : MonoBehaviour
{
    public GameObject anchor; // Positions everything
    public GameObject placementIndicator; // Shows where the board will go
    public LevelLoadManager levelLoader;

    public GameObject textBG;
    public GameObject validText; // Text to show when grid can be placed
    public GameObject notValidText; // Text to show when grid can't be placed

    public float baseScale; // The default scale size

    private ARRaycastManager arRaycastManager;
    private Pose placementPose;
    private bool validPlacementPose = false;

    private float rotationOffset = 0f;

    void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        updatePlacementPose();
        UpdatePlacementIndicator();
        updateValidText(validPlacementPose);
        if (shouldPlace())
        {
            /*
            placementIndicator.SetActive(false);
            textBG.SetActive(false);
            levelLoader.transitionToNextLevel(anchor.transform);
            this.enabled = false;
            */
            levelLoader.transitionToNextLevel(anchor.transform.position, anchor.transform.rotation, anchor.transform.localScale);
        }
    }

    private bool shouldPlace()
    {
        bool touched = Input.touchCount > 0;
        bool touchBegan = Input.GetTouch(0).phase == TouchPhase.Began;
        bool notTouchingUi = !EventSystem.current.IsPointerOverGameObject(0);
        return validPlacementPose && touched && touchBegan && notTouchingUi;
    }

    private void updateValidText(bool valid)
    {
        validText.SetActive(valid);
        notValidText.SetActive(!valid);
    }

    private void UpdatePlacementIndicator()
    {
        if (validPlacementPose)
        {
            placementIndicator.SetActive(true);
            anchor.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void updatePlacementPose()
    {
        try
        {
            var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            var hits = new List<ARRaycastHit>();
            arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

            validPlacementPose = hits.Count > 0;
            if (validPlacementPose)
            {
                placementPose = hits[0].pose;

                var cameraForward = Camera.current.transform.forward;
                Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                placementPose.rotation = Quaternion.LookRotation(cameraBearing);
                placementPose.rotation.eulerAngles += new Vector3(0, rotationOffset, 0);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
        
    }

    public void SetRotation(float newRotation)
    {
        rotationOffset = newRotation;
    }

    public void SetScale(float newScale)
    {
        if (0 <= newScale && newScale <= 100)
        {
            anchor.transform.localScale = Vector3.one * baseScale * (newScale / 50f);
        }
        else
        {
            throw new UnityException("Board Scale out of bounds");
        }
    }
}
