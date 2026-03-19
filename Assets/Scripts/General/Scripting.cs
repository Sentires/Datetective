using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TheDates.General
{
    public static class Scripting
    {
        // @ could preface dialogue/preset context, such as...
        // @pn.sp/op/pa/pp - 'pronouns'
        // % could preface variables and/or event functions/actions, such as...
        // %fnc:<name>
        // %var:<name>
        // The above will take some research and trial and error...
        
        // CSV-style regex pattern, " as encapsulation
        private const string QuotePattern = @"(?<quoted>""(?:[^""]|"""")*"")";
        // Tags identified by @ or %, broken by whitespace
        private const string TagPattern = @"(?<tag>[@%][^\s]+)";
        // Anything broken by whitespace
        private const string BreakPattern = @"(?<other>[^\s]+)";
        
        public static Queue<string> EvaluateDialogue(this string sentence)
        {
            // Combine the pattern
            var pattern = $"{QuotePattern}|{TagPattern}|{BreakPattern}"; 
            var queue = new Queue<string>();
            var i = 0;
            
            foreach (Match m in Regex.Matches(sentence, pattern)) {
                if (m.Groups["quoted"].Success) {
                    // Remove the outer quotes
                    var raw = m.Value.Substring(1, m.Value.Length - 2);
                    // CSV-style, replace "" and write "
                    string csvFixed = raw.Replace("\"\"", "\"");
                    // Unescape (convert) specific characters (\n, \t, etc)
                    // https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference
                    string final = Regex.Unescape(csvFixed);
                    // Enqueue the "string"
                    queue.Enqueue(final);
                }
                else if (m.Groups["tag"].Success) {
                    // Enqueue the @tag or %tag
                    var raw = m.Groups["tag"].Value;
                    queue.Enqueue(raw);
                }
                else {
                    // Log an error
                    var raw = m.Groups["other"].Value;
                    Debug.LogError($"Syntax error, check your sentence at segment [{i + 1}](\"{raw}\") and try again.");
                }
                i++;
            }
            
            return queue;
        }
    }
}
