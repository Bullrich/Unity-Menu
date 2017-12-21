using Blue.Menu;
using UnityEngine;

public class TestingPrefab : MonoBehaviour
{
    public MenuController menu;

    // Use this for initialization
    void Start()
    {
        Instantiate(menu.gameObject);
    }
}