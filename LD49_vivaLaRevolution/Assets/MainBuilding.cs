
using UnityEngine;

public class MainBuilding : Building
{

    override protected void Awake()
    {
        base.Awake();
        lootable = true;
    }
}
