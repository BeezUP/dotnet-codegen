using CodegenUP.CustomHandlebars;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace CodegenUP.Tests
{
    public class CustomHelpersLoadingTest
    {
        private readonly ITestOutputHelper _output;

        public CustomHelpersLoadingTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void LoadHelpersFromTheExampleProjectBeside()
        {
            var customHelpersProjectPath = "../../../../../src/CodegenUP.CustomHelpers";
            var tmpFolder = Path.GetRandomFileName();
            try
            {
                var helpers = HandlebarsConfigurationHelper.GetHelpersFromFolder(customHelpersProjectPath, tmpFolder);
                _output.WriteLine(string.Join(Environment.NewLine, helpers.Select(h => h.ToString())));
                helpers.Length.ShouldNotBe(0);
            }
            finally
            {
                //if (Directory.Exists(tmpFolder))
                //{
                //    Directory.Delete(tmpFolder, true);
                //}
            }
        }
    }
}
