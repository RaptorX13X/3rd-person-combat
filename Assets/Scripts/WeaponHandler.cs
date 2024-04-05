using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponHandler : MonoBehaviour
{

    [SerializeField] private GameObject weaponObject;

    public void EnableWeapon()
    {
        weaponObject.SetActive(true);
    }

    public void DisableWeapon()
    {
        weaponObject.SetActive(false);
    }
}
