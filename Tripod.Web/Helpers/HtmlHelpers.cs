using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Tripod.Web
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString BootstrapValidationCssClassFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
        {
            if (Equals(html.ViewData.Model, null)) return null;

            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var isValid = html.ViewData.ModelState.IsValidField(fieldName);
            var cssClass = isValid ? "has-success" : "has-error";
            return MvcHtmlString.Create(cssClass);
        }

        public static MvcHtmlString CssClassWhenInvalidFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string cssClass)
        {
            if (Equals(html.ViewData.Model, null)) return null;

            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var isInvalid = !html.ViewData.ModelState.IsValidField(fieldName);
            return isInvalid ? MvcHtmlString.Create(cssClass) : null;
        }

        public static MvcHtmlString CssClassWhenValidFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string cssClass)
        {
            // when model is null, assume it is valid
            if (Equals(html.ViewData.Model, null)) return MvcHtmlString.Create(cssClass);

            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var isValid = html.ViewData.ModelState.IsValidField(fieldName);
            return isValid ? MvcHtmlString.Create(cssClass) : null;
        }

        public static MvcHtmlString CssClassWhenNullModel(this HtmlHelper html, string cssClass)
        {
            return Equals(html.ViewData.Model, null) ? MvcHtmlString.Create(cssClass) : null;
        }

        public static MvcHtmlString CssClassWhenNotNullModel(this HtmlHelper html, string cssClass)
        {
            return Equals(html.ViewData.Model, null) ? null : MvcHtmlString.Create(cssClass);
        }

        [UsedImplicitly]
        public static MvcHtmlString CssStyleWhenInvalidFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string cssStyle)
        {
            if (Equals(html.ViewData.Model, null)) return null;

            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var isInvalid = !html.ViewData.ModelState.IsValidField(fieldName);
            return isInvalid ? MvcHtmlString.Create(cssStyle) : null;
        }

        [UsedImplicitly]
        public static MvcHtmlString CssStyleWhenValidFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string cssStyle)
        {
            // when model is null, assume it is valid
            if (Equals(html.ViewData.Model, null)) return MvcHtmlString.Create(cssStyle);

            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var isValid = html.ViewData.ModelState.IsValidField(fieldName);
            return isValid ? MvcHtmlString.Create(cssStyle) : null;
        }

        public static MvcHtmlString ValidationMessageTextFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
        {
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullHtmlFieldName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);

            if (!html.ViewData.ModelState.ContainsKey(fullHtmlFieldName))
                return null;
            var modelState = html.ViewData.ModelState[fullHtmlFieldName];
            var modelErrorCollection = modelState == null ? null : modelState.Errors;
            var error = modelErrorCollection == null || modelErrorCollection.Count == 0
                ? null
                : modelErrorCollection.FirstOrDefault(m => !string.IsNullOrEmpty(m.ErrorMessage))
                ?? modelErrorCollection[0];
            return error != null ? MvcHtmlString.Create(error.ErrorMessage) : null;
        }

        public static MvcHtmlString ValueForJavaScriptString<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, char escapeQuote = '\'')
        {
            var valueFor = html.ValueFor(expression);
            if (valueFor == null) return null;

            var valueForRaw = valueFor.ToString();
            valueForRaw = valueForRaw.Replace(@"\", @"\\");
            switch (escapeQuote)
            {
                case '\'':
                    valueForRaw = valueForRaw.Replace("'", @"\'").Replace("&#39;", @"\'");
                    break;
                case '"':
                    valueForRaw = valueForRaw.Replace(@"""", @"\""");
                    break;
            }

            return MvcHtmlString.Create(valueForRaw);
        }

        private static T ViewBagged<T>(object value)
        {
            if (value == null) return default(T);
            return (T)value;
        }

        public static int AuthenticatedUserId(this HtmlHelper html)
        {
            return ViewBagged<int>(html.ViewBag.AuthenticatedUserId);
        }

        public static MvcHtmlString Gravatar(this HtmlHelper html, string hash, int size, string cssClass = null)
        {
            const string srcFormat = "http://www.gravatar.com/avatar/{0}.jpg?s={1}&d=wavatar&r=PG";
            var src = string.Format(srcFormat, hash, size);
            var tagBuilder = new TagBuilder("img");
            tagBuilder.Attributes.Add("src", src);
            tagBuilder.Attributes.Add("alt", "Avatar icon");
            cssClass = !string.IsNullOrWhiteSpace(cssClass) ? string.Format("{0} gravatar", cssClass) : "gravatar";
            tagBuilder.Attributes.Add("class", cssClass);
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString Gravatar(this HtmlHelper html, int size, string cssClass = null)
        {
            var hash = html.ViewBag.AuthenticatedUserGravatar;
            return html.Gravatar((string) hash, size, cssClass);
        }
    }
}