

using BlazorComponentRenderer.Renderer.Classes;
using BlazorComponentRenderer.Renderer.Interfaces;
using LanguageExt.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.HtmlRendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using BlazorComponentRenderer.Renderer.Extensions;
using NSubstitute.Extensions;
using Xunit;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging.Abstractions;
using Bunit;
using Microsoft.AspNetCore.Components.RenderTree;



namespace BlazorRenderer.Tests
{
    public class RenderToStringTest : TestContext
    {
        private readonly IComponentRenderer _componentRenderer = Substitute.For<IComponentRenderer>();
        private readonly IParameterConverter _parameterConverter = Substitute.For<IParameterConverter>();
        private readonly ILoggerFactory _loggerFactory = Substitute.For<ILoggerFactory>();

        private ArcBlazorComponentRenderer _renderer;

        public RenderToStringTest()
        {
            _renderer = new ArcBlazorComponentRenderer(_componentRenderer, _parameterConverter, _loggerFactory);
        }

        [Fact]
        public async Task RenderComponentToString_ReturnsSuccessResult()
        {
            // Arrange
            var model = new TestModel(); // Define your TestModel according to your actual model
            var cancellationToken = new CancellationToken(false);
            var objects = new Dictionary<string, object> { { "test", "value" } };
            var parameters = ParameterView.FromDictionary(objects);
            const string expectedHtml = "Rendered HTML";

            _parameterConverter.ConvertToParameters<TestModel, TestComponent>(model)
                .Returns(Task.FromResult(new Result<ParameterView>(parameters)));

            _componentRenderer.RenderAsync<TestComponent>(parameters, cancellationToken)
                .Returns(Task.FromResult(expectedHtml));

            // Act
            var result = await _renderer.RenderComponentToString<TestModel, TestComponent>(model, cancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedHtml, result.HandleSuccess(success=>success));
        }
    }

    public class TestModel
    {
        public string TestProperty { get; set; }
    }
}

    


