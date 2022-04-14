using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    public float xOffset, zOffset;
    private float nextZRotation = 90f;
    private GameObject clickedTile;
    
    private int hitCount;
    public int shipSize;

    private Material[] materials;

    private readonly List<GameObject> touchTiles = new List<GameObject>();
    private readonly List<Color> colors = new List<Color>();

    private void Start()
    {
        materials = GetComponent<Renderer>().materials;
        
        foreach (var material in materials)
            colors.Add(material.color);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tile"))
        {
            touchTiles.Add(collision.gameObject);
        }
    }

    public void ClearTileList()
    {
        touchTiles.Clear();
    }

    public Vector3 GetOffsetVec(Vector3 tilePos)
    {
        return new Vector3(tilePos.x + xOffset, 2, tilePos.z + zOffset);
    }

    public void RotateShip()
    {
        if (clickedTile == null) return;
        touchTiles.Clear();
        
        transform.localEulerAngles += new Vector3(0, 0, nextZRotation);
        nextZRotation *= -1;
        
        (xOffset, zOffset) = (zOffset, xOffset);
        SetPosition(clickedTile.transform.position);
    }

    private void SetPosition(Vector3 newVec)
    {
        ClearTileList();
        transform.localPosition = new Vector3(newVec.x + xOffset, 2, newVec.z + zOffset);
    }

    public void SetClickedTile(GameObject tile)
    {
        clickedTile = tile;
    }

    public bool OnGameBoard()
    {
        return touchTiles.Count == shipSize;
    }

    public bool HitCheckSank()
    {
        hitCount++;
        return shipSize <= hitCount;
    }

    public void FlashColor(Color tempColor)
    {
        foreach (var mat in materials)
            mat.color = tempColor;
        
        Invoke(nameof(ResetColor), 0.5f);
    }

    private void ResetColor()
    {
        var i = 0;
        foreach (var mat in materials)
            mat.color = colors[i++];
    }
}
