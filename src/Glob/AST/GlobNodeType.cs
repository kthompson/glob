namespace GlobExpressions.AST;

internal enum GlobNodeType
{
    Tree,

    // Segments
    Root,

    DirectoryWildcard,
    DirectorySegment,

    // SubSegments
    CharacterSet,

    Identifier,
    LiteralSet,
    StringWildcard,
    CharacterWildcard
}