using System.Linq.Expressions;

namespace Soul.SqlBatis.Infrastructure
{
    internal class DbSwitchCaseExpressionVisitor : DbExpressionVisitor
    {
        public DbSwitchCaseExpressionVisitor(IModel model, DynamicParameters parameters)
            : base(model, parameters)
        {

        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == nameof(DbOperations.Switch))
            {
                SetSql("(CASE WHEN ");
                Visit(node.Arguments[0]);
                SetSql(" THEN ");
                Visit(node.Arguments[1]);
            }
            else if (node.Method.Name == nameof(DbSwitchCase.Case))
            {
                if (node.Object != null)
                {
                    Visit(node.Object);
                }
                SetSql(" WHEN ");
                Visit(node.Arguments[0]);
                SetSql(" THEN ");
                Visit(node.Arguments[1]);
            }
            else if (node.Method.Name == nameof(DbSwitchCase.Default))
            {
                Visit(node.Object);
                SetSql(" ELSE ");
                Visit(node.Arguments[0]);
                SetSql(" END)");
            }
            return node;
        }
    }
}
