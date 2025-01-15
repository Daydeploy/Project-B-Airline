public class MilesModel
{
    public MilesModel(string level, int experience, int points, string history)
    {
        Enrolled = false;
        Level = level;
        Experience = experience;
        Points = points;
        History = history;
    }

    public bool Enrolled { get; set; }
    public string Level { get; set; }
    public int Experience { get; set; }
    public int Points { get; set; }
    public string History { get; set; }
}