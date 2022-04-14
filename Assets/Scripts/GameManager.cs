using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Ships")]
    public GameObject[] ships;
    public EnemyScript enemyScript;
    private ShipScript shipScript;
    private List<int[]> enemyShips;
    private int shipIndex;
    public List<TileScript> allTileScripts;    

    [Header("HUD")]
    public Button nextBtn, rotateBtn, replayBtn;
    public Text topText, playerShipText, enemyShipText;

    [Header("Objects")]
    public GameObject missilePrefab, firePrefab, woodDock;

    private bool setupComplete,
        playerTurn = true;
    
    private readonly List<GameObject> 
        playerFires = new List<GameObject>(),
        enemyFires = new List<GameObject>();
    
    private int 
        enemyShipCount = 5,
        playerShipCount = 5;

    private readonly Color32
        green = new Color32(38, 57, 76, 255),
        brown = new Color32(68, 0, 0, 255);

    private void Start()
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        
        nextBtn.onClick.AddListener(NextShipClicked);
        rotateBtn.onClick.AddListener(RotateClicked);
        replayBtn.onClick.AddListener(ReplayClicked);
        
        enemyShips = EnemyScript.PlaceEnemyShips();
    }

    private void NextShipClicked()
    {
        if (!shipScript.OnGameBoard())
            shipScript.FlashColor(Color.red);
        else
        {
            if (shipIndex <= ships.Length - 2)
            {
                shipIndex++;
                shipScript = ships[shipIndex].GetComponent<ShipScript>();
                shipScript.FlashColor(Color.yellow);
            }
            else
            {
                rotateBtn.gameObject.SetActive(false);
                nextBtn.gameObject.SetActive(false);
                woodDock.SetActive(false);
                
                topText.text = "Guess an enemy tile.";
                setupComplete = true;
                
                foreach (var ship in ships)
                    ship.SetActive(false);
            }
        }
    }

    public void TileClicked(GameObject tile)
    {
        switch (setupComplete)
        {
            case true when playerTurn:
                var tilePos = tile.transform.position;
                tilePos.y += 15;
                playerTurn = false;
                Instantiate(missilePrefab, tilePos, missilePrefab.transform.rotation);
                break;
            
            case false:
                PlaceShip(tile);
                shipScript.SetClickedTile(tile);
                break;
        }
    }

    private void PlaceShip(GameObject tile)
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipScript.ClearTileList();
        
        var newVec = shipScript.GetOffsetVec(tile.transform.position);
        ships[shipIndex].transform.localPosition = newVec;
    }

    private void RotateClicked()
    {
        shipScript.RotateShip();
    }

    public void CheckHit(GameObject tile)
    {
        var tileNum = int.Parse(Regex.Match(tile.name, @"\d+").Value);
        var hitCount = 0;
        foreach (var tileNumArray in enemyShips)
        {
            if (!tileNumArray.Contains(tileNum)) continue;
            
            for (var i = 0; i < tileNumArray.Length; i++)
            {
                switch (tileNumArray[i])
                {
                    case -5:
                        hitCount++;
                        break;

                    case var n when n == tileNum:
                        tileNumArray[i] = -5;
                        hitCount++;
                        break;
                }
            }

            switch (hitCount)
            {
                case var n when n == tileNumArray.Length:
                    enemyShipCount--;
                    topText.text = "SUNK!!!!!!";
                    enemyFires.Add(Instantiate(firePrefab, tile.transform.position, Quaternion.identity));
                    tile.GetComponent<TileScript>().SetTileColor(1, brown);
                    tile.GetComponent<TileScript>().SwitchColors(1);
                    break;
                    
                default:
                    topText.text = "HIT!!";
                    tile.GetComponent<TileScript>().SetTileColor(1, Color.red);
                    tile.GetComponent<TileScript>().SwitchColors(1);
                    break;
            }
            break;
        }
        
        if (hitCount == 0)
        {
            tile.GetComponent<TileScript>().SetTileColor(1, green);
            tile.GetComponent<TileScript>().SwitchColors(1);
            topText.text = "Missed, there is no ship there.";
        }
        Invoke(nameof(EndPlayerTurn), 1.0f);
    }

    public void EnemyHitPlayer(Vector3 tile, int tileNum, GameObject hitObj)
    {
        enemyScript.MissileHit(tileNum);
        tile.y += 0.2f;
        playerFires.Add(Instantiate(firePrefab, tile, Quaternion.identity));
        
        if (hitObj.GetComponent<ShipScript>().HitCheckSank())
        {
            playerShipCount--;
            playerShipText.text = playerShipCount.ToString();
            enemyScript.SunkPlayer();
        }
        Invoke(nameof(EndEnemyTurn), 2.0f);
    }

    private void EndPlayerTurn()
    {
        SetActive(true);
        enemyShipText.text = enemyShipCount.ToString();
        topText.text = "Enemy's turn";
        
        enemyScript.NpcTurn();
        ColorAllTiles(0);
        
        if (playerShipCount < 1) 
            GameOver("ENEMY WINs!!!");
    }

    public void EndEnemyTurn()
    {
        SetActive(false);
        playerShipText.text = playerShipCount.ToString();
        topText.text = "Select a tile";
        
        playerTurn = true;
        ColorAllTiles(1);
        
        if (enemyShipCount < 1) 
            GameOver("YOU WIN!!!");
    }

    private void SetActive(bool isForPlayer)
    {
        foreach (var ship in ships)
            ship.SetActive(isForPlayer);
        
        foreach (var fire in playerFires) 
            fire.SetActive(isForPlayer);
        foreach (var fire in enemyFires) 
            fire.SetActive(!isForPlayer);
    }

    private void ColorAllTiles(int colorIndex)
    {
        foreach (var tileScript in allTileScripts)
            tileScript.SwitchColors(colorIndex);
    }

    private void GameOver(string winner)
    {
        topText.text = "Game Over: " + winner;
        replayBtn.gameObject.SetActive(true);
        playerTurn = false;
    }

    private static void ReplayClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
