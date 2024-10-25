using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManagementCharacterInputs : MonoBehaviour
{
    public static CharacterInputs characterInputs;
    public static CharacterActions characterActions;

    public void Awake() {
        characterInputs = new CharacterInputs();
        characterActions = new CharacterActions();
    }
    private void OnEnable() {
        characterInputs.Enable();    
    }
    private void OnDisable() {
        characterInputs.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        characterActions.camera = characterInputs.Character.Camera.ReadValue<Vector2>();
        characterActions.movement = characterInputs.Character.Move.ReadValue<Vector2>();
    }
    [System.Serializable]
    public class CharacterActions{
        public Vector2 camera;
        public Vector2 movement;
    }
}
