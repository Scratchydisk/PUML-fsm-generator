
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


namespace Fsm_Generator
{

    /// <summary>
    /// Functions that can be used in the Stubble 
    /// templates as Tag {{Tag}} and Secton Lambdas {{#Section}}...{{/Section}}
    /// </summary>
    public class StubbleFuncs
    {
        #region Tag Lambdas

        /// <summary>
        /// Simple test tag, returns "*"
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        public Func<string> Star =
            new Func<string>(() => { return "*"; });

        #endregion

        #region Section Lambdas

        /// <summary>
        /// Uppercases all text between the {{#Upper}} to {{/Upper}} tags
        /// </summary>
        /// <returns></returns>
        public Func<string, Func<string, string>, object> Upper =
            new Func<string, Func<string, string>, object>((str, render) =>
            {
                return render(str).ToUpperInvariant();
            });


        /// <summary>
        /// Convers text to Title Case.
        /// E.g. InProgress => In Progress
        /// </summary>
        /// <returns></returns>
        public Func<string, Func<string, string>, object> TitleCase =
            new Func<string, Func<string, string>, object>((str, render) =>
            {
                return TitleCaseText(render(str), " ");
            });

        /// <summary>
        /// Converts text to Pascal Case.
        /// E.g. In Progress => InProgress
        /// </summary>
        /// <returns></returns>
        public Func<string, Func<string, string>, object> PascalCase =
            new Func<string, Func<string, string>, object>((str, render) =>
            {
                String pCase = render(str).Replace(" ", "").Replace("_", "");
                if (pCase.Length == 0)
                    return pCase;
                else if (pCase.Length == 1)
                    return char.ToUpperInvariant(pCase[0]);
                else
                    return char.ToUpperInvariant(pCase[0]) + pCase.Substring(1);

            });

        /// <summary>
        /// Converts text to Camel Case.
        /// E.g. In Progress => inProgress
        /// </summary>
        /// <returns></returns>
        public Func<string, Func<string, string>, object> CamelCase =
            new Func<string, Func<string, string>, object>((str, render) =>
            {
                String cCase = render(str).Replace(" ", "").Replace("_", "");
                if (cCase.Length == 0)
                    return cCase;
                else if (cCase.Length == 1)
                    return char.ToLowerInvariant(cCase[0]);
                else
                    return char.ToLowerInvariant(cCase[0]) + cCase.Substring(1);
            });
            
        /// <summary>
        /// Converts the tag section contents to snake case.
        /// E.g. InProgress => in_progress
        /// </summary>
        /// <returns></returns>    
        public Func<string, Func<string, string>, object> SnakeCase =
            new Func<string, Func<string, string>, object>((str, render) =>
            {
                // Convert to TitleCase using separator "_" and 
                // concert to lowercase.
                String rendered = render(str) ?? "";
                rendered = (String)TitleCaseText(rendered, "_");
                return rendered.ToLowerInvariant();
            });

        #endregion

        #region Helper Code

        private static object TitleCaseText(String str, String separator)
        {
            // De snake'ify the text
            str = str.Replace("_", " ");
            const string pattern = @"
                (?<!^) # Not start
                (
                    # Digit, not preceded by another digit
                    (?<!\d)\d 
                    |
                    # Upper-case letter, followed by lower-case letter if
                    # preceded by another upper-case letter, e.g. 'G' in HTMLGuide
                    (?(?<=[A-Z])[A-Z](?=[a-z])|[A-Z])
                )";

            var options = RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled;

            var SeparatorRegex = new Regex(pattern, options);

            return SeparatorRegex.Replace(str, separator + "$1");
        }

        #endregion
    }

}