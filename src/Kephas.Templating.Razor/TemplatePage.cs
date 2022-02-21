// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplatePage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor
{
    using System.Security.Claims;
    using Kephas.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// The base template page class.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    public abstract class TemplatePage<T> : RazorPage<T>, ITemplatePage<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatePage{T}"/> class.
        /// </summary>
        protected TemplatePage()
        {
            this.HtmlEncoder = System.Text.Encodings.Web.HtmlEncoder.Default;
        }

        /// <summary>
        /// Renders the template asynchronously using the provided model.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual async Task RenderAsync(T model, TextWriter writer, CancellationToken cancellationToken = default)
        {
            this.ViewData = new ViewDataDictionary<T>(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary())
            {
                Model = model,
            };

            this.ViewContext = new TemplateViewContext(this.ViewData, writer);
            await this.ExecuteAsync().PreserveThreadContext();
        }

        private class TemplateViewContext : ViewContext
        {
            public TemplateViewContext(ViewDataDictionary<T> viewData, TextWriter textWriter)
                : base(new NullActionContext(), new NullView(), viewData, new NullTempDataDictionary(), textWriter, new HtmlHelperOptions())
            {
            }

            private class NullActionContext : ActionContext
            {
                public NullActionContext()
                    : base(new NullHttpContext(), new RouteData(), new ActionDescriptor())
                {
                }

                private class NullHttpContext : HttpContext
                {
                    public override IFeatureCollection Features => throw new NotImplementedException();

                    public override HttpRequest Request => throw new NotImplementedException();

                    public override HttpResponse Response => throw new NotImplementedException();

                    public override ConnectionInfo Connection => throw new NotImplementedException();

                    public override WebSocketManager WebSockets => throw new NotImplementedException();

                    public override ClaimsPrincipal User
                    {
                        get => throw new NotImplementedException();
                        set => throw new NotImplementedException();
                    }

                    public override IDictionary<object, object?> Items
                    {
                        get => throw new NotImplementedException();
                        set => throw new NotImplementedException();
                    }

                    public override IServiceProvider RequestServices
                    {
                        get => throw new NotImplementedException();
                        set => throw new NotImplementedException();
                    }

                    public override CancellationToken RequestAborted
                    {
                        get => throw new NotImplementedException();
                        set => throw new NotImplementedException();
                    }

                    public override string TraceIdentifier
                    {
                        get => throw new NotImplementedException();
                        set => throw new NotImplementedException();
                    }

                    public override ISession Session
                    {
                        get => throw new NotImplementedException();
                        set => throw new NotImplementedException();
                    }

                    public override void Abort()
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            private class NullTempDataDictionary : Dictionary<string, object?>, ITempDataDictionary
            {
                public void Keep()
                {
                    throw new NotImplementedException();
                }

                public void Keep(string key)
                {
                    throw new NotImplementedException();
                }

                public void Load()
                {
                    throw new NotImplementedException();
                }

                public object? Peek(string key)
                {
                    throw new NotImplementedException();
                }

                public void Save()
                {
                    throw new NotImplementedException();
                }
            }

            private class NullView : IView
            {
                public string Path => string.Empty;

                public Task RenderAsync(ViewContext context)
                {
                    return Task.CompletedTask;
                }
            }
        }
    }
}