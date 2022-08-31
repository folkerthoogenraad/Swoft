using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Semantic
{
    public static class ScopePath
    {
        public static string Create(ScopeSymbol scope)
        {
            List<string> path = new List<string>();

            ScopeSymbol? s = scope;

            while(s != null)
            {
                path.Add(s.GetScopeIdentifier());
                s = s.ParentScope;
            }

            path.Reverse();

            return string.Join('.', path);
        }

        public static ScopeSymbol Resolve(ScopeSymbol root, string s)
        {
            var paths = s.Split('.');
            var rootIdentifier = paths[0];

            if(root.GetScopeIdentifier() != rootIdentifier)
            {
                throw new Exception("Root doesn't match.");
            }

            ScopeSymbol result = root;

            for(int i = 1; i < paths.Length; i++)
            {
                result = LookupChild(result, paths[i]);
            }

            return result;
        }

        private static ScopeSymbol LookupChild(ScopeSymbol self, string s)
        {
            if (s.StartsWith("child#"))
            {
                int index = int.Parse(s.Substring("child#".Length));

                return self.Children[index];
            }
            else
            {
                return self.Functions.First(f => f.Name == s);
            }
        }

        private static (string First, string Remainder) SplitOff(string s)
        {
            int index = s.IndexOf('.');

            if (index < 0) return (s, string.Empty);

            return (s.Substring(0, index), s.Substring(index + 1));
        }
    }
}
