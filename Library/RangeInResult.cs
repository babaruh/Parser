namespace Library;

public class RangeInResult(PositionInResult start, PositionInResult end)
{
    public PositionInResult Start { get; set; } = start;

    public PositionInResult End { get; set; } = end;
}
