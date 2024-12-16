using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] private GameObject _tile;
    [SerializeField] private GameObject _wall;
    [SerializeField] private int _rows = 8;
    [SerializeField] private int _cols = 8;

    // Startは、MonoBehaviourが作成された後、Updateが最初に実行される前に一度だけ呼び出されます。

    void Start()
    {

        Instantiate(_wall, new Vector3((float)-1.15, 7, 0),Quaternion.AngleAxis(90, Vector3.forward), transform);
        Instantiate(_wall, new Vector3((float)15.15, 7, 0),Quaternion.AngleAxis(90, Vector3.forward), transform);
        Instantiate(_wall, new Vector3(7, (float)-1.15, 0),Quaternion.identity, transform);
        Instantiate(_wall, new Vector3(7, (float)15.15, 0),Quaternion.identity, transform);


        for (int row = 0; row < _rows*2; row+=2)
        {
            for (int col = 0; col < _cols*2; col+=2)
            {
                Instantiate(_tile, new Vector3(row, col, 0),Quaternion.identity, transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
