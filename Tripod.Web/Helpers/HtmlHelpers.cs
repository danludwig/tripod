using System;
using System.Linq.Expressions;
using System.Web.Mvc;

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

        public static MvcHtmlString CssStyleWhenInvalidFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string cssStyle)
        {
            if (Equals(html.ViewData.Model, null)) return null;

            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var isInvalid = !html.ViewData.ModelState.IsValidField(fieldName);
            return isInvalid ? MvcHtmlString.Create(cssStyle) : null;
        }

        public static MvcHtmlString CssStyleWhenValidFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string cssStyle)
        {
            // when model is null, assume it is valid
            if (Equals(html.ViewData.Model, null)) return MvcHtmlString.Create(cssStyle);

            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var isValid = html.ViewData.ModelState.IsValidField(fieldName);
            return isValid ? MvcHtmlString.Create(cssStyle) : null;
        }
    }
}