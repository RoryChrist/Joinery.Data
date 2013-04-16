using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Joinery.Data
{
    public abstract class CommandBase<T>
    {
        // ---------------------------------------------------------------------------------------------
        // Public Constructor
        // ---------------------------------------------------------------------------------------------

        public CommandBase()
        {
            Parameters = new List<object>();
        }

        // ---------------------------------------------------------------------------------------------
        // Protected Properties
        // ---------------------------------------------------------------------------------------------

        protected StringBuilder ColumnList
        {
            get;
            private set;
        }

        protected StringBuilder JoinClause
        {
            get;
            private set;
        }

        protected List<object> Parameters
        {
            get;
            private set;
        }

        protected StringBuilder OrderbyClause
        {
            get;
            private set;
        }

        protected StringBuilder SetList
        {
            get;
            private set;
        }

        protected StringBuilder ValueList
        {
            get;
            private set;
        }

        protected StringBuilder WhereClause
        {
            get;
            private set;
        }

        // ---------------------------------------------------------------------------------------------
        // Protected Methods
        // ---------------------------------------------------------------------------------------------

        protected void AppendColumn(StringBuilder builder, MemberInfo member)
        {
            builder.AppendFormat("[{0}]", Mapper.GetColumnName(member));
        }

        protected void AppendConstant(StringBuilder builder, Type type, object value)
        {
            // TODO: Formatting for dates and numbers
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    builder.Append(((bool)value) ? "1" : "0");
                    break;

                case TypeCode.String:
                    builder.AppendFormat("'{0}'", value);
                    break;

                default:
                    builder.Append(value);
                    break;
            }
        }

        protected void AppendInsertColumnValue(MemberInfo column, object value)
        {
            InitializeColumnList();
            InitializeValueList();

            AppendColumn(ColumnList, column);
            AppendParameter(ValueList, value);
        }

        protected void AppendParameter(StringBuilder builder, object value)
        {
            builder.AppendFormat("@P{0}", Parameters.Count);

            Parameters.Add(value);
        }

        protected void AppendTable(StringBuilder builder, Type type)
        {
            builder.AppendFormat("[{0}]", Mapper.GetTableName(type));
        }

        protected void AppendUpdateColumnValue(MemberInfo column, object value)
        {
            InitializeSetList();

            AppendColumn(SetList, column);

            SetList.Append(" = ");

            AppendParameter(SetList, value);
        }

        protected void AppendWhereCondition(MemberInfo column, object value)
        {
            InitializeWhereClause();

            AppendColumn(WhereClause, column);

            WhereClause.Append(" = ");

            AppendParameter(WhereClause, value);
        }

        protected void ParseJoin<J>(Expression<Func<J, T, bool>> predicate)
        {
            InitializeJoinClause();

            AppendTable(JoinClause, typeof(J));

            JoinClause.Append(" ON ");

            Parse(JoinClause, predicate.Body);
        }

        protected void ParseOrderBy<R>(Expression<Func<T, R>> property, bool descending)
        {
            InitializeOrderbyClause();

            Parse(OrderbyClause, property.Body);

            if (descending)
            {
                OrderbyClause.Append(" DESC");
            }
        }

        protected void ParseSet<R>(Expression<Func<T, R>> property, R value)
        {
            InitializeSetList();

            Parse(SetList, property.Body);

            SetList.Append(" = ");

            AppendParameter(SetList, value);
        }

        protected void ParseSet<R>(Expression<Func<T, R>> property, Expression<Func<T, R>> value)
        {
            InitializeSetList();

            Parse(SetList, property.Body);

            SetList.Append(" = ");

            Parse(SetList, value.Body);
        }

        protected void ParseValue<R>(Expression<Func<T, R>> property, R value)
        {
            InitializeColumnList();
            InitializeValueList();

            Parse(ColumnList, property);

            AppendParameter(ValueList, value);
        }

        protected void ParseWhere(Expression<Func<T, bool>> predicate)
        {
            InitializeWhereClause();

            Parse(WhereClause, predicate.Body);
        }

        // ---------------------------------------------------------------------------------------------
        // Private Methods
        // ---------------------------------------------------------------------------------------------

        private void InitializeColumnList()
        {
            if (ColumnList == null)
            {
                ColumnList = new StringBuilder();
            }
            else
            {
                ColumnList.Append(", ");
            }
        }

        private void InitializeJoinClause()
        {
            if (JoinClause == null)
            {
                JoinClause = new StringBuilder();
            }
            else
            {
                JoinClause.Append(" INNER JOIN ");
            }
        }

        private void InitializeOrderbyClause()
        {
            if (OrderbyClause == null)
            {
                OrderbyClause = new StringBuilder();
            }
            else
            {
                OrderbyClause.Append(", ");
            }
        }

        private void InitializeSetList()
        {
            if (SetList == null)
            {
                SetList = new StringBuilder();
            }
            else
            {
                SetList.Append(", ");
            }
        }

        private void InitializeValueList()
        {
            if (ValueList == null)
            {
                ValueList = new StringBuilder();
            }
            else
            {
                ValueList.Append(", ");
            }
        }

        private void InitializeWhereClause()
        {
            if (WhereClause == null)
            {
                WhereClause = new StringBuilder();
            }
            else
            {
                WhereClause.Append(" AND ");
            }
        }

        private void Parse(StringBuilder builder, Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.NotEqual:
                case ExpressionType.Subtract:
                    ParseBinary((BinaryExpression)expression, builder);
                    break;

                case ExpressionType.Call:
                    ParseCall(builder, (MethodCallExpression)expression);
                    break;

                case ExpressionType.Constant:
                    ParseConstant(builder, (ConstantExpression)expression);
                    break;

                case ExpressionType.Convert:
                    ParseConvert(builder, (UnaryExpression)expression);
                    break;

                case ExpressionType.MemberAccess:
                    ParseMemberAccess(builder, (MemberExpression)expression);
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Unexpected expression type: {0}", expression.NodeType));
            }
        }

        private void ParseBinary(BinaryExpression binaryExpression, StringBuilder builder)
        {
            bool logicalOperator = (binaryExpression.NodeType == ExpressionType.AndAlso || binaryExpression.NodeType == ExpressionType.OrElse);

            if (logicalOperator)
            {
                builder.Append("(");
            }

            Parse(builder, binaryExpression.Left);

            if (logicalOperator)
            {
                builder.Append(")");
            }

            switch (binaryExpression.NodeType)
            {
                case ExpressionType.Add:
                    builder.Append(" + ");
                    break;

                case ExpressionType.AndAlso:
                    builder.Append(" AND ");
                    break;

                case ExpressionType.Equal:
                    builder.Append(" = ");
                    break;

                case ExpressionType.GreaterThan:
                    builder.Append(" > ");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    builder.Append(" >= ");
                    break;

                case ExpressionType.LessThan:
                    builder.Append(" < ");
                    break;

                case ExpressionType.LessThanOrEqual:
                    builder.Append(" <= ");
                    break;

                case ExpressionType.NotEqual:
                    builder.Append(" <> ");
                    break;

                case ExpressionType.OrElse:
                    builder.Append(" OR ");
                    break;

                case ExpressionType.Subtract:
                    builder.Append(" - ");
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Unexpected expression type: {0}", binaryExpression.NodeType));
            }

            if (logicalOperator)
            {
                builder.Append("(");
            }

            Parse(builder, binaryExpression.Right);

            if (logicalOperator)
            {
                builder.Append(")");
            }
        }

        private void ParseCall(StringBuilder builder, MethodCallExpression methodCallExpression)
        {
            Parse(builder, methodCallExpression.Object);

            switch (methodCallExpression.Method.Name)
            {
                case "Contains":
                    builder.Append(" LIKE '%' + ");
                    Parse(builder, methodCallExpression.Arguments[0]);
                    builder.Append(" + '%'");
                    break;

                case "EndsWith":
                    builder.Append(" LIKE '%' + ");
                    Parse(builder, methodCallExpression.Arguments[0]);
                    break;

                case "StartsWith":
                    builder.Append(" LIKE ");
                    Parse(builder, methodCallExpression.Arguments[0]);
                    builder.Append(" + '%'");
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Unsupported method call: {0}", methodCallExpression.Method.Name));
            }
        }

        private void ParseConstant(StringBuilder builder, ConstantExpression constantExpression)
        {
            AppendConstant(builder, constantExpression.Type, constantExpression.Value);
        }

        private void ParseConvert(StringBuilder builder, UnaryExpression unaryExpression)
        {
            Parse(builder, unaryExpression.Operand);
        }

        private void ParseMemberAccess(StringBuilder builder, MemberExpression memberExpression)
        {
            switch (memberExpression.Expression.NodeType)
            {
                case ExpressionType.Constant:
                    ParseMemberAccessConstant(builder, (ConstantExpression)memberExpression.Expression, memberExpression.Member);
                    break;

                case ExpressionType.Parameter:
                    ParseMemberAccessParameter(builder, memberExpression.Member, (ParameterExpression)memberExpression.Expression);
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Unexpected expression type: {0}", memberExpression.Expression.NodeType));
            }
        }

        private void ParseMemberAccessConstant(StringBuilder builder, ConstantExpression constantExpression, MemberInfo member)
        {
            object value = ((FieldInfo)member).GetValue(constantExpression.Value);

            AppendParameter(builder, value);
        }

        private void ParseMemberAccessParameter(StringBuilder builder, MemberInfo member, ParameterExpression parameterExpression)
        {
            AppendTable(builder, parameterExpression.Type);

            builder.Append('.');

            AppendColumn(builder, member);
        }
    }
}