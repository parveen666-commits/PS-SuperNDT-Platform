using System;
using System.Collections.Generic;
using System.Linq;

namespace PS.SuperNDT.UI.Services;

public sealed class ReviewSessionService
{
    private readonly List<string> _reviewImages = new();

    public IReadOnlyList<string> Images => _reviewImages;

    public int CurrentIndex { get; private set; } = -1;

    public string? CurrentImage
    {
        get
        {
            if (CurrentIndex < 0 ||
                CurrentIndex >= _reviewImages.Count)
            {
                return null;
            }

            return _reviewImages[CurrentIndex];
        }
    }

    public void Load(IEnumerable<string> imagePaths)
    {
        _reviewImages.Clear();

        _reviewImages.AddRange(
            imagePaths.Where(x =>
                !string.IsNullOrWhiteSpace(x)));

        CurrentIndex =
            _reviewImages.Count > 0 ? 0 : -1;
    }

    public string? Next()
    {
        if (_reviewImages.Count == 0)
        {
            return null;
        }

        if (CurrentIndex < _reviewImages.Count - 1)
        {
            CurrentIndex++;
        }

        return CurrentImage;
    }

    public string? Previous()
    {
        if (_reviewImages.Count == 0)
        {
            return null;
        }

        if (CurrentIndex > 0)
        {
            CurrentIndex--;
        }

        return CurrentImage;
    }

    public void Clear()
    {
        _reviewImages.Clear();
        CurrentIndex = -1;
    }
}