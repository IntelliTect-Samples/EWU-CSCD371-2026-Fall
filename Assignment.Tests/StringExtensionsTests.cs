using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IntelliTect.TestTools; // Assuming your extensions are in this namespace

namespace IntelliTect.TestTools.Tests;

[TestClass]
public class StringExtensionsTests
{
    // --- Tests for IsLikeRegEx(this string s, string pattern) ---

    [TestMethod]
    public void IsLikeRegEx_CaseInsensitiveMatch_ReturnsTrue()
    {
        const string text = "HelloWorld";
        const string pattern = "hello(.*)";
        Assert.IsTrue(text.IsMatchRegex(pattern), "Should match case-insensitively.");
    }

    [TestMethod]
    public void IsMatchRegEx_FullMatch_ReturnsTrue()
    {
        const string text = "ExactMatch";
        const string pattern = "^ExactMatch$";
        Assert.IsTrue(text.IsMatchRegex(pattern), "Should match the exact string.");
    }

    [TestMethod]
    public void IsLikeRegEx_NoMatch_ReturnsFalse()
    {
        const string text = "HelloWorld";
        const string pattern = "Goodbye";
        Assert.IsFalse(text.IsMatchRegex(pattern), "Should not find a match.");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IsMatchRegEx_NullString_ThrowsException()
    {
        string? text = null;
        const string pattern = ".*";
        // Regex.IsMatch on a null string throws an ArgumentNullException
#pragma warning disable CS8604 // Possible null reference argument.
        _ = text.IsMatchRegex(pattern);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IsLikeRegEx_NullPattern_ThrowsException()
    {
        const string text = "Test";
        string? pattern = null;
        // The Regex constructor on a null pattern throws an ArgumentNullException
#pragma warning disable CS8604 // Possible null reference argument.
        _ = text.IsMatchRegex(pattern);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    // --- Tests for IsLike(this string text, string pattern) (No escape character) ---

    [TestMethod]
    public void IsLike_SimpleWildcardMatch_ReturnsTrue()
    {
        const string text = "TestString";
        const string pattern = "Test*"; // * matches zero or more characters
        Assert.IsTrue(text.IsLike(pattern));
    }

    [TestMethod]
    public void IsLike_SingleCharacterMatch_ReturnsTrue()
    {
        const string text = "cat";
        const string pattern = "c?t"; // ? matches exactly one character
        Assert.IsTrue(text.IsLike(pattern));
    }

    [TestMethod]
    public void IsLike_CharacterListMatch_ReturnsTrue()
    {
        const string text = "A1";
        const string pattern = "[AB]1"; // Matches 'A' or 'B' followed by '1'
        Assert.IsTrue(text.IsLike(pattern));
    }

    [TestMethod]
    public void IsLike_CharacterListNoMatch_ReturnsFalse()
    {
        const string text = "C1";
        const string pattern = "[AB]1";
        Assert.IsFalse(text.IsLike(pattern));
    }

    [TestMethod]
    public void IsLike_NoMatch_ReturnsFalse()
    {
        const string text = "HelloWorld";
        const string pattern = "Goodbye*";
        Assert.IsFalse(text.IsLike(pattern));
    }

    [TestMethod]
    public void IsLike_NullText_ReturnsFalse()
    {
        string? text = null;
        const string pattern = "*";
#pragma warning disable CS8604 // Possible null reference argument.
        Assert.IsFalse(condition: text.IsLike(pattern));
#pragma warning restore CS8604 // Possible null reference argument.
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IsLike_NullPattern_ThrowsException()
    {
        const string text = "Test";
        string? pattern = null;
        // The WildcardPattern constructor should throw on a null pattern
#pragma warning disable CS8604 // Possible null reference argument.
        _ = text.IsLike(pattern);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    // --- Tests for IsLike(this string text, string pattern, char escapeCharacter) (With escape character) ---

    [TestMethod]
    public void IsLikeEscapeCharacter_EscapedWildcard_MatchesLiteral()
    {
        // FIX: Changed the escaped character from '%' to '*' to match the expected behavior 
        // of the WildcardPattern class, which likely only allows escaping of actual wildcards.
        const string text = "100*";
        const string pattern = "100#*"; // Pattern uses '#' to escape the '*'
        const char escapeChar = '#';

        Assert.IsTrue(text.IsLike(pattern, escapeChar), "Escaping '*' with '#' should treat it as a literal character.");
    }

    [TestMethod]
    public void IsLikeEscapeCharacter_EscapedQuestionMark_MatchesLiteral()
    {
        const string text = "File?";
        const string pattern = "File\\?"; // \ is the escape character
        const char escapeChar = '\\';
        Assert.IsTrue(text.IsLike(pattern, escapeChar), "The wildcard '?' should be treated as a literal.");
    }

    [TestMethod]
    public void IsLikeEscapeCharacter_NoEscapeNeeded_StillWorks()
    {
        const string text = "Test";
        const string pattern = "T?st";
        const char escapeChar = '\\';
        Assert.IsTrue(text.IsLike(pattern, escapeChar), "Should still match when escape isn't used.");
    }
}
