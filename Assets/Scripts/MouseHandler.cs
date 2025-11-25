using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


public class MouseHandler : MonoBehaviour
{

    public InputAction clickAction;
    public TurretType selectedTurretType = TurretType.Basic;
    // private int turretIndex = 0;
    private GameObject ghostPreview;
    public bool turretbought = false; //shop olmadığı için true yaptım sonra false a çevirin 

    public static MouseHandler inst;

    private bool onUi = false;

    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
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
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        onUi = EventSystem.current.IsPointerOverGameObject(-1);

        
        if (ghostPreview != null && turretbought)
        {
            ghostPreview.transform.position = mouseWorldPos;
        }
    }
    
    public void BuyTurret(TurretType type)
    {
        turretbought = true;
        selectedTurretType = type;
        SpawnGhostPreview();
    }

    private void SpawnGhostPreview()
    {
        if (ghostPreview != null)
            Destroy(ghostPreview);

        GameObject prefab = GameManager.inst.turretPrefabs[(int) selectedTurretType];
        
        
        ghostPreview = Instantiate(prefab);
        LineRender lineRender = ghostPreview.GetComponent<LineRender>();
        lineRender.real = false;
        lineRender.setUpRenderer(TurretManager.inst.getTurretData(selectedTurretType).Item3);
        

        SpriteRenderer[] renderers = ghostPreview.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in renderers)
        {
            Color c = sr.color;
            c.a = 0.4f;
            sr.color = c;
        }
    }


    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (onUi)
            return;
 
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;
        
        OnMouseClick(mouseWorldPos);
        
    }

    private void OnMouseClick(Vector3 clickPosition)
    {
        if (turretbought)
        {
            TurretManager.inst.spawnTurret(clickPosition, selectedTurretType);
            ShopManager.inst.showShop();
            turretbought = false;
            Destroy(ghostPreview);
        }
    }
}
