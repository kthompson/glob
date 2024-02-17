using System.Collections.Generic;
using System.Linq;

namespace GlobExpressions.AST;

internal sealed class LiteralSet : SubSegment
{
    public Identifier[] Literals { get; }

    public LiteralSet(params Identifier[] literals)
        : base(GlobNodeType.LiteralSet)
    {
            Literals = literals.ToArray();
        }

    public LiteralSet(IEnumerable<Identifier> literals)
        : base(GlobNodeType.LiteralSet)
    {
            Literals = literals.ToArray();
        }

    public override string ToString() => $"{{{string.Join(",", Literals.Select(lit => lit.ToString()))}}}";
}