using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleCooldownIcon : MonoBehaviour
{
    public int obstacleID; // ID del obstáculo asignado en Inspector
    private Image buttonImage;
    private Button button;

    private Color availableColor = Color.white;
    private Color unavailableColor = Color.gray;
    public ObjectDatabaseSO objectDatabase;


    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    public void UpdateCooldown()
    {
        ObjectData objData = objectDatabase.objectsData.Find(o => o.ID == obstacleID);
        if (objData == null)
            return;

        //  Si no está desbloqueado, botón desactivado y gris
        if (!objData.Unlocked)
        {
            button.interactable = false;
            buttonImage.color = unavailableColor;
            return;
        }

        float cooldownRatio = Mathf.Clamp01(objData.CooldownTimer / objData.CooldownTime);
        Color currentColor = Color.Lerp(availableColor, unavailableColor, cooldownRatio);
        buttonImage.color = currentColor;
        button.interactable = cooldownRatio == 0;
    }


}
