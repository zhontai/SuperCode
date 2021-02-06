using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperCode.Models
{
    public class Pane
    {
        public string Url;
        public string Title;
        public DateTime StartTime = DateTime.Now;
        public DateTime ActiveTime = DateTime.Now;
        public bool ShowClose = false;
        public bool IsClosed = false;
        public RenderFragment Body;

        public Type PageType;
        public IReadOnlyDictionary<string, object> RouteValues;

        public object Instance;

        public void BuildCustomBodyRenderer(Type pageType, IReadOnlyDictionary<string, object> routeValues)
        {
            PageType = pageType;
            RouteValues = routeValues;
            Body = CustomBodyRenderer;
        }

        void CustomBodyRenderer(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
        {
            builder.OpenComponent(0, PageType);
            builder.SetKey(StartTime);

            foreach (KeyValuePair<string, object> routeValue in RouteValues)
            {
                builder.AddAttribute(1, routeValue.Key, routeValue.Value);
            }

            builder.AddComponentReferenceCapture(2, obj =>
            {
                Instance = obj;
            });
            builder.CloseComponent();
        }
    }
}
