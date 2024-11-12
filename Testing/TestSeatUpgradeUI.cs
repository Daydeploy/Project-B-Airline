using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class TestSeatUpgradeUI
{
    private SeatSelectionUI _seatSelectionUI;
    private StringWriter _consoleOutput;
    private TextWriter _originalOutput;

    [TestInitialize]
    public void Setup()
    {
        _seatSelectionUI = new SeatSelectionUI();
        _consoleOutput = new StringWriter();
        _originalOutput = Console.Out;
        Console.SetOut(_consoleOutput);
    }

    [TestCleanup]
    public void Cleanup()
    {
        Console.SetOut(_originalOutput);
        _consoleOutput.Dispose();
    }

    [TestMethod]
    public void TestDisplayUpgradeBenefits()
    {
        _seatSelectionUI.DisplayUpgradeBenefits("Business");
        string output = _consoleOutput.ToString();

        Assert.IsTrue(output.Contains("Priority boarding"));
        Assert.IsTrue(output.Contains("Extra legroom"));
        Assert.IsTrue(output.Contains("Premium meals"));
    }

    [TestMethod]
    public void TestSeatClassDetermination()
    {
        Assert.AreEqual("First", _seatSelectionUI.GetSeatClass("1A"));
        Assert.AreEqual("First", _seatSelectionUI.GetSeatClass("3F"));
        Assert.AreEqual("Business", _seatSelectionUI.GetSeatClass("5C"));
        Assert.AreEqual("Business", _seatSelectionUI.GetSeatClass("8D"));
        Assert.AreEqual("Economy", _seatSelectionUI.GetSeatClass("15A"));
        Assert.AreEqual("Economy", _seatSelectionUI.GetSeatClass("30F"));
    }

    [TestMethod]
    public void TestSeatOccupancy()
    {
        string seatNumber = "5A";

        _seatSelectionUI.SetSeatOccupied(seatNumber);

        _seatSelectionUI.DisplaySeatingChart();
        string output = _consoleOutput.ToString();

        Assert.IsTrue(output.Contains(seatNumber));
    }

    [TestMethod]
    public void TestSeatSelection()
    {
        var stringReader = new StringReader("5A\n");
        Console.SetIn(stringReader);

        string selectedSeat = _seatSelectionUI.SelectSeat();

        Assert.AreEqual("5A", selectedSeat);
    }
} 