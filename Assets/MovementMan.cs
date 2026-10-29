using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.SceneManagement;

public class MovementMan : MonoBehaviour
{
    public int numberOfButtons;
    private int numberOFButtonsPressed;

    private Image image;
    
    [Tooltip("Reference to the Teleportation Provider on your XR Rig / XR Origin.")]
    public TeleportationProvider teleportationProvider;

    [Tooltip("The Transform of the target location you want the player to teleport to.")]
    public Transform teleportDestination;


    public void OnButtonPressed()
    {
        numberOFButtonsPressed++;

        if (numberOfButtons <= numberOFButtonsPressed)
        {
            image.enabled = true;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = this.GetComponentInChildren<Image>();
    }

    public void OnMovementButtonPressed()
    {
        if (teleportationProvider == null)
        {
            Debug.LogWarning("Teleportation Provider or Destination is missing.");
            return;
        }

        if(teleportDestination == null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // Create a teleport request
        TeleportRequest request = new TeleportRequest()
        {
            destinationPosition = teleportDestination.position,
            destinationRotation = teleportDestination.rotation,
            // Match the rotation of the destination object
            matchOrientation = MatchOrientation.TargetUpAndForward 
        };

        // Send the request to the Locomotion System
        teleportationProvider.QueueTeleportRequest(request);
    }
}
