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
            if (node.Type != GlobNodeType.Tree)
                throw new InvalidOperationException();

            return ProcessTree(node);
        }

        private void Assert(GlobNode node, GlobNodeType type)
        {
            if (node.Type != type)
                throw new InvalidOperationException();
        }

        private static string ProcessTree(GlobNode node)
        {
            return string.Join("/", node.Children.Select(ProcessSegment));
        }

        private static string ProcessSegment(GlobNode node)
        {
            switch (node.Type)
            {
                case GlobNodeType.Root:
                    return ProcessRoot(node);
                case GlobNodeType.DirectoryWildcard:
                    return ProcessDirectoryWildcard(node);
                case GlobNodeType.PathSegment:
                    return ProcessPathSegment(node);
                default:
                    throw new InvalidOperationException();
            }
        }

        private static string ProcessPathSegment(GlobNode node)
        {
            return string.Join("", node.Children.Select(ProcessSubSegment));
        }

        private static string ProcessSubSegment(GlobNode node)
        {
            switch (node.Type)
            {
                case GlobNodeType.Identifier:
                    return ProcessIdentifier(node);
                case GlobNodeType.CharacterSet:
                    return ProcessCharacterSet(node);
                case GlobNodeType.LiteralSet:
                    return ProcessLiteralSet(node);
                case GlobNodeType.CharacterWildcard:
                    return ProcessCharacterWildcard(node);
                case GlobNodeType.WildcardString:
                    return ProcessWildcardString(node);
                default:
                    throw new InvalidOperationException("Expected SubSegment, found " + node.Type);
            }
        }

        private static string ProcessWildcardString(GlobNode node)
        {
            return @"[^/]*";
        }

        private static string ProcessCharacterWildcard(GlobNode node)
        {
            return @"[^/]{1}";
        }

        private static string ProcessLiteralSet(GlobNode node)
        {
            return "(" + string.Join(",", node.Children.Select(ProcessIdentifier)) + ")";
        }

        private static string ProcessCharacterSet(GlobNode node)
        {
            return "[" + ProcessIdentifier(node.Children.First()) + "]";
        }

        private static string ProcessIdentifier(GlobNode node)
        {
            return Regex.Escape(node.Text);
        }

        private static string ProcessDirectoryWildcard(GlobNode node)
        {
            return ".*";
        }

        private static string ProcessRoot(GlobNode node)
        {
            if (node.Children.Count > 0) //windows root
                return Regex.Escape(node.Children[0].Text + ":");

            if (!string.IsNullOrEmpty(node.Text)) // CWD
                return Regex.Escape(node.Text);
            

            return string.Empty;
        }
    }
}
