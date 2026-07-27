using PagoDirecto.Domain.Enums;
using System.Linq.Expressions;
using System.Reflection;

namespace PagoDirecto.Application.Extensions
{
    public static class ExtendQuery
    {
        public static IQueryable<T> OrderQuery<T>(this IQueryable<T> query, string tipoOrdenQueryApi, string propertyName)
        {
            if (tipoOrdenQueryApi != "asc" && tipoOrdenQueryApi != "desc")
            {
                tipoOrdenQueryApi = "asc";
            }
            var entityType = typeof(T);
            var queryType = tipoOrdenQueryApi == "asc" ? QueryOrderType.Ascending : QueryOrderType.Descending;
            propertyName = propertyName ?? typeof(T).Name + "Id";

            if (entityType.GetProperty(propertyName) != null)
            {
                string order = string.Empty;
                switch (queryType)
                {
                    case QueryOrderType.Ascending:
                        order = "OrderBy";
                        break;
                    case QueryOrderType.Descending:
                        order = "OrderByDescending";
                        break;
                }

                //Create x=>x.PropName
                var propertyInfo = entityType.GetProperty(propertyName);
                ParameterExpression arg = Expression.Parameter(entityType, "x");
                MemberExpression property = Expression.Property(arg, propertyName);
                var selector = Expression.Lambda(property, new ParameterExpression[] { arg });

                //Get System.Linq.Queryable.OrderBy() method.
                var enumarableType = typeof(System.Linq.Queryable);
                var method = enumarableType.GetMethods()
                     .Where(m => m.Name == order && m.IsGenericMethodDefinition)
                     .Where(m =>
                     {
                         var parameters = m.GetParameters().ToList();
                         //Put more restriction here to ensure selecting the right overload                
                         return parameters.Count == 2;//overload that has 2 parameters
                     }).Single();
                //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
                MethodInfo genericMethod = method
                     .MakeGenericMethod(entityType, propertyInfo.PropertyType);

                /*Call query.OrderBy(selector), with query and selector: x=> x.PropName
                  Note that we pass the selector as Expression to the method and we don't compile it.
                  By doing so EF can extract "order by" columns and generate SQL for it.*/
                var newQuery = (IOrderedQueryable<T>)genericMethod
                     .Invoke(genericMethod, new object[] { query, selector });
                return newQuery;
            }

            return query;
        }
    }
}

