using System;

public static class ExpressionEvaluator
{
    /// <summary>
    /// Evaluates a Sum of Products (SOP) boolean expression using numerical pin indices (e.g., "0.1 + 2.3").
    /// </summary>
    /// <param name="expression">The SOP string using '.' for AND and '+' for OR with number indices.</param>
    /// <param name="inputs">The array of boolean variable states mapped directly to their array index.</param>
    public static bool EvaluateSOP(string expression, bool[] inputs)
    {
        // 1. Clean whitespace and isolate individual AND terms
        string[] productTerms = expression.Replace(" ", "").Split('+');

        foreach (string term in productTerms)
        {
            if (string.IsNullOrEmpty(term)) continue;

            // 2. Isolate individual numerical literals within the AND term
            string[] literals = term.Split('.');
            bool termResult = true;

            foreach (string literal in literals)
            {
                if (string.IsNullOrEmpty(literal)) continue;

                // Check for inversion markers
                bool invert = literal.StartsWith('~') || literal.StartsWith('!');
                
                // Extract just the number portion of the string
                string indexString = invert ? literal.Substring(1) : literal;

                // 3. Parse the index string directly to an integer
                if (int.TryParse(indexString, out int inputIndex))
                {
                    if (inputIndex >= 0 && inputIndex < inputs.Length)
                    {
                        bool variableState = inputs[inputIndex];
                        if (invert) variableState = !variableState;

                        // Accumulate the inner AND gate calculation
                        termResult &= variableState;
                    }
                    else
                    {
                        // Pin index doesn't exist in the input array; fail this term safely
                        termResult = false;
                    }
                }
                else
                {
                    // Malformed string fallback
                    termResult = false;
                }

                // Optimization: Short-circuit out of the AND chain if it's already false
                if (!termResult) break;
            }

            // 4. If any full product term evaluates to true, the outer OR condition is met
            if (termResult && literals.Length > 0)
            {
                return true;
            }
        }

        // All product terms evaluated to false
        return false;
    }
}
