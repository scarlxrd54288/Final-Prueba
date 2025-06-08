using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;


    [SerializeField]
    private ObjectDatabaseSO database;
    private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private AudioSource source;

    //private GridData floorData, furnitureData;

    //Aumento----
    //private GridData carTrafficData;
    [SerializeField] private GridData globalGridData;
    public GridData GlobalGridData => globalGridData;

    //private Renderer previewRenderer;

    private List<GameObject> placedGameObject = new();

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;


    private void Awake()
    {
        if (globalGridData == null)
        {
            globalGridData = new GridData();
            Debug.Log("[PlacementSystem] GridData creado en runtime.");
        }
    }




    // Start is called before the first frame update
    void Start()
    {
        StopPlacement();
        //floorData = new();
        //furnitureData = new();
        //Aumento---
        //carTrafficData = new GridData();

        //previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }
        gridVisualization.SetActive(true);
        //cellIndicator.SetActive(true);
        preview.StartShowingPlacementPreview(
            database.objectsData[selectedObjectIndex].Prefab,
            database.objectsData[selectedObjectIndex].Size);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        //cellIndicator.SetActive(false);
        preview.StopShowingPreview();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
    }
    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI())
        {
            return;
        }

        /*Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
            return;
        */

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        var objData = database.objectsData[selectedObjectIndex];
        var type = objData.ID == 0 ? GridObjectType.Floor : GridObjectType.Furniture;

        if (!globalGridData.CanPlaceObjectAt(gridPosition, objData.Size, type))
            return;

        source.Play();

        /*bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
        {
            AudioSource.PlayOneShot(wrongPlaacementClip);
            return;
        }

        source.PlayOneShot(correctPlacementClip);
        */
        //GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        //newObject.transform.position = grid.CellToWorld(gridPosition);
        GameObject newObject = Instantiate(objData.Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);



        /*placedGameObjects.Add( newObject );
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
            floorData;
            furnitureData;
        selectedData AddOcjectAt(gridPosition);*/

        //Aumento----------------Resistencia y Evolución
        Obstacle obstacle = newObject.GetComponent<Obstacle>();
        if (obstacle != null)
        {
            //var objData = database.objectsData[selectedObjectIndex];
            //obstacle.Evolve(0, objData.BaseResistance); // Nivel 0, resistencia base
            obstacle.Evolve(0, objData.BaseResistance);
        }



        placedGameObject.Add(newObject);
        //GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        //selectedData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size, database.objectsData[selectedObjectIndex].ID, placedGameObject.Count - 1);
        
        //var objData = database.objectsData[selectedObjectIndex];
        //var type = objData.ID == 0 ? GridObjectType.Floor : GridObjectType.Furniture;

        globalGridData.AddObjectAt(gridPosition, objData.Size, objData.ID, placedGameObject.Count - 1, type);
            
        preview.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }





    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        //Original
        /*GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;

        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
        */
        var objData = database.objectsData[selectedObjectIndex];
        var type = objData.ID == 0 ? GridObjectType.Floor : GridObjectType.Furniture;

        return globalGridData.CanPlaceObjectAt(gridPosition, objData.Size, type);
         
    }

    //Aumento---------------
    /*public void SetCarTrafficData(GridData data)
    {
        carTrafficData = data;
    }*/


    private void Update()
    {
        if (selectedObjectIndex < 0)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if(lastDetectedPosition != gridPosition)
        {
            bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
            //previewRenderer.material.color = placementValidity ? Color.white : Color.red;

            mouseIndicator.transform.position = mousePosition;
            //cellIndicator.transform.position = grid.CellToWorld(gridPosition);
            preview.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
            lastDetectedPosition = gridPosition;
        }
        
    }

   







}
