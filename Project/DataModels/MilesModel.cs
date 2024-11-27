public class MilesModel
{
    public string Level { get; set; }
    public int Experience { get; set; }
    public int Points { get; set; }
    public string History { get; set; }

    public MilesModel(string level, int experience, int points, string history)
    {
        Level = level;
        Experience = experience;
        Points = points;
        History = history;
    }
}