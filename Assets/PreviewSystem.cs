using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]// Start is called before the first frame update
    private float previewYOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialsPrefab;
    private Material previewMaterialInstaance;

    private Renderer cellIndicatorRenderer;

    private void Start()
    {
        previewMaterialInstaance = new Material(previewMaterialsPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();

    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if(size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicator.GetComponentInChildren<Renderer>().material.mainTextureScale = size;
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for( int i = 0; i < materials.Length; i++ )
            {
                materials[i] = previewMaterialInstaance;
            }
            renderer.materials = materials;
        }
        DisableAllColliders(previewObject);

    }

    private void DisableAllColliders(GameObject go)
    {
        foreach (var collider in go.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
    }


    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false );
        Destroy( previewObject );
    }
    
    public void UpdatePosition(Vector3 position, bool validity)
    {
        MovePreview(position);
        MoveCursor(position);
        ApplyFeedback(validity);
    }

    private void ApplyFeedback(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
        previewMaterialInstaance.color = c;
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }

    private void MovePreview(Vector3 position)
    {
        if (previewObject != null)
        {
            previewObject.transform.position = new Vector3(position.x,
                position.y + previewYOffset,
                position.z);
        }
        else
        {
            Debug.LogWarning("[PreviewSystem] previewObject ya fue destruido.");
        }
    }

}
