using UnityEngine;

public class TurretLink : MonoBehaviour
{
    public Turret linkedTurret;
    public bool waitingForDespawn = false;
    public bool HasLink => linkedTurret != null;

    public void Link(Turret other)
    {
        linkedTurret = other;
        other.GetComponent<TurretLink>().linkedTurret = this.GetComponent<Turret>();
    }

    public void Unlink()
    {
        if (linkedTurret != null)
            linkedTurret.GetComponent<TurretLink>().linkedTurret = null;
        linkedTurret = null;
    }

    public void Reset()
    {
        linkedTurret = null;
        waitingForDespawn = false;
    }
}