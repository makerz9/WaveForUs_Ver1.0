using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMakerMove : MonoBehaviour
{
    [SerializeField] private int moveSpeed;
    public float posPower;

    [SerializeField] private string PowerType = "";

    [SerializeField] private float DisTime = 10;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(DeactivateObject), DisTime);

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

        if(PowerType == "Item2")
        {
            posPower += 14f * Time.deltaTime;
        }
    }

    
    void DeactivateObject()
    {
        gameObject.SetActive(false);
    }
}
