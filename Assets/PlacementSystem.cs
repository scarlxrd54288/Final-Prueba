using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    //Esfera--------
    [SerializeField]
    private GameObject mouseIndicator;
    [SerializeField]
    //Script posicionar--------------
    private InputManager inputManager;
    //Grilla--------
    [SerializeField]
    private Grid grid;

    //ScriptableObject de los obstaculos
    [SerializeField]
    private ObjectDatabaseSO database;
    //ID del objeto seleccionado
    private int selectedObjectIndex = -1;
    //Material Grilla
    [SerializeField]
    private GameObject gridVisualization;

    //Script del GRidData - solo lectura
    [SerializeField] private GridData globalGridData;
    public GridData GlobalGridData => globalGridData;
    //Lista de objetos que se utilizan en la escena--
    private List<GameObject> placedGameObject = new();
    //Script de preview
    [SerializeField]
    private PreviewSystem preview;
    //Ültima posicion detectada
    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    //Indicador de celda
    [SerializeField]
    private GameObject cellIndicator;
    //Npcs
    [SerializeField] private NPCDatabaseSO npcDatabase;

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
        cellIndicator.SetActive(true);
        preview.StartShowingPlacementPreview(
            database.objectsData[selectedObjectIndex].Prefab,
            database.objectsData[selectedObjectIndex].Size);
        inputManager.OnClicked += PlaceStructure;//Mientras
        inputManager.OnExit += StopPlacement;
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        preview.StopShowingPreview();
        inputManager.OnClicked -= PlaceStructure;//Mientras
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
    }

    private void PlaceStructure()
    {
        //VAlidaciones--------------------
        if (inputManager.IsPointerOverUI())
            return;

        if (selectedObjectIndex < 0)
            return;

        var objData = database.objectsData[selectedObjectIndex];
        var type = GridObjectType.Obstacle;


        // Verificar si el objeto está desbloqueado antes de colocarlo
        if (!objData.Unlocked || objData.CooldownTimer > 0f)
        {
            Debug.Log($"[PlacementSystem] Objeto bloqueado o en cooldown: {objData.Name}");
            AudioManager.Instance.PlayPlaceErrorSound();
            StopPlacement();              
            preview.StopShowingPreview(); 
            return;
        }


        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

       
        if (!globalGridData.CanPlaceObjectAt(gridPosition, objData.Size, type))
        {
            AudioManager.Instance.PlayPlaceErrorSound(); 
            return;
        }


        AudioManager.Instance.PlayPlaceObjectSound();


        GameObject prefabToUse = objData.Evolved && objData.EvolvedPrefab != null
            ? objData.EvolvedPrefab
            : objData.Prefab;

        GameObject newObject = ObjectPoolManager.Instance.GetObject(prefabToUse, objData.ID);
        //Spawn
        newObject.transform.position = grid.CellToWorld(gridPosition);
        //Animación
        Animator objAnimator = newObject.GetComponent<Animator>();
        if (objAnimator != null)
        {
            objAnimator.SetTrigger("Entry");
        }
        // Animacion y spawn npc

        // Spawn NPC desde el NPCPoolManager
        NPCData npcData = npcDatabase.npcList.Find(n => n.ID == objData.ID);
        if (npcData != null)
        {
            GameObject npcObject = NPCPoolManager.Instance.GetNPC(npcData.ID);
            npcObject.transform.position = grid.CellToWorld(gridPosition);

            NPC npc = npcObject.GetComponent<NPC>();
            if (npc != null)
                npc.PlayEntry(npcData.Lifetime);
        }



        //Inicia cooldown---------------------------
        objData.CooldownTimer = objData.CooldownTime;

        //Configura resistencia--------------------------------
        Obstacle obstacle = newObject.GetComponent<Obstacle>();
        if (obstacle != null)
        {
            obstacle.SetPoolID(objData.ID);
            float resistance = objData.Evolved ? objData.EvolvedResistance : objData.BaseResistance;
            float damage = objData.Evolved ? objData.EvolvedDamage : 0f;
            obstacle.Evolve(objData.Evolved ? 1 : 0, resistance, damage, objData.Durability);
        }

        placedGameObject.Add(newObject);

        globalGridData.AddObjectAt(
            gridPosition,
            objData.Size,
            objData.ID,
            placedGameObject.Count - 1,
            type
        );

        preview.UpdatePosition(grid.CellToWorld(gridPosition), false);
        //Cool Down------------------
        preview.StopShowingPreview();
        StopPlacement();

    }


    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        var objData = database.objectsData[selectedObjectIndex];
        var type = GridObjectType.Obstacle;
        return globalGridData.CanPlaceObjectAt(gridPosition, objData.Size, type);
         
    }



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
