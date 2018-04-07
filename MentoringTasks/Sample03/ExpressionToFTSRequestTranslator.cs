using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sample03
{
	public class ExpressionToFTSRequestTranslator : ExpressionVisitor
	{
		StringBuilder resultString;

		public string Translate(Expression exp)
		{
			resultString = new StringBuilder();
			Visit(exp);

			return resultString.ToString();
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node.Method.DeclaringType == typeof(Queryable)
				&& node.Method.Name == "Where")
			{
				var predicate = node.Arguments[1];
				Visit(predicate);

				return node;
            }

            var arg = node.Arguments[0] as ConstantExpression;
		    if (arg != null)
		    {
		        var newArg = new List<ConstantExpression>();
                switch (node.Method.Name)
		        {
		            case "StartsWith":
		                newArg.Add(Expression.Constant($"{arg.Value}*", typeof(string)));
		                break;
                    case "EndsWith":
                        newArg.Add(Expression.Constant($"*{arg.Value}", typeof(string)));
                        break;
                    case "Contains":
                        newArg.Add(Expression.Constant($"*{arg.Value}*", typeof(string)));
                        break;
                }
                node = node.Update(node.Object, newArg);
		    }

            return base.VisitMethodCall(node);
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			switch (node.NodeType)
			{
                case ExpressionType.Equal:
			        var case1 = node.Left.NodeType == ExpressionType.Constant &&
			                    node.Right.NodeType == ExpressionType.MemberAccess;
			        var case2 = node.Left.NodeType == ExpressionType.MemberAccess &&
			                    node.Right.NodeType == ExpressionType.Constant;
                    
			        if (!(case1 || case2))
			        {
			            throw new NotSupportedException("Left/Right operand should be property, field or constant");
			        }

			        if (case1)
			        {
                        Visit(node.Right);
                        Visit(node.Left);
                        break;
			        }

			        Visit(node.Left);
					Visit(node.Right);
					break;

                case ExpressionType.AndAlso:
                    Visit(node.Left);
                    resultString.Append("&&");
                    Visit(node.Right);
                    break;

                default:
                    throw new NotSupportedException(string.Format("Operation {0} is not supported", node.NodeType));
            }

            return node;
		}

		protected override Expression VisitMember(MemberExpression node)
		{
            resultString.Append(node.Member.Name).Append(":(");
            return base.VisitMember(node);
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			resultString.Append($"{node.Value})");
            return node;
		}
	}
}
