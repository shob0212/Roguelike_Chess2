using UnityEngine;

public class Ground : MonoBehaviour
{
    public static Ground Instance { get; private set; }

    private System.Random rdm = new System.Random();
    [SerializeField] private GameObject block;
    [SerializeField] private GameObject _tileWhite;
    [SerializeField] private GameObject _tileBlack;
    //[SerializeField] private GameObject _wall;
    [SerializeField] private int _rows = 8;
    [SerializeField] private int _cols = 8;
    private GameObject tile;
    public RectTransform canvas;


    void Awake()
    {
        // インスタンスが存在しない場合は設定し、存在する場合は破棄する
        if (Instance == null){
            Instance = this;
        }else{
            Destroy(gameObject);
        }
    }


    public void SetTile()
    {
    
        for (int row = 0; row < _rows*2; row+=2)
        {
            for (int col = 0; col < _cols*2; col+=2)
            {
                if(row == GameManager.Instance.block1_1 && col == GameManager.Instance.block1_2){
                    Instantiate(block, new Vector3(GameManager.Instance.block1_1, GameManager.Instance.block1_2, 0),Quaternion.identity, transform);
                }else if(row == GameManager.Instance.block2_1 && col == GameManager.Instance.block2_2){
                    Instantiate(block, new Vector3(GameManager.Instance.block2_1, GameManager.Instance.block2_2, 0),Quaternion.identity, transform);
                }else{
                    if(row % 4 == 0) 
                    {
                        if(col % 4 == 0)
                        {
                            tile = Instantiate(_tileWhite, new Vector3(row, col, 0),Quaternion.identity, transform);
                        }
                        else
                        {
                            tile = Instantiate(_tileBlack, new Vector3(row, col, 0),Quaternion.identity, transform);
                        }
                    }
                    else
                    {
                        if(col % 4 == 0)
                        {
                            tile = Instantiate(_tileBlack, new Vector3(row, col, 0),Quaternion.identity, transform);
                        }
                        else
                        {
                            tile = Instantiate(_tileWhite, new Vector3(row, col, 0),Quaternion.identity, transform);
                        }
                    }
                    GameManager.Instance.allTiles.Add(tile.GetComponent<Tile>());
                }
            }
        }
    }

    
}
