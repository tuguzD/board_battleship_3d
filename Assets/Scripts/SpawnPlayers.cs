using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public float minX, maxX, minY, maxY, minZ, maxZ;

    private void Start()
    {
        var x = Random.Range(minX, maxX);
        var y = Random.Range(minY, maxY);
        var z = Random.Range(minZ, maxZ);
        
        PhotonNetwork.Instantiate(playerPrefab.name, 
            new Vector3(x, y, z), Quaternion.identity);
    }
}
