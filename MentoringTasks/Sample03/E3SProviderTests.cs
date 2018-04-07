using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample03.E3SClient.Entities;
using Sample03.E3SClient;
using System.Configuration;
using System.Linq;

namespace Sample03
{
	[TestClass]
	public class E3SProviderTests
	{
		[TestMethod]
		public void WithoutProvider()
		{
			var client = new E3SQueryClient(ConfigurationManager.AppSettings["user"] , ConfigurationManager.AppSettings["password"]);
			var res = client.SearchFTS<EmployeeEntity>("workstation:(epbygrow0286)", 0, 1);

			foreach (var emp in res)
			{
				Console.WriteLine("{0} {1}", emp.nativeName, emp.shortStartWorkDate);
			}
		}

		[TestMethod]
		public void WithoutProviderNonGeneric()
		{
			var client = new E3SQueryClient(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);
			var res = client.SearchFTS(typeof(EmployeeEntity), "workstation:(*028*)&&city:(Minsk)", 0, 10);

			foreach (var emp in res.OfType<EmployeeEntity>())
			{
				Console.WriteLine("{0} {1}", emp.nativeName, emp.shortStartWorkDate);
			}
		}

        [TestMethod]
		public void RunConstantFirst()
		{
			var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

			foreach (var emp in employees.Where(e => "Aliaksandr" == e.firstName))
			{
				Console.WriteLine($"{emp.displayName}, city: {emp.citySum}");
			}
        }

        [TestMethod]
		public void RunStartsWith()
		{
			var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

			foreach (var emp in employees.Where(e => e.firstName.StartsWith("Ali")))
			{
				Console.WriteLine($"{emp.displayName.Replace("Ali", "ALI")}, city: {emp.citySum}");
			}
        }

        [TestMethod]
        public void RunEndsWith()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            foreach (var emp in employees.Where(e => e.firstName.EndsWith("sandr")))
            {
                Console.WriteLine($"{emp.displayName.Replace("sandr", "SANDR")}, city: {emp.citySum}");
            }
        }

        [TestMethod]
        public void RunContains()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            foreach (var emp in employees.Where(e => e.firstName.Contains("aksan")))
            {
                Console.WriteLine($"{emp.displayName.Replace("aksan", "AKSAN")}, city: {emp.citySum}");
            }
        }

        [TestMethod]
        public void RunStartsAndEnds()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            foreach (var emp in employees.Where(e => e.firstName.StartsWith("Ali") && e.lastName.EndsWith("kin")))
            {
                Console.WriteLine($"{emp.displayName.Replace("Ali", "ALI").Replace("kin", "KIN")}, city: {emp.citySum}");
            }
        }
    }
}
