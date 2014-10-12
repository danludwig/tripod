using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Api._Extensions
{
    public class StringExtensionTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("\r\n\t")]
        public void Format_ReturnsTemplate_WhenTemplateIsNullOrWhitespace(string template)
        {
            var replacements = new Dictionary<string, string>
            {
                { "{Token1}", "Replacement 1" },
                { "{Token2}", "Replacement 2" },
            };
            var result = replacements.Format(template);
            result.ShouldEqual(template);
        }

        [Theory]
        [InlineData("Test 1: {Token1} formatted", "Test 1: Replacement 1 formatted")]
        [InlineData("Test 2: {Token2} formatted", "Test 2: Replacement 2 formatted")]
        [InlineData("Test 3: {Token1} and {Token2} formatted", "Test 3: Replacement 1 and Replacement 2 formatted")]
        public void Format_ReturnsFormattedTemplate_WhenTemplateIsNullOrWhitespace(string template, string expected)
        {
            var replacements = new Dictionary<string, string>
            {
                { "{Token1}", "Replacement 1" },
                { "{Token2}", "Replacement 2" },
            };
            var result = replacements.Format(template);
            result.ShouldEqual(expected);
        }
    }
}
