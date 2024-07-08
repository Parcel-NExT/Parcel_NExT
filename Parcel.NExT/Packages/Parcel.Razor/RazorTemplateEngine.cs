using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace Parcel.Infrastructure
{
    public enum TemplateFormat
    {
        Text,
        HTML
    }
    public static class RazorTemplateEngine
    {
        #region Method
        public static string CompileTemplate<TType>(TType model, string template, TemplateFormat format = TemplateFormat.HTML, Dictionary<string, object> additionalValues = null)
        {
            TemplateServiceConfiguration config = new()
            {
                Language = RazorEngine.Language.CSharp,
                EncodedStringFactory = format == TemplateFormat.HTML ? new HtmlEncodedStringFactory() : new RawStringFactory(),
                Debug = false
            };
            IRazorEngineService service = RazorEngineService.Create(config);

            if (additionalValues == null)
                return service.RunCompile(template, "templateKey", typeof(TType), model);
            else
                return service.RunCompile(template, template.GetHashCode().ToString(), typeof(TType), model, new DynamicViewBag(additionalValues));
        }
        public static string CompileTemplate(Dictionary<string, object> values, string template, TemplateFormat format = TemplateFormat.HTML)
        {
            TemplateServiceConfiguration config = new()
            {
                Language = RazorEngine.Language.CSharp,
                EncodedStringFactory = format == TemplateFormat.HTML ? new HtmlEncodedStringFactory() : new RawStringFactory(),
                Debug = false
            };
            IRazorEngineService service = RazorEngineService.Create(config);

            return service.RunCompile(template, template.GetHashCode().ToString(), null, null, new DynamicViewBag(values));
        }
        #endregion
    }
}
