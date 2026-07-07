namespace PS.SuperNDT.UI.Models;

public class Job
{
    public string JobNo { get; set; } = "";

    public string Customer { get; set; } = "";

    public string PipeSize { get; set; } = "";

    public string Material { get; set; } = "";

    public int TotalShots { get; set; }

    public string Result { get; set; } = "";
}