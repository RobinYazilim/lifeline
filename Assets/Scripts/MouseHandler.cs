using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


public class MouseHandler : MonoBehaviour
{

    public InputAction clickAction;
    public InputAction rightClickAction;
    public Turret hoveredTurret;
    public TurretType selectedTurretType = TurretType.Basic;
    // private int turretIndex = 0;
    private GameObject ghostPreview;
    public bool turretbought = false; //shop olmadığı için true yaptım sonra false a çevirin 

    public static MouseHandler inst;
    private Color ogColor;

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
        rightClickAction.Enable();
        rightClickAction.performed += OnRightClickPerformed;
    }

    private void OnDisable()
    {
        clickAction.performed -= OnClickPerformed;
        clickAction.Disable();
        rightClickAction.performed -= OnRightClickPerformed;
        rightClickAction.Disable();
    }

    private void Update()
    {

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        foreach (Turret turret in TurretManager.inst.turrets)
        {
            Collider2D c2d = turret.physical.GetComponent<Collider2D>();
            if (c2d == null)
                continue;
            bool amIOnBro = c2d.OverlapPoint(mouseWorldPos);
            if (amIOnBro)
            {
                // yes you are on this diddy blud enemy papi
                hoveredTurret = turret;
                break;
            }
            else
            {
                hoveredTurret = null;
            }
        }
        

        onUi = EventSystem.current.IsPointerOverGameObject(-1);

        if (ghostPreview != null && turretbought)
        {

            if (TurretManager.inst.turretPosBlocked(mouseWorldPos) || !TurretManager.inst.canSpawnTurret())
            {
                // pathte g˘ruyÚ göster
                SpriteRenderer sr = ghostPreview.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.color = new Color(0.4f, 0.4f, 0.4f, 0.2f);
            }
            else
            {
                // pathte değil 
                SpriteRenderer sr = ghostPreview.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.color = ogColor;
            }
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

        GameObject prefab = GameManager.inst.turretPrefabs[(int)selectedTurretType];
        ogColor = prefab.GetComponent<SpriteRenderer>().color;

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
        if (TurretManager.inst.canSpawnTurret() == false)
            return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        if (TurretManager.inst.turretPosBlocked(mouseWorldPos))
            return;
        

        OnMouseClick(mouseWorldPos);

    }
    private void OnRightClickPerformed(InputAction.CallbackContext context)
    {
        if (onUi)
            return;

        if (hoveredTurret == null)
            return;
        
        TurretManager.inst.deleteTurret(hoveredTurret);
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
