using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Api.Test.Helper
{
    /// <summary>
    /// Object exntesion comparer
    /// </summary>
    public static class ObjecComparerExtension
    {
        /// <summary>
        /// Checks whether a object is equals to another
        /// </summary>
        /// <param name="actual">Actual object</param>
        /// <param name="expected">Object to be compared</param>
        /// <returns>True whether an object is equivalent to the other</returns>
        public static bool EquivalentTo(this object actual, object expected)
        {
            if (actual == expected)
                return true;

            if (actual == null || expected == null)
                return false;

            if (actual.Equals(expected))
                return true;

            if (!actual.GetType().IsEquivalentTo(expected.GetType()))
                return false;

            foreach (var property in actual.GetType().GetProperties())
            {
                var expectedProperty = expected.GetType().GetProperty(property.Name);
                if (expectedProperty == null)
                    return false;

                object actualValue = null;
                object expectedValue = null;

                try
                {
                    actualValue = property.GetValue(actual);
                    expectedValue = expectedProperty.GetValue(expected);
                }
                catch
                {
                    continue;
                }

                if (!EquivalentTo(actualValue, expectedValue))
                    return false;
            }

            return true;
        }
    }
}
