using UnityEngine;
using TMPro;

public class TurretMenu : MonoBehaviour
{
    public static TurretMenu inst;
    public TMP_Text turretNameText;

    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }

    public void UpdateTurretName(string name)
    {
        if (turretNameText != null)
            turretNameText.text = name;
    }
}
