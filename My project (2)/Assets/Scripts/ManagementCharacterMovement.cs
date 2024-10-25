using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementCharacterMovement : MonoBehaviour
{
    public CharacterController characterController;
    public CharacterMovementInfo characterMovementInfo;
    public ManagementCharacterCamera managementCharacterCamera;
    private Vector3 currentMovement = new Vector3();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SetMovement();
        SetGravity();
        characterController.Move(currentMovement * Time.deltaTime);
    }
    public void SetMovement()
    {
        managementCharacterCamera.CamDirection();
        Vector3 movement = new Vector3(ManagementCharacterInputs.characterActions.movement.x, 0, ManagementCharacterInputs.characterActions.movement.y);
        movement = Vector3.ClampMagnitude(movement, 1);
        movement = movement.x * characterMovementInfo.camRigth + movement.z * characterMovementInfo.camForward;
        movement *= characterMovementInfo.movementSpeed;
        movement.y = characterMovementInfo.fallFelocity;
        currentMovement = movement;
    }
    public void SetGravity()
    {
        if (characterController.isGrounded)
        {
            characterMovementInfo.fallFelocity = 0;
        }
        else
        {
            characterMovementInfo.fallFelocity -= characterMovementInfo.gravity * Time.deltaTime;
        }
        characterMovementInfo.fallFelocity = Mathf.Clamp(characterMovementInfo.fallFelocity, -10, 10);
    }
    [System.Serializable]
    public class CharacterMovementInfo
    {
        public float gravity = -9.8f;
        public float movementSpeed = 3;
        public float fallFelocity = 0;
        public Vector3 camForward = new Vector3();
        public Vector3 camRigth = new Vector3();
    }
}
