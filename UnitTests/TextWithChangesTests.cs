namespace UnitTests;

public class TextWithChangesTests
{
    public readonly int TabWidth = 8;
    
    [Fact]
    public void AddChange_NormalFlow()
    {
        var text = new TextWithChanges("while( true){foo( );}");

        text.AddChange(new RangeInResult(new PositionInResult(5), new PositionInResult(7)), " (");
        text.AddChange(new RangeInResult(new PositionInResult(12), new PositionInResult(12)), "\n");
        text.AddChange(new RangeInResult(new PositionInResult(13), new PositionInResult(13)), "\n    ");
        text.AddChange(new RangeInResult(new PositionInResult(17), new PositionInResult(18)), "");
        text.AddChange(new RangeInResult(new PositionInResult(20), new PositionInResult(20)), "\n");

        var result = text.ApplyChanges();
        
        Assert.Equal("while (true)\n{\n    foo();\n}", result);
    }

    [Fact]
    public void SimpleSpaceCount_TwoSpacesOneTab()
    {
        var text = new TextWithChanges("foo \t bar");
        var range = new RangeInResult(new PositionInResult(0), new PositionInResult(9));
        var result = text.SimpleSpaceCount(range, TabWidth);

        Assert.Equal(2, result.SpaceCount);
        Assert.Equal(1, result.TabCount);
        Assert.Equal(8 + 1 * TabWidth, result.VisualLength);
    }
    
    [Fact]
    public void SimpleSpaceCount_DoubleSpaceOneTab()
    {
        var text = new TextWithChanges("foo  \tbar");
        var response = text.SimpleSpaceCount(new RangeInResult(new PositionInResult(0), new PositionInResult(9)), TabWidth);

        Assert.Equal(2, response.SpaceCount);
        Assert.Equal(1, response.TabCount);
        Assert.Equal(8 + 1 * TabWidth, response.VisualLength);
    }

    [Fact]
    public void Search_FoundNonWhitespace()
    {
        var text = new TextWithChanges("   foo");
        var response = text.Search(new RangeInResult(new PositionInResult(0), new PositionInResult(6)), "startToEnd", "nonWhitespace");

        Assert.True(response.Found);
        Assert.Equal(3, response.Position.OffsetInOriginal);
    }

    [Fact]
    public void Search_NotFoundNonWhitespace()
    {
        var text = new TextWithChanges("     ");
        var response = text.Search(new RangeInResult(new PositionInResult(0), new PositionInResult(5)), "startToEnd", "nonWhitespace");

        Assert.False(response.Found);
    }

    [Fact]
    public void NewLineCount_TwoLineBreaks()
    {
        var text = new TextWithChanges("foo\nbar\nbaz");
        var count = text.NewLineCount(new RangeInResult(new PositionInResult(0), new PositionInResult(11)));

        Assert.Equal(2, count);
    }

    [Fact]
    public void ApplyChanges_NoChanges()
    {
        var text = new TextWithChanges("foo");
        var result = text.ApplyChanges();

        Assert.Equal("foo", result);
    }
}
