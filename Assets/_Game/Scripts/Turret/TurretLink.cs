using System;
using UnityEngine;

public class TurretLink : MonoBehaviour
{
    public event Action<bool> OnLinkChanged;

    [SerializeField] Turret linkedTurret;
    TurretLink linkedTurretLink;

    public Turret LinkedTurret => linkedTurret;
    public bool WaitingForDespawn {  get; private set; }
    public bool HasLink => linkedTurret != null;


    public void Link(Turret other)
    {
        if (other == null || HasLink) return;

        linkedTurret = other;
        linkedTurretLink = other.GetComponent<TurretLink>();

        if (linkedTurretLink != null && !linkedTurretLink.HasLink)
            linkedTurretLink.Link(this.GetComponent<Turret>());

        OnLinkChanged?.Invoke(true);
    }

    public void Unlink()
    {
        if (linkedTurretLink != null && linkedTurretLink.HasLink)
            linkedTurretLink.ForceUnlink();
        
        ForceUnlink();
    }

    public void ForceUnlink()
    {
        linkedTurret = null;
        linkedTurretLink = null;
        OnLinkChanged?.Invoke(false);
    }

    public void SetWaitingForDespawn(bool value) => WaitingForDespawn = value;

    public void Clear()
    {
        linkedTurret = null;
        linkedTurretLink = null;
        WaitingForDespawn = false;
        OnLinkChanged?.Invoke(false);
    }
}