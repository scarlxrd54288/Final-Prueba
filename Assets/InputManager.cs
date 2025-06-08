using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class InputManager : MonoBehaviour
{

    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action OnClicked, OnExit;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if(Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();




    public Vector3 GetSelectedMapPosition()
    {
       /*        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayermask))
        {
            return hit.point;
        }
        return Vector3.zero;*/
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane; // Set the z position to the near clip plane
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayermask))
        {
            //return hit.point;
            lastPosition = hit.point; // <- Aquí actualizas la posición
            return hit.point;
        }
        //return lastPosition;
        return lastPosition;
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

  
}
