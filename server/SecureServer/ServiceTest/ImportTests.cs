using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RHITMobile.Secure.Data_Import;
using System.Diagnostics;
using RHITMobile.Secure;
using RHITMobile.Secure.Data;

namespace ServiceTest
{
    public class DBTest
    {
        protected DB db { get; private set; }

        protected void ClearDB()
        {
            db = DB.Instance;
            using (var writeLock = db.AcquireWriteLock())
            {
                db.ClearData();
                db.Flip();
            }
            using (var writeLock = db.AcquireWriteLock())
            {
                db.ClearData();
                db.Flip();
            }
        }
    }

    [TestClass]
    public class LockTests : DBTest
    {
        private User GetUser()
        {
            User user = new User();
            user.Username = "username";
            user.Mailbox = 1234;
            user.Alias = "username@rose-hulman.edu";
            user.FirstName = "First";
            user.LastName = "Last";
            return user;
        }

        [TestInitialize]
        public void Setup()
        {
            ClearDB();
        }

        [TestMethod]
        public void TestFlips()
        {
            DB db = DB.Instance;
            User user = GetUser();
            using (var writeLock = db.AcquireWriteLock())
            {
                db.ClearData();
                db.AddUser(user);
                db.Flip();
            }
            user = GetUser();
            user.Username = "username2";
            using (var writeLock = db.AcquireWriteLock())
            {
                db.ClearData();
                db.AddUser(user);
                db.Flip();
            }
            Assert.IsNull(db.GetUser("username"));
            Assert.AreEqual(user, db.GetUser("username2"));
        }
    }

    [TestClass]
    public class ImportTests : DBTest
    {
        [TestInitialize]
        public void Setup()
        {
            ClearDB();
        }

        [TestMethod]
        public void TestImport()
        {
            Importer importer = new Importer(new ConsoleLogger(), "../../../ServiceTest/InputData");
            importer.ImportData();
        }
    }
}
