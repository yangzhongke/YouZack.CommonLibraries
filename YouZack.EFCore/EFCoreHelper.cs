using Infrastructures.EFCore;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore
{
    public static class EFCoreHelper
    {
        /// <summary>
        /// set global 'IsDeleted=false' queryfilter for every entity
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void EnableGlobal_IsDeleted_Filter(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var isDeletedProperty = entityType.FindProperty(nameof(BaseEntity.IsDeleted));
                if (isDeletedProperty != null && isDeletedProperty.ClrType == typeof(bool))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "p");
                    var filter = Expression.Lambda(Expression.Not(Expression.Property(parameter, isDeletedProperty.PropertyInfo)), parameter);
                    entityType.SetQueryFilter(filter);//EFCore 2.x中是QueryFilter属性，在3.x中改成了SetQueryFilter扩展方法
                }
            }
        }

        /// <summary>
        /// 得到表名
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public static string GetTableName<TEntity>(this DbContext dbCtx)
        {
            var entityType = dbCtx.Model.FindEntityType(typeof(TEntity));
            if (entityType == null)
            {
                throw new ArgumentOutOfRangeException("TEntity is nof found in DbContext");
            }
            return entityType.GetTableName();
        }

        /// <summary>
        /// 得到实体中属性对应的列名
        /// 用法：string fName = dbCtx.GetColumnName<Person>(p=>p.Name);
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="propertyLambda"></param>
        /// <returns></returns>

        public static string GetColumnName<TEntity>(this DbContext dbCtx, Expression<Func<TEntity, object>> propertyLambda)
        {
            var entityType = dbCtx.Model.FindEntityType(typeof(TEntity));
            if (entityType == null)
            {
                throw new ArgumentOutOfRangeException("TEntity is nof found in DbContext");
            }

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
            {
                var unary = propertyLambda.Body as UnaryExpression;
                if (unary != null)
                {
                    member = unary.Operand as MemberExpression;
                }
            }
            if (member == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));
            }

            Type type = typeof(TEntity);
            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(), type));
            }

            string propertyName = propInfo.Name;
            return entityType.FindProperty(propertyName).GetColumnName();
        }
    }
}
