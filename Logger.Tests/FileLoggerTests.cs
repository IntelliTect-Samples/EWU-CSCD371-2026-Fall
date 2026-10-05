using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class FileLoggerTests
{
	[TestMethod]
	public void Log_WritesFormattedMessageToFile()
	{
		// Arrange
		var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");
		var logger = new FileLogger(filePath) { className = nameof(FileLoggerTests) };

		try
		{
			// Act
			logger.Log(LogLevel.Information, "Test message");

			// Assert
			var contents = File.ReadAllText(filePath);
			StringAssert.Contains(contents, "FileLoggerTests Information: Test message");
			Assert.IsTrue(
				Regex.IsMatch(
					contents,
					@"\d{4}-\d{2}-\d{2} \d{2}:\d{2} (AM|PM) FileLoggerTests Information: Test message"),
				$"The log message was not written with the expected timestamp format. Contents: {contents}");
		}
		finally
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}
	}

	[TestMethod]
	public void Log_AppendsMultipleMessagesToTheSameFile()
	{
		// Arrange
		var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");
		var logger = new FileLogger(filePath) { className = nameof(FileLoggerTests) };

		try
		{
			// Act
			logger.Log(LogLevel.Information, "First message");
			logger.Log(LogLevel.Warning, "Second message");

			// Assert
			var contents = File.ReadAllText(filePath);
			StringAssert.Contains(contents, "First message");
			StringAssert.Contains(contents, "Second message");
			Assert.AreEqual(2, contents.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Length);
		}
		finally
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}
	}
}
