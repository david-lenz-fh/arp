using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace mrp
{
    public class RoutingNode
    {
        public readonly Dictionary<string, RoutingNode>? _children;
        public readonly Dictionary<string, Action<HttpListenerContext, Dictionary<string, string>>>? _routes;
        
        
        public (RoutingNode?, (string, string)?) NextRoute(string childpath)
        {
            if (_children == null)
            {
                return (null, null);
            }
            if (_children.TryGetValue(childpath, out RoutingNode? child))
            {
                return (child, null);
            }
            foreach (var kvp in _children)
            {
                if (kvp.Key.StartsWith("{") && kvp.Key.EndsWith("}"))
                {
                    string paramName = kvp.Key.Trim('{', '}');
                    return (kvp.Value, (paramName, childpath));
                }
            }
            return (null, null);
        }
        public RoutingNode(
            Dictionary<string, Action<HttpListenerContext, Dictionary<string, string>>>? routes,
            Dictionary<string, RoutingNode>? children)
        {
            _routes = routes;
            _children = children;
        }
    }
}
