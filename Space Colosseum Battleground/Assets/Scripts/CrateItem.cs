using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateItem : MonoBehaviour
{
    public int heal;
    public void DeleteCrateItem()
    {
        Destroy(gameObject);
    }
}
