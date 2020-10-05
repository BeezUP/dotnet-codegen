using CodegenUP.CodeGen;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CodegenUP.Tests
{
    public class TemplateHelperTests
    {
        [Fact]
        public void GetTemplates()
        {
            var expecteds = new[]
            {
                (path:"_samples/test0/template.cshtml", fName:"template", dir:""),
                (path:"_samples/test0/template2.txt.cshtml", fName:"template2.txt", dir:""),
                (path:"_samples/test0/subFolder/template.cs.cshtml", fName:"template.cs", dir:"subFolder"),
                (path:"_samples/test0/subFolder/template2.cshtml", fName:"template2", dir:"subFolder"),
                (path:"_samples/test0/subFolder/subsub/temp.cshtml", fName:"temp", dir:"subFolder/subsub"),
            }.OrderBy(x => x.path).ToArray();

            var tInfos = TemplateHelper.GetTemplates("_samples/test0", extension: "*.cshtml").OrderBy(i => i.FilePath).ToArray();

            for (int i = 0; i < expecteds.Length; i++)
            {
                var info = tInfos[i];
                var (path, fName, dir) = expecteds[i];

                info.FilePath.Replace("\\", "/").ShouldBe(path);
                info.FileName.ShouldBe(fName);
                info.Directory.Replace("\\", "/").ShouldBe(dir);
            }

            tInfos.Length.ShouldBe(expecteds.Length);
        }

        [Fact]
        public void SingleFileFolder()
        {
            var tInfos = TemplateHelper.GetTemplates("_samples/test0/template.cshtml").Single();
            tInfos.FilePath.ShouldBe("_samples/test0/template.cshtml");
            tInfos.FileName.ShouldBe("template");
            tInfos.Directory.ShouldBe("");
        }

        [Fact]
        public void EmptyTemplate()
        {
            TemplateHelper.GetTemplates("folder/nonExistingfile.cshtml").Count().ShouldBe(0);
            TemplateHelper.GetTemplates("nonExistingFolder").Count().ShouldBe(0);
        }
    }
}
