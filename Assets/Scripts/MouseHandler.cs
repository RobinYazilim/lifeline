using UnityEngine;
using UnityEngine.InputSystem;

public class MouseHandler : MonoBehaviour
{
    public InputAction clickAction;
    public TurretType selectedTurretType = TurretType.Basic;
    private int turretIndex = 0;
    private TurretType[] turretTypes;

    private void Awake()
    {
        turretTypes = (TurretType[])System.Enum.GetValues(typeof(TurretType));
    }

    private void OnEnable()
    {
        clickAction.Enable();
        clickAction.performed += OnClickPerformed;
    }

    private void OnDisable()
    {
        clickAction.performed -= OnClickPerformed;
        clickAction.Disable();
    }

    private void Update()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll > 0f)
        {
            turretIndex = (turretIndex + 1) % turretTypes.Length;
            UpdateSelectedTurret();
        }
        else if (scroll < 0f)
        {
            turretIndex--;
            if (turretIndex < 0) turretIndex = turretTypes.Length - 1;
            UpdateSelectedTurret();
        }
    }
    private void UpdateSelectedTurret()
    {
        selectedTurretType = turretTypes[turretIndex];
        // Update your menu/UI here
        TurretMenu.inst.UpdateTurretName(selectedTurretType.ToString());
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        OnMouseClick(mouseWorldPos);
    }

    private void OnMouseClick(Vector3 clickPosition)
    {
        Debug.Log("Mouse clicked at: " + clickPosition);
        GameObject prefab = GameManager.inst.turretPrefabs[turretIndex];
        TurretManager.inst.spawnTurret(clickPosition, selectedTurretType, prefab);
    }
}
