// ReactiveInteractiveUserInterface/BusinessLogicTest/DiagnosticsLoggerUnitTest.cs

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;

[TestClass]
public class DiagnosticsLoggerUnitTest
{
    [TestMethod]
    public void LoggerWritesToFile()
    {
        string filePath = "testlog.txt";
        if (File.Exists(filePath))
            File.Delete(filePath);

        using (var logger = new DiagnosticsLogger(filePath))
        {
            logger.Log("TestLine1");
            logger.Log("TestLine2");
        }

        string[] lines = File.ReadAllLines(filePath);
        Assert.IsTrue(lines.Length >= 2);
        Assert.AreEqual("TestLine1", lines[0]);
        Assert.AreEqual("TestLine2", lines[1]);
    }

    [TestMethod]
    public void LoggerDisposesGracefully()
    {
        string filePath = "testlog2.txt";
        if (File.Exists(filePath))
            File.Delete(filePath);

        var logger = new DiagnosticsLogger(filePath);
        logger.Log("DisposeTest");
        logger.Dispose();

        string[] lines = File.ReadAllLines(filePath);
        Assert.IsTrue(lines.Length >= 1);
        Assert.AreEqual("DisposeTest", lines[0]);
    }
}