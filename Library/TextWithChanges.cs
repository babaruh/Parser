using System.Text;
using Library.Responses;

namespace Library;

public class TextWithChanges(string originalText)
{
    private string OriginalText { get; } = originalText;
    private SortedSet<Change> Changes { get; } = [];

    public void AddChange(RangeInResult range, string replacement)
    {
        var start = range.Start.OffsetInOriginal;
        var end = range.End.OffsetInOriginal;

        var newChange = new Change { Start = start, End = end, Replacement = replacement };

        var oldChange = Changes.FirstOrDefault(c => c.Start <= start && c.End >= end);
        if (oldChange is not null)
        {
            if (start <= oldChange.Start && end >= oldChange.End)
            {
                Changes.Remove(oldChange);
            }
            else
            {
                oldChange.Replacement = replacement;
                return;
            }
        }

        Changes.Add(newChange);
    }


    public SearchResponse Search(RangeInResult range, string direction, string whatToSearch)
    {
        var result = new SearchResponse();

        switch (direction)
        {
            case "startToEnd":
            {
                for (var i = range.Start.OffsetInOriginal; i < range.End.OffsetInOriginal; i++)
                {
                    if ((whatToSearch != "nonWhitespace" || char.IsWhiteSpace(OriginalText[i]))
                        && (whatToSearch != "lineBreak" || OriginalText[i] is not '\n')) continue;

                    result.Found = true;
                    result.Position = new PositionInResult(i);
                    break;
                }

                break;
            }
            case "endToStart":
            {
                for (var i = range.End.OffsetInOriginal - 1; i >= range.Start.OffsetInOriginal; i--)
                {
                    if ((whatToSearch != "nonWhitespace" || char.IsWhiteSpace(OriginalText[i]))
                        && (whatToSearch != "lineBreak" || OriginalText[i] is not '\n')) continue;

                    result.Found = true;
                    result.Position = new PositionInResult(i);
                    break;
                }

                break;
            }
        }

        return result;
    }

    public int NewLineCount(RangeInResult range)
    {
        var count = 0;

        for (var i = range.Start.OffsetInOriginal; i < range.End.OffsetInOriginal; i++)
            if (OriginalText[i] == '\n')
                count++;

        return count;
    }

    public SimpleSpaceCountResponse SimpleSpaceCount(RangeInResult range, int tabWidth)
    {
        var result = new SimpleSpaceCountResponse();

        for (var i = range.Start.OffsetInOriginal; i < range.End.OffsetInOriginal; i++)
        {
            switch (OriginalText[i])
            {
                case ' ':
                    result.SpaceCount++;
                    result.VisualLength++;
                    break;
                case '\t':
                    result.TabCount++;
                    result.VisualLength += tabWidth;
                    break;
                default:
                    result.VisualLength++;
                    break;
            }
        }

        return result;
    }

    public string ApplyChanges()
    {
        var result = new StringBuilder(OriginalText);
        var offset = 0;

        foreach (var change in Changes)
        {
            var start = change.Start + offset;
            var end = change.End + offset;
            var length = end - start;

            result.Remove(start, length);
            result.Insert(start, change.Replacement);

            offset += change.Replacement.Length - length;
        }

        return result.ToString();
    }
}
