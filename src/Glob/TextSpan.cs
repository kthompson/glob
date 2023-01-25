using System;

namespace GlobExpressions;

struct TextSpan : IComparable<TextSpan>, IComparable
{
    public int Start { get; }
    public int Length { get; }
    public int End => Start + Length;

    public TextSpan(int start, int length)
    {
        Start = start;
        Length = length;
    }

    public static TextSpan FromBounds(in int start, in int end) => new TextSpan(start, end - start);

    public bool Contains(int position) => Start <= position && position <= End;

    public bool Overlaps(TextSpan other) => other.Contains(Start) || Contains(other.Start);

    public TextSpan? Intersection(TextSpan other) =>
        Overlaps(other)
            ? TextSpan.FromBounds(Math.Max(Start, other.Start), Math.Min(End, other.End))
            : null;

    public override string ToString() => $"{Start}..{End}";

    public int CompareTo(TextSpan other)
    {
        var startComparison = Start.CompareTo(other.Start);
        return startComparison != 0 ? startComparison : Length.CompareTo(other.Length);
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return 1;
        return obj is TextSpan other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {nameof(TextSpan)}");
    }

    public static readonly TextSpan Empty = new TextSpan(-1, 0);
}
