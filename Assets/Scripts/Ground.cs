using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] private GameObject _tile;
    [SerializeField] private GameObject _wall;
    [SerializeField] private int _rows = 8;
    [SerializeField] private int _cols = 8;


    void Start()
    {

        for (int row = 0; row < _rows*2; row+=2)
        {
            for (int col = 0; col < _cols*2; col+=2)
            {
                Instantiate(_tile, new Vector3(row, col, 0),Quaternion.identity, transform);
            }
        }
    }

    void Update()
    {
        
    }
}
