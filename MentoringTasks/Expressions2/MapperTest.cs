using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Expressions2
{
    public class Mapper<TSource, TDestination>
    {
        Func<TSource, TDestination> mapFunction;
        internal Mapper(Func<TSource, TDestination> func)
        {
            mapFunction = func;
        }
        public TDestination Map(TSource source)
        {
            return mapFunction(source);
        }
    }

    public class MappingGenerator
    {
        private Func<TSource, TDestination> CreateMapFunc<TSource, TDestination>()
        {
            var sourceExpr = Expression.Parameter(typeof(TSource));
            var ctorExpr = Expression.New(typeof(TDestination));
            var propBinders = sourceExpr.Type.GetProperties()
                .Select(prop => Expression.Bind(typeof(TDestination).GetProperty(prop.Name), Expression.Property(sourceExpr, prop)));
            var initExpr = Expression.MemberInit(ctorExpr, propBinders);
            var mapExpr = Expression.Lambda<Func<TSource, TDestination>>(initExpr, sourceExpr);
            var mapFunc = mapExpr.Compile();
            return mapFunc;
        }

        public Mapper<TSource, TDestination> Generate<TSource, TDestination>()
        {
            var mapFunction = CreateMapFunc<TSource, TDestination>();               
            return new Mapper<TSource, TDestination>(mapFunction);
        }
    }

    public class Foo
    {
        public string X { get; set; }
        public int Y { get; set; }
        public DateTime Z { get; set; }
        
        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Z: {Z}";
        }        
}

    public class Bar
    {
        public string X { get; set; }
        public int Y { get; set; }
        public DateTime Z { get; set; }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Z: {Z}";
        }
    }

    [TestClass]
    public class MapperTest
    {
        [TestMethod]
        public void TestMapper()
        {
            var mapGenerator = new MappingGenerator();
            var mapper = mapGenerator.Generate<Foo, Bar>();
            var f = new Foo {X = "X", Y = 5, Z = DateTime.Now};
            var b = mapper.Map(f);
            Console.WriteLine($"Foo: {f}\nBar: {b}");
        }
    }
}