﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nResult_task.ViewModel;

namespace nResultUnitTest
{
    [TestClass]
    public class NResultTests
    {

        // i was going to add mocks to make more reliable unit tests
        // but filter and sort feature took time more than my estimation 

        [TestMethod]
        public void TestPagesCount()
        {
            MainViewModel CustomerVm = new MainViewModel();
            int count = CustomerVm.PagesCount;
            Assert.IsTrue(count == 0);
        }

        [TestMethod]
        public void TestNavigation()
        {
            MainViewModel CustomerVm = new MainViewModel();
            bool gotoFirst = CustomerVm.FirstEnabled;
            Assert.IsTrue(gotoFirst == false);
        }
    }
}
