namespace Library.Responses;

public class SearchResponse
{
    public bool Found { get; set; }

    public PositionInResult Position { get; set; } = default!;
}
