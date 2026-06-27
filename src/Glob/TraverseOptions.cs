namespace GlobExpressions;

internal sealed class TraverseOptions
{
    public TraverseOptions(bool caseSensitive, bool emitFiles, bool emitDirectories)
    {
        CaseSensitive = caseSensitive;
        EmitFiles = emitFiles;
        EmitDirectories = emitDirectories;
    }

    public bool CaseSensitive { get; }
    public bool EmitFiles { get; }
    public bool EmitDirectories { get; }
}
