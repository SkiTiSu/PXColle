using PXColle.Common;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PXColle.Master
{
    public class PXPolicyManager
    {
        public List<PXPolicy> Policies { get; private set; } = new List<PXPolicy>();

        public PXPolicyManager(IEnumerable<PXPolicy> data)
        {
            Policies = (List<PXPolicy>)data;
        }

        public IEnumerable<PXPolicyResult> GetMatchedPolicy(string url)
        {
            foreach (var policy in Policies)
            {
                foreach (string pattern in policy.UrlPatterns)
                {
                    var match = Regex.Match(url, pattern);
                    if (match.Success)
                    {
                        PXPolicyResult result = new PXPolicyResult
                        {
                            Name = policy.Name,
                            MatchedUrlPattern = pattern,
                            Path = policy.PathPattern,
                            Actions = policy.Actions
                        };
                        var ms = Regex.Matches(policy.PathPattern, @"(?<={)\S+?(?=})");
                        foreach (Match m in ms)
                        {
                            string va = match.Groups[m.Value].Value;
                            // In netstandard2.1 we can use Group.Name
                            if (!string.IsNullOrEmpty(va))
                            {
                                result.Path = result.Path.Replace("{" + m.Value + "}", va);
                            }
                        }
                        yield return result;
                        break;
                    }
                }
            }
        }
    }

    public class PXPolicy
    {
        public string Name { get; set; }
        public IEnumerable<string> UrlPatterns { get; set; }
        public string PathPattern { get; set; }
        public IEnumerable<string> Actions { get; set; }
    }
}
