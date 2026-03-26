using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    [SerializeField] int Desint;
    // Start is called before the first frame update
    void Start()
    {
        if (Desint == 0)
        {
            StartCoroutine(DesTeleport());
        }
    }

    private IEnumerator DesTeleport()
    {
        yield return new WaitForSeconds(0.5f);  // 3√  ¥Î±‚
        Destroy(gameObject);
    }
}
