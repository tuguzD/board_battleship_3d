using System.Collections.Generic;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    public GameObject redFire, yellowFire, orangeFire;
    private int count;

    private List<Color> fireColors = 
        new List<Color> { Color.red, Color.yellow, new Color(1.0f, 0.64f, 0) };

    private static readonly int Color1 = Shader.PropertyToID("_Color");

    private void FixedUpdate()
    {
        if (count > 30)
        {
            fireColors.Add(Color.red);
            var rnd = Random.Range(0, fireColors.Count);
            
            redFire.GetComponent<Renderer>().material.SetColor(Color1, fireColors[rnd]);
            fireColors.RemoveAt(rnd);
            rnd = Random.Range(0, fireColors.Count);
            
            orangeFire.GetComponent<Renderer>().material.SetColor(Color1, fireColors[rnd]);
            fireColors.RemoveAt(rnd);
            
            yellowFire.GetComponent<Renderer>().material.SetColor(Color1, fireColors[0]);
            fireColors.Clear();
            
            fireColors = new List<Color> { Color.red, Color.yellow, new Color(1.0f, 0.64f, 0) };
            count = 0;
        }
        count++;
    }
}
