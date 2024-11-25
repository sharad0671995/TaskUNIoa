using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TaskManagementSystem.FileUpload
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class DisableFormValueModelBindingAttribute:Attribute,IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            throw new NotImplementedException();
        }

        public  void OnResourceExecuting(ResourceExecutingContext context)
        {
            var factories = context.ValueProviderFactories;
            factories.RemoveType<FormValueProviderFactory>();
            factories.RemoveType<FormFileValueProviderFactory>();
            factories.RemoveType<JQueryFormValueProviderFactory>();

        }
    }
}
