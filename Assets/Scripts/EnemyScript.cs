using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ReSharper disable CollectionNeverUpdated.Local

public class EnemyScript : MonoBehaviour
{
    private char[] guessGrid;
    private List<int> potentialHits, currentHits;
    private int guess;
    
    public GameObject enemyMissilePrefab;
    public GameManager gameManager;

    private void Start()
    {
        potentialHits = new List<int>();
        currentHits = new List<int>();
        guessGrid = Enumerable.Repeat('o', 100).ToArray();
    }

    public static List<int[]> PlaceEnemyShips()
    {
        var enemyShips = new List<int[]>
        {
            new [] {-1, -1, -1, -1, -1},
            new [] {-1, -1, -1, -1},
            new [] {-1, -1, -1},
            new [] {-1, -1, -1},
            new [] {-1, -1}
        };
        var gridNumbers = Enumerable.Range(1, 100).ToArray();
        foreach (var tileNumArray in enemyShips)
        {
            var taken = true;
            while (taken)
            {
                taken = false;
                var shipNose = Random.Range(0, 99);
                var rotateBool = Random.Range(0, 2);
                var minusAmount = rotateBool == 0 ? 10 : 1;
                for (var i = 0; i < tileNumArray.Length; i++)
                {
                    // check that ship end will not go off board and check if tile is taken
                    if (shipNose - minusAmount * i < 0 || 
                        gridNumbers[shipNose - i * minusAmount] < 0)
                    {
                        taken = true;
                        break;
                    }
                    // Ship is horizontal, check ship doesnt go off the sides 0 to 10, 11 to 20
                    if (minusAmount == 1 && shipNose / 10 != ((shipNose - i * minusAmount) - 1) / 10)
                    {
                        taken = true;
                        break;
                    }
                }
                // if tile is not taken, loop through tile numbers assign them to the array in the list
                if (taken == false)
                {
                    for (var j = 0; j < tileNumArray.Length; j++)
                    {
                        tileNumArray[j] = gridNumbers[shipNose - j * minusAmount];
                        gridNumbers[shipNose - j * minusAmount] = -1;
                    }
                }
            }
        }
        foreach (var x in enemyShips)
            Debug.Log("x: " + x[0]);
        
        return enemyShips;
    }

    public void NpcTurn()
    {
        var hitIndex = new List<int>();
        for (var i = 0; i < guessGrid.Length; i++)
            if (guessGrid[i] == 'h') hitIndex.Add(i);

        switch (hitIndex.Count)
        {
            case var n when n > 1:
                var diff = hitIndex[1] - hitIndex[0];
                var nextIndexMore = hitIndex[0] + diff;
                while (guessGrid[nextIndexMore] != 'o')
                {
                    if (guessGrid[nextIndexMore] == 'm' || nextIndexMore > 100 || nextIndexMore < 0)
                        diff *= -1;
                
                    nextIndexMore += diff;
                }
                guess = nextIndexMore;
                break;
            
            case 1:
                var closeTiles = new List<int> {1, -1, 10, -10};

                var index = Random.Range(0, closeTiles.Count);
                var possibleGuess = hitIndex[0] + closeTiles[index];
                var onGrid = possibleGuess > -1 && possibleGuess < 100;
            
                while ((!onGrid || guessGrid[possibleGuess] != 'o') && closeTiles.Count > 0)
                {
                    closeTiles.RemoveAt(index);
                    index = Random.Range(0, closeTiles.Count);
                    possibleGuess = hitIndex[0] + closeTiles[index];
                    onGrid = possibleGuess > -1 && possibleGuess < 100;
                }
                guess = possibleGuess;
                break;
            
            case var n when n < 1:
                var nextIndexLess = Random.Range(0, 100);
                while (guessGrid[nextIndexLess] != 'o')
                    nextIndexLess = Random.Range(0, 100);
                
                nextIndexLess = GuessAgainCheck(nextIndexLess);
                Debug.Log(" --- ");
                
                nextIndexLess = GuessAgainCheck(nextIndexLess);
                Debug.Log(" -########-- ");
                
                guess = nextIndexLess;
                break;
        }
        
        var tile = GameObject.Find($"Tile ({guess + 1})");
        guessGrid[guess] = 'm';
        var position = tile.transform.position;
        
        var vec = position;
        vec.y += 15;
        
        var missile = 
            Instantiate(enemyMissilePrefab, vec, enemyMissilePrefab.transform.rotation);
        
        missile.GetComponent<EnemyMissileScript>().SetTarget(guess);
        missile.GetComponent<EnemyMissileScript>().targetTileLocation = position;
    }

    private int GuessAgainCheck(int nextIndex)
    {
        var newGuess = nextIndex;
        var edgeCase =
            nextIndex < 10 || nextIndex > 89 || 
            nextIndex % 10 == 0 || nextIndex % 10 == 9;
        
        var nearGuess = false;
        
        if (nextIndex + 1 < 100) 
            nearGuess = guessGrid[nextIndex + 1] != 'o';
        
        if (!nearGuess && nextIndex - 1 > 0) 
            nearGuess = guessGrid[nextIndex - 1] != 'o';
        
        if (!nearGuess && nextIndex + 10 < 100) 
            nearGuess = guessGrid[nextIndex + 10] != 'o';
        
        if (!nearGuess && nextIndex - 10 > 0) 
            nearGuess = guessGrid[nextIndex - 10] != 'o';

        if (nearGuess || edgeCase)
            newGuess = Random.Range(0, 100);

        while (guessGrid[newGuess] != 'o')
            newGuess = Random.Range(0, 100);
        
        Debug.Log($"nx: {nextIndex} newGuess: {newGuess} e: {edgeCase} g: {nearGuess}");
        
        return newGuess;
    }
    public void MissileHit(int hit)
    {
        guessGrid[guess] = 'h';
        Invoke(nameof(EndTurn), 1.0f);
    }

    public void SunkPlayer()
    {
        for (var i = 0; i < guessGrid.Length; i++)
            if (guessGrid[i] == 'h') guessGrid[i] = 'x';
    }

    private void EndTurn()
    {
        gameManager.GetComponent<GameManager>().EndEnemyTurn();
    }

    public void PauseAndEnd(int miss)
    {
        if (currentHits.Count > 0 && currentHits[0] > miss)
            foreach (var potential in potentialHits)
            {
                var condition = currentHits[0] > miss;
                if ((condition && potential < miss) || 
                    (!condition && potential > miss))
                {
                    potentialHits.Remove(potential);
                }
            }
        Invoke(nameof(EndTurn), 1.0f);
    }
}
