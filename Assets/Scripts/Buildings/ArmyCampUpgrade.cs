using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyCampUpgrade : BuildingUpgrade
{
    [SerializeField] private List<UnitData> unlockableUnits;
    private List<UnitData> unlockedUnits = new List<UnitData>();
    [SerializeField] private UnitSpawner buttonSpawner;

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        UnlockUnit(CurrentLevel);
    }

    private void UnlockUnit(int level)
    {

        int unitIndex = level - 1;
        if (unitIndex < unlockableUnits.Count)
        {
            UnitData unit = unlockableUnits[unitIndex];
            if (!unlockedUnits.Contains(unit))
            {
                unlockedUnits.Add(unit);
                NotificationUI.Instance.ShowMessage($"{unit.name} unlocked!", Color.green);
                buttonSpawner.ActivateUnitButton(unit);
            }
        }
    }

    public List<UnitData> GetUnlockedUnits()
    {
        return unlockedUnits;
    }
}
