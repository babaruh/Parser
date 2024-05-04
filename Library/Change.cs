namespace Library;

public class Change : IComparable<Change>
{
    public int Start { get; init; }

    public int End { get; init; }

    public string Replacement { get; set; } = default!;

    public int CompareTo(Change? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        var startComparison = Start.CompareTo(other.Start);

        return startComparison != 0
            ? startComparison
            : End.CompareTo(other.End);
    }
}
