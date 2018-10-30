using System.Collections.Generic;

namespace GlobExpressions.Tests
{
    internal class MockFileSystemNode
    {
        public MockFileSystemNode Parent { get; }

        public string Name { get; }

        public bool IsFolder { get; private set; }

        public Dictionary<string, MockFileSystemNode> Children { get; }

        public MockFileSystemNode(string name, MockFileSystemNode parent)
        {
            this.Name = name;
            Parent = parent;
            this.Children = new Dictionary<string, MockFileSystemNode>();
        }

        public MockFileSystemNode Add(string path)
        {
            var segments = path.Split('/', '\\');
            return Add(segments, 0);
        }

        public MockFileSystemNode GetNodeAtPath(string fullPath)
        {
            var segments = fullPath.Split('/', '\\');
            return GetNodeAtPath(segments, 0);
        }

        public string GetFullName()
        {
            var names = new List<string>();

            var node = this;
            while (node != null)
            {
                names.Add(node.Name);
                node = node.Parent;
            }

            names.Reverse();

            return string.Join("/", names);
        }

        private MockFileSystemNode Add(string[] pathSegments, int pathSegmentIndex)
        {
            if (pathSegmentIndex >= pathSegments.Length)
                return this;

            this.IsFolder = true;

            var segment = pathSegments[pathSegmentIndex];

            return GetOrCreateNode(segment).Add(pathSegments, pathSegmentIndex + 1);
        }

        private MockFileSystemNode GetOrCreateNode(string name)
        {
            if (this.Children.TryGetValue(name, out var node))
                return node;

            var newNode = new MockFileSystemNode(name, this);
            this.Children.Add(name, newNode);
            return newNode;
        }

        private MockFileSystemNode GetNodeAtPath(string[] fullPathSegments, int pathSegment)
        {
            var segment = fullPathSegments[pathSegment];
            if (!this.Children.TryGetValue(segment, out var node))
                return null;

            if (pathSegment == fullPathSegments.Length - 1)
                return node;

            return node.GetNodeAtPath(fullPathSegments, pathSegment + 1);
        }
    }
}