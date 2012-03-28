using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using RHITMobile.Secure.Data;
using RHITMobile.Secure.BannerTableAdapters;

namespace RHITMobile.Secure
{
    class DB
    {
        private static DB _instance = null;

        private bool _switch;

        private DB()
        { }

        public static DB Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DB();
                return _instance;
            }
        }

        public void Flip()
        {
            //TODO locking
            _switch = !_switch;
        }

        public void ClearData()
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            adapter.spDeleteData(!_switch);
        }

        public void AddUser(User user)
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            adapter.AddUser(!_switch, user);
        }

        public void SetAdvisor(User user)
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            adapter.spSetAdvisor(!_switch, user.Username, user.Advisor);
        }

        public void AddCourse(Course course)
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            adapter.AddCourse(!_switch, course);
            adapter.AddCourseSchedule(!_switch, course);
        }
        
        public void AddUserEnrollment(Enrollment enrollment)
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            adapter.AddUserEnrollment(!_switch, enrollment);
        }
        
        public User GetUser(String username)
        {
            GetUserDataTableAdapter adapter = new GetUserDataTableAdapter();
            Banner.GetUserDataDataTable table = adapter.GetData(_switch, username);
            return table.User;
        }
    }
}
