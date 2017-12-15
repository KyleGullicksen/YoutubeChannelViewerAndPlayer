using NUnit.Framework;
using System;
using Week9PrismExampleApp.ViewModels;

namespace Week9PrismExampleUnitTests
{
    [TestFixture()]
    public class EasyTestPageViewModelTests
    {
        EasyTestPageViewModel easyTestPageViewModel;

        [SetUp]
        public void Init()
        {
            easyTestPageViewModel = new EasyTestPageViewModel();
        }

        [Test]
        public void TestReturnOneReturnsOnes()
        {
            var result = easyTestPageViewModel.ReturnOne();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void TestAddItemToCollectionAddsItemToCollection()
        {
            string itemToAdd = "Thomas";
            easyTestPageViewModel.AddItemToCollection(itemToAdd);
            CollectionAssert.Contains(easyTestPageViewModel.CollectionOfNames, itemToAdd);
        }

        [Test]
        [TestCase(1, 1, 2)]
		[TestCase(0, 0, 0)]
		[TestCase(100, 52, 152)]
		[TestCase(-1, 10, 9)]
		public void TestAddTwoNumbersReturnsSumOfNumbers(int firstNumber, int secondNumber, int expectedResult)
        {
            var result = easyTestPageViewModel.AddTwoNumbers(firstNumber, secondNumber);

            Assert.AreEqual(expectedResult, result);
        }
    }
}
