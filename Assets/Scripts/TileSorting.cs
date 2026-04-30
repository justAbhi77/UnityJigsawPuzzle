using System.Collections.Generic;
using UnityEngine;

public class TileSorting
{
    private readonly List<SpriteRenderer> _mSortedIndices = new();

    public void Clear()
    {
        _mSortedIndices.Clear();
    }

    public void Add(SpriteRenderer spriteRenderer)
    {
        _mSortedIndices.Add(spriteRenderer);
        SetRenderOrder(spriteRenderer, _mSortedIndices.Count);
    }

    public void Remove(SpriteRenderer spriteRenderer)
    {
        _mSortedIndices.Remove(spriteRenderer);
        for (var i = 0; i < _mSortedIndices.Count; i++)
        {
            SetRenderOrder(_mSortedIndices[i], i + 1);
        }
    }

    public void BringToTop(SpriteRenderer spriteRenderer)
    {
        Remove(spriteRenderer);
        Add(spriteRenderer);
    }

    private static void SetRenderOrder(SpriteRenderer spriteRenderer, int index)
    {
        spriteRenderer.sortingOrder = index;
        var p = spriteRenderer.transform.position;
        p.z = -index / 10.0f;
        spriteRenderer.transform.position = p;
    }
}