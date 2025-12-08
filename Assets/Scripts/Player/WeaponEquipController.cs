using UnityEngine;

public class WeaponEquipController : MonoBehaviour
{
    public static WeaponEquipController instance;

    [SerializeField] private bool isWeaponEquiped = false;
    [SerializeField] private Transform playerGrip;
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private GameObject WeaponPrefab;
    private void Awake()
    {
        instance = this;
    }

    private void SetISWeaponEquipped(bool isEquiped) 
    {
        isWeaponEquiped = isEquiped;
    }
    
    public void EquipWeapon() 
    {
        // set weapon as a child of player grip transform
        WeaponPrefab.transform.SetParent(playerGrip);

        // Reset local position/rotation so it fits exactly into the grip
        WeaponPrefab.transform.localPosition = Vector3.zero;
        WeaponPrefab.transform.localRotation = Quaternion.identity;

        SetISWeaponEquipped(true);
    }

    public void UnEquipWeapon()
    {
        // set weapon as a child of wweapon holder transform
        WeaponPrefab.transform.SetParent(weaponHolder);

        // Reset local position/rotation so it fits exactly into the grip
        WeaponPrefab.transform.localPosition = Vector3.zero;
        WeaponPrefab.transform.localRotation = Quaternion.identity;

        SetISWeaponEquipped(false);
    }

    public bool IsWeaponEquiped() 
    {
        return isWeaponEquiped; 
    }
}
