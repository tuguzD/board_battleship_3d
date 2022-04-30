using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    public float xOffset, zOffset;
    private float nextZRotation = 90f;
    private GameObject clickedTile;

    private Outline outline;
    private readonly Color32 outlineColor = new Color32(255, 130, 50, 255);

    private int hitCount;
    public int shipSize;

    private readonly List<GameObject> touchTiles = new List<GameObject>();

    private void Start()
    {
        outline = GetComponent<Outline>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tile"))
        {
            touchTiles.Add(other.gameObject);
        }
    }

    private void OnMouseEnter()
    {
        outline.enabled = true;
    }

    private void OnMouseExit()
    {
        outline.enabled = false;
    }

    public void ClearTileList()
    {
        touchTiles.Clear();
    }

    public Vector3 GetOffsetVec(Vector3 tilePos)
    {
        return new Vector3(tilePos.x + xOffset, tilePos.y + 4, tilePos.z + zOffset);
    }

    public void RotateShip()
    {
        if (clickedTile == null) return;
        touchTiles.Clear();
        
        transform.localEulerAngles += new Vector3(0, 0, nextZRotation);
        nextZRotation *= -1;
        (xOffset, zOffset) = (zOffset, xOffset);
        
        ClearTileList();
        transform.localPosition = GetOffsetVec(clickedTile.transform.position);
        // transform.rotation = Quaternion.Euler(-90, 0, nextZRotation);
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

    public void FlashOutline(Color tempColor)
    {
        outline.OutlineColor = tempColor;
        outline.enabled = true;

        Invoke(nameof(ResetOutline), 0.5f);
    }

    private void ResetOutline()
    {
        outline.OutlineColor = outlineColor;
        outline.enabled = false;
    }
}
