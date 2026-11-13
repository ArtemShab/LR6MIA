using Xunit;

namespace DryCleaningApi.Tests;

public class StringUtils
{
    public string Reverse(string input) => new string(input.Reverse().ToArray());

    public bool IsPalindrome(string input)
    {
        var clean = input.Replace(" ", "").ToLower();
        return clean == Reverse(clean);
    }

    public int CountWords(string input) =>
        string.IsNullOrWhiteSpace(input) ? 0 : input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
}

public class UnitTest1
{
    private readonly StringUtils _utils = new();

    [Fact]
    public void Reverse_ReturnsReversedString()
    {
        Assert.Equal("cba", _utils.Reverse("abc"));
    }

    [Fact]
    public void IsPalindrome_ReturnsTrueForRadar()
    {
        Assert.True(_utils.IsPalindrome("radar"));
    }

    [Fact]
    public void CountWords_ReturnsCorrectCount()
    {
        Assert.Equal(3, _utils.CountWords("Dry cleaning service"));
    }
}