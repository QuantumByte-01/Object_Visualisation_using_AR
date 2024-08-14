// InputManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.AR;
using TMPro;

public class InputManager : ARBaseGestureInteractable
{
    [SerializeField] private Camera arCam;
    [SerializeField] private ARRaycastManager _raycastManager;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Button toggleButton; 
    [SerializeField] private Button clearButton; 

    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    private Pose pose;
    public bool singlePlacementMode = true;
    private List<GameObject> placedObjects = new List<GameObject>(); 

    public TextMeshProUGUI toggleButtonText; // Corrected this

    void Start()
    {
        toggleButton.onClick.AddListener(TogglePlacementMode);
        clearButton.onClick.AddListener(ClearPlacedObjects);
    }

    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        if(gesture.targetObject == null)
            return true;
        return false;
    }

    protected override void OnEndManipulation(TapGesture gesture)
    {
        if(gesture.isCanceled)
            return;
        if(gesture.targetObject != null || IsPointerOverUI(gesture))
        {
            return;
        }
        if (_raycastManager.Raycast(gesture.startPosition, _hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            pose = _hits[0].pose;
            GameObject placedObj = Instantiate(DataHandler.Instance.GetFurniture(),pose.position,pose.rotation);

            var anchorObject = new GameObject("PlacementAnchor");
            anchorObject.transform.position = pose.position;
            anchorObject.transform.rotation = pose.rotation;
            placedObj.transform.parent = anchorObject.transform; 

            placedObjects.Add(placedObj); 

            if (singlePlacementMode)
            {
                this.enabled = false;
            }
        }
    }

    void FixedUpdate()
    {
        CrosshairCalculation();
    }

    bool IsPointerOverUI(TapGesture touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touch.startPosition.x, touch.startPosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    void CrosshairCalculation()
    {
        Vector3 origin = arCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
        
        if (_raycastManager.Raycast(origin, _hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            pose = _hits[0].pose;
            crosshair.transform.position = pose.position;
            crosshair.transform.eulerAngles = new Vector3(90,0,0);
        }
    }

    public void TogglePlacementMode()
    {
        singlePlacementMode = !singlePlacementMode;

        if (singlePlacementMode)
        {
            this.enabled = true;
            toggleButtonText.text = "Place"; 
            toggleButton.GetComponent<Image>().color = Color.white;
        }
        else
        {
            toggleButtonText.text = "Multiple"; 
            toggleButton.GetComponent<Image>().color = Color.green;
        }
    } // Added this

    public void ClearPlacedObjects()
    {
        foreach (GameObject obj in placedObjects)
        {
            Destroy(obj);
        }
        placedObjects.Clear();
    }
}
