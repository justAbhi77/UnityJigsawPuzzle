using System.Collections.Generic;
using UnityEngine;

public class GameManager : Patterns.Singleton<GameManager>
{
    public bool tileMovementEnabled;
    public double secondsSinceStart;
    public int totalTilesInCorrectPosition;

    [SerializeField] private List<string> jigsawImageNames = new();

    private int _imageIndex;

    public string GetJigsawImageName()
    {
        var imageName = jigsawImageNames[_imageIndex++];
        if (_imageIndex == jigsawImageNames.Count)
        {
            _imageIndex = 0;
        }

        return imageName;
    }
}