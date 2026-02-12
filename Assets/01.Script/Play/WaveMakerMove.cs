using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMakerMove : MonoBehaviour
{
    [SerializeField] private int moveSpeed;
    public float posPower;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(DeactivateObject), 10f);

    }

    public void SetPosPower(float power)
    {
        posPower = power;
        Debug.Log($"WaveMaker posPower ¼³Á¤: {posPower}");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }

    
    void DeactivateObject()
    {
        gameObject.SetActive(false);
    }
}
