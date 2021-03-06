﻿using Newtonsoft.Json.Linq;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DocumentRefLoader.Tests
{
    public class OpenApiRefMergerTests
    {
        [Fact]
        public async Task Test_Merger_Simple()
        {
            var merger = new OpenApiMerger(new[]
            {
                new Uri("./_yamlSamples/merger/petstore-minimal.json", UriKind.RelativeOrAbsolute),
                new Uri("./_yamlSamples/merger/example-minimal.json", UriKind.RelativeOrAbsolute)
            });

            var result = await merger.GetMergedJObjectAsync();

            var paths = result[OpenApiConstants.PATHS_KEYWORD]?.Children().ToArray() ?? throw new NullReferenceException();
            var definitions = result[OpenApiConstants.DEFINITIONS_KEYWORD]?.Children().ToArray() ?? throw new NullReferenceException();
            paths.Length.ShouldBe(2);
            definitions.Length.ShouldBe(2);

            ((JProperty)paths[0]).Name.ShouldBe("/petBasePath/pets");
            ((JProperty)paths[1]).Name.ShouldBe("/fooBasePath/foos");
            ((JProperty)definitions[0]).Name.ShouldBe("Pet");
            ((JProperty)definitions[1]).Name.ShouldBe("Foos");
        }

        [Fact]
        public async Task Test_Merger_Duplicated_Definition()
        {
            var merger = new OpenApiMerger(new[]
            {
                new Uri("./_yamlSamples/merger/petstore-minimal.json", UriKind.RelativeOrAbsolute),
                new Uri("./_yamlSamples/merger/example-minimal2.json", UriKind.RelativeOrAbsolute)
            });

            var result = await merger.GetMergedJObjectAsync();

            var paths = result[OpenApiConstants.PATHS_KEYWORD]?.Children().ToArray() ?? throw new NullReferenceException();
            var definitions = result[OpenApiConstants.DEFINITIONS_KEYWORD]?.Children().ToArray() ?? throw new NullReferenceException();
            paths.Length.ShouldBe(2);
            definitions.Length.ShouldBe(1);

            ((JProperty)paths[0]).Name.ShouldBe("/petBasePath/pets");
            ((JProperty)paths[1]).Name.ShouldBe("/fooBasePath/foos");
            ((JProperty)definitions[0]).Name.ShouldBe("Pet");
        }
    }
}
