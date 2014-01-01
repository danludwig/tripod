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
    }
}