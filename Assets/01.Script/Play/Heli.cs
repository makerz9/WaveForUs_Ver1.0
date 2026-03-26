using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heli : MonoBehaviour
{
    [SerializeField] private GameObject[] ItemBox;
    [SerializeField] private Transform HeliPot;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        Invoke("Return", 10);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * 12 * Time.deltaTime);
    }

    void ItemDrop()
    {
        int items = Random.Range(0, ItemBox.Length);

        Instantiate(ItemBox[items], HeliPot.position, Quaternion.identity);


    }

    void Return()
    {
        gameManager.SoundCall("heli");
        transform.position = new Vector3(35, transform.position.y, transform.position.z);
        int returns = Random.Range(25, 30);
        int times = Random.Range(1, 3);

        Invoke("Return", returns);
        Invoke("ItemDrop", times);

    }
}
