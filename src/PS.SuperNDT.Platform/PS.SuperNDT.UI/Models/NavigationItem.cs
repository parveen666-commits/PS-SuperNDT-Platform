using System;

namespace PS.SuperNDT.UI.Models;

public class NavigationItem
{
    public string Title { get; set; } = "";

    public Type? ViewType { get; set; }
}