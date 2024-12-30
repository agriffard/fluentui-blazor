// ------------------------------------------------------------------------
// MIT License - Copyright (c) Microsoft Corporation. All rights reserved.
// ------------------------------------------------------------------------

using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Xml.Linq;
using FluentUI.Demo.Generators;
using Xunit;

namespace FluentUI.Demo.DocViewer.Tests.Generators;

public class DemoGeneratorsTests
{
    [Fact]
    public void DemoGenerators_SampleCommentCode()
    {
        var filename = "C:\\VSO\\Perso\\fluentui-blazor-v5\\examples\\Demo\\FluentUI.Demo.Client\\Microsoft.FluentUI.AspNetCore.Components.xml";

        var members = new List<XElement>();
        using (var reader = new StreamReader(filename))
        {
            var xml = XDocument.Load(reader);
            members.AddRange(xml.Descendants("member"));
        }

        var code = CodeCommentsGenerator.GenerateCodeComments(members);
    }
}
