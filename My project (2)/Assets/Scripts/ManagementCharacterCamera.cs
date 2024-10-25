using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ManagementCharacterCamera : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public ManagementCharacterMovement managementCharacterMovement;
    public Vector2 speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (ManagementCharacterInputs.characterActions.camera != Vector2.zero)
        {            
            CinemachinePOV pov = cinemachineVirtualCamera.GetCinemachineComponent<CinemachinePOV>();
            float verticalValue = ManagementCharacterInputs.characterActions.camera.y * speed.y * Time.deltaTime;
            verticalValue = Mathf.Clamp(verticalValue, -40, 40);
            pov.m_HorizontalAxis.Value += ManagementCharacterInputs.characterActions.camera.x * speed.x * Time.deltaTime;
            pov.m_VerticalAxis.Value += verticalValue;
        }
    }
    public void Update()
    {
        
    }
    public void CamDirection()
    {
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRigth = Camera.main.transform.right;
        camForward.y = 0;
        camRigth.y = 0;
        managementCharacterMovement.characterMovementInfo.camForward = camForward.normalized;
        managementCharacterMovement.characterMovementInfo.camRigth = camRigth.normalized;
    }
}
