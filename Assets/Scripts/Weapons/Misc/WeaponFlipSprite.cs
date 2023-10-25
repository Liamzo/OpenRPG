using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFlipSprite : MonoBehaviour
{
    ItemHandler item;

    // Start is called before the first frame update
    void Start()
    {
        item = GetComponent<ItemHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.eulerAngles.z >= 5.0f && transform.eulerAngles.z <= 175.0f) {
            item.objectHandler.spriteRenderer.transform.localEulerAngles = new Vector3(0.0f, 180.0f, 90.0f);
        } else if (transform.eulerAngles.z <= 355.0f && transform.eulerAngles.z >= 185.0f) {
            item.objectHandler.spriteRenderer.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
        }
    }
}
