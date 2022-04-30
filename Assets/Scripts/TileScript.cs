using System;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private GameManager gameManager;
    private Outline outline;

    private bool missileHit;
    private readonly Color32[] hitColor = new Color32[2];
    
    private readonly Color gray = new Color32(38, 57, 76, 255);
    private readonly Color brown = new Color32(68, 0, 0, 255);
    private readonly Color green = new Color32(0, 170, 0, 255);

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        hitColor[0] = gameObject.GetComponent<MeshRenderer>().material.color;
        hitColor[1] = gameObject.GetComponent<MeshRenderer>().material.color;
    }

    private void Update()
    {
        if (Camera.main != null)
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0) && hit.collider.gameObject.name == gameObject.name)
            {
                if (missileHit == false)
                {
                    gameManager.TileClicked(hit.collider.gameObject);
                }
            }
        }

        var color = GetComponent<Renderer>().material.color;
        outline.OutlineColor = (color == gray || color == Color.red || 
                                color == brown) ? Color.red : green;
    }

    private void OnMouseEnter()
    {
        outline.enabled = true;
    }
    
    private void OnMouseExit()
    {
        outline.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject)
        {
            case var n when n.CompareTag("Missile"):
                missileHit = true;
                break;
            
            case var n when n.CompareTag("EnemyMissile"):
                hitColor[0] = gray;
                GetComponent<Renderer>().material.color = hitColor[0];
                break;
        }
    }

    public void SetTileColor(int index, Color32 color)
    {
        hitColor[index] = color;
    }

    public void SwitchColors(int colorIndex)
    {
        GetComponent<Renderer>().material.color = hitColor[colorIndex];
    }
}
