using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Glob
{
    class GlobToRegexVisitor
    {
        public static string Process(GlobNode node)
        {
            Assert(node, GlobNodeType.Tree);

            return ProcessTree((Tree)node) + "$";
        }

        private static void Assert(GlobNode node, GlobNodeType type)
        {
            if (node.Type != type)
                throw new InvalidOperationException();
        }

        private static string ProcessTree(Tree node)
        {
            return string.Join(@"[/\\]", node.Segments.Select(ProcessSegment));
        }

        private static string ProcessSegment(PathSegment node)
        {
            switch (node.Type)
            {
                case GlobNodeType.Root:
                    return ProcessRoot((Root)node);
                case GlobNodeType.DirectoryWildcard:
                    return ProcessDirectoryWildcard((DirectoryWildcard)node);
                case GlobNodeType.PathSegment:
                    return ProcessPathSegment(node);
                default:
                    throw new InvalidOperationException();
            }
        }

        private static string ProcessPathSegment(PathSegment node)
        {
            return string.Join("", node.SubSegments.Select(ProcessSubSegment));
        }

        private static string ProcessSubSegment(GlobNode node)
        {
            switch (node.Type)
            {
                case GlobNodeType.Identifier:
                    return ProcessIdentifier((Identifier)node);
                case GlobNodeType.CharacterSet:
                    return ProcessCharacterSet((CharacterSet)node);
                case GlobNodeType.LiteralSet:
                    return ProcessLiteralSet((LiteralSet)node);
                case GlobNodeType.CharacterWildcard:
                    return ProcessCharacterWildcard((CharacterWildcard)node);
                case GlobNodeType.StringWildcard:
                    return ProcessStringWildcard((StringWildcard)node);
                default:
                    throw new InvalidOperationException("Expected SubSegment, found " + node.Type);
            }
        }

        private static string ProcessStringWildcard(StringWildcard node)
        {
            return @"[^/\\]*"; //TOOD: we may need to take a better look at using a Windows mode vs a Linux mode for filename matching.
        }

        private static string ProcessCharacterWildcard(CharacterWildcard node)
        {
            return @"[^/\\]{1}";
        }

        private static string ProcessLiteralSet(LiteralSet node)
        {
            return "(" + string.Join(",", node.Literals.Select(ProcessIdentifier)) + ")";
        }

        private static string ProcessCharacterSet(CharacterSet node)
        {
            return (node.Inverted ? "[^" : "[" ) + ProcessIdentifier(node.Characters) + "]";
        }

        private static string ProcessIdentifier(Identifier node)
        {
            return Regex.Escape(node.Value);
        }

        private static string ProcessDirectoryWildcard(DirectoryWildcard node)
        {
            return ".*";
        }

        private static string ProcessRoot(Root node)
        {
            if (node.Text != "") //windows root
                return Regex.Escape(node.Text);

            if (!string.IsNullOrEmpty(node.Text)) // CWD
                return Regex.Escape(node.Text);
            

            return string.Empty;
        }
    }
}
