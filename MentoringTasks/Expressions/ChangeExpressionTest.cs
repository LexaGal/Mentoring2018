using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Expressions
{
    [TestClass]
    public class ChangeExpressionTest
    {
        public class IncDecTransformer : ExpressionVisitor
        {
            protected override Expression VisitBinary(BinaryExpression node)
            {
                if (node.NodeType == ExpressionType.Add)
                {
                    ParameterExpression param = null;
                    ConstantExpression constant = null;
                    if (node.Left.NodeType == ExpressionType.Parameter)
                    {
                        param = (ParameterExpression)node.Left;
                    }
                    else if (node.Left.NodeType == ExpressionType.Constant)
                    {
                        constant = (ConstantExpression)node.Left;
                    }

                    if (node.Right.NodeType == ExpressionType.Parameter)
                    {
                        param = (ParameterExpression)node.Right;
                    }
                    else if (node.Right.NodeType == ExpressionType.Constant)
                    {
                        constant = (ConstantExpression)node.Right;
                    }

                    if (param != null && constant != null && constant.Type == typeof(int) && (int)constant.Value == 1)
                    {
                        return Expression.Increment(param);
                    }
                }
                else if (node.NodeType == ExpressionType.Subtract)
                {
                    ParameterExpression param = null;
                    ConstantExpression constant = null;
                    if (node.Left.NodeType == ExpressionType.Parameter)
                    {
                        param = (ParameterExpression)node.Left;
                    }

                    if (node.Right.NodeType == ExpressionType.Constant)
                    {
                        constant = (ConstantExpression)node.Right;
                    }

                    if (param != null && constant != null && constant.Type == typeof(int) && (int)constant.Value == 1)
                    {
                        return Expression.Decrement(param);
                    }
                }
                return base.VisitBinary(node);
            }
        }

        public class ReplaceParamsTransformer : ExpressionVisitor
        {
            Expression _expression;
            IDictionary<string, object> _replaceDict;

            public ReplaceParamsTransformer(Expression<Func<int, int, int, int, int>> expression,
                params Expression<Func<string, object>>[] replaceFuncs)
            {
                _replaceDict = new Dictionary<string, object>();
                _expression = expression;
                foreach (var func in replaceFuncs)
                {
                    _replaceDict.Add(func.Parameters[0].Name, func.Compile().Invoke(string.Empty));
                }
            }

            public LambdaExpression ReplaceWithConstant()
            {
                return (LambdaExpression)Visit(_expression);
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                var expr = new IncDecTransformer().VisitAndConvert(node, string.Empty);
                Console.WriteLine(expr);
                var parameters = expr.Parameters.Where(p => !_replaceDict.ContainsKey(p.Name));
                return Expression.Lambda(Visit(expr.Body), parameters);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node.NodeType == ExpressionType.Parameter && _replaceDict.ContainsKey(node.Name))
                {
                    var obj = _replaceDict[node.Name];
                    return Expression.Constant(obj);
                }
                return base.VisitParameter(node);
            }
        }

        [TestMethod]
        public void IncrDecTransformTest()
        {
            Expression<Func<int, int>> sourceExp = a => a + (a + 1) * (a + 10 - 1) + (a - 1);
            var tr = new IncDecTransformer();
            var transformered = tr.VisitAndConvert(sourceExp, string.Empty);
            Console.WriteLine($"{sourceExp} = {sourceExp.Compile().Invoke(10)}");
            Console.WriteLine($"{transformered} = {transformered.Compile().Invoke(10)}");
        }

        [TestMethod]
        public void ReplaceParamsTransformTest()
        {
            Expression<Func<int, int, int, int, int>> sourceExp =
                (a, b, c, d) => a + (a + 1) * (a + 10 - 1) + (a - 1) - (b + c) + d;
            var tr = new ReplaceParamsTransformer(sourceExp, a => 1, b => 2, c => 3, d => 4);
            var replacedParamsExpression = tr.ReplaceWithConstant();
            Console.WriteLine($"{replacedParamsExpression} = {replacedParamsExpression.Compile().DynamicInvoke()}");
        }
        
    }
}