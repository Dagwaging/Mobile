using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using RHITMobile.Secure.Data;
using RHITMobile.Secure.BannerTableAdapters;
using System.Threading;
using System.Data;

namespace RHITMobile.Secure
{
    public class DB
    {
        private static DB _instance = null;

        private SwitchLockData _switchLock = new SwitchLockData();

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

        public int ActiveReaderCount
        {
            get
            {
                return _switchLock.ActiveReaders;
            }
        }

        public int AffectedRows { get; private set; }

        public IDisposable AcquireWriteLock()
        {
            AffectedRows = 0;
            return _switchLock.AcquireWriteLock();
        }

        public void Flip()
        {
            _switchLock.Flip();
        }

        public void ClearData()
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            adapter.spDeleteData(_switchLock.WriteSwitch);
        }

        public void AddUser(User user)
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            AffectedRows += adapter.AddUser(_switchLock.WriteSwitch, user);
        }

        public void SetAdvisor(User user)
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            AffectedRows += adapter.spSetAdvisor(_switchLock.WriteSwitch, user.Username, user.Advisor);
        }

        public void AddCourse(Course course)
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            AffectedRows += adapter.AddCourse(_switchLock.WriteSwitch, course);
            AffectedRows += adapter.AddCourseSchedule(_switchLock.WriteSwitch, course);
        }
        
        public void AddUserEnrollment(Enrollment enrollment)
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            AffectedRows += adapter.AddUserEnrollment(_switchLock.WriteSwitch, enrollment);
        }
        
        public User GetUser(String username)
        {
            using (SwitchLock switchLock = AcquireReadSwitch())
            {
                GetUserTableAdapter adapter = new GetUserTableAdapter();
                Banner.GetUserDataTable table = adapter.GetData(switchLock.Switch, username);
                return table.User;
            }
        }
        
        public User[] SearchUsers(String search)
        {
            using (SwitchLock switchLock = AcquireReadSwitch())
            {
                SearchUsersTableAdapter adapter = new SearchUsersTableAdapter();
                Banner.SearchUsersDataTable table = adapter.GetData(switchLock.Switch, search);
                return table.Users;
            }
        }
        
        public Course GetCourse(int term, int crn)
        {
            using (SwitchLock switchLock = AcquireReadSwitch())
            {
                GetCourseTableAdapter adapter = new GetCourseTableAdapter();
                Banner.GetCourseDataTable table = adapter.GetData(switchLock.Switch, term, crn);
                return table.Course;
            }
        }

        public Course[] SearchCourses(String search)
        {
            using (SwitchLock switchLock = AcquireReadSwitch())
            {
                SearchCoursesTableAdapter adapter = new SearchCoursesTableAdapter();
                Banner.SearchCoursesDataTable table = adapter.GetData(switchLock.Switch, search);
                return table.Courses;
            }
        }
        
        public string[] GetCourseEnrollment(int term, int crn)
        {
            using (SwitchLock switchLock = AcquireReadSwitch())
            {
                GetCourseEnrollmentTableAdapter adapter = new GetCourseEnrollmentTableAdapter();
                Banner.GetCourseEnrollmentDataTable table = adapter.GetData(switchLock.Switch, term, crn);
                return table.Enrollment;
            }
        }
        
        public UserEnrollment[] GetUserEnrollment(string username)
        {
            using (SwitchLock switchLock = AcquireReadSwitch())
            {
                GetUserEnrollmentTableAdapter adapter = new GetUserEnrollmentTableAdapter();
                Banner.GetUserEnrollmentDataTable table = adapter.GetData(switchLock.Switch, username);
                return table.Enrollment;
            }
        }
        
        public CourseTime[] GetCourseSchedule(int term, int crn)
        {
            using (SwitchLock switchLock = AcquireReadSwitch())
            {
                GetCourseScheduleTableAdapter adapter = new GetCourseScheduleTableAdapter();
                Banner.GetCourseScheduleDataTable table = adapter.GetData(switchLock.Switch, term, crn);
                return table.Schedule;
            }
        }
        
        public RoomSchedule[] GetRoomSchedule(string room)
        {
            using (SwitchLock switchLock = AcquireReadSwitch())
            {
                GetRoomScheduleTableAdapter adapter = new GetRoomScheduleTableAdapter();
                Banner.GetRoomScheduleDataTable table = adapter.GetData(switchLock.Switch, room);
                return table.Schedule;
            }
        }

        private SwitchLock AcquireReadSwitch()
        {
            return new SwitchLock(_switchLock);
        }
    }


    class Counter
    {
        public int Count { get; private set; }

        public void Increment()
        {
            Count++;
        }

        public void Decrement()
        {
            Count--;
        }
    }

    class SwitchLockData
    {
        private bool _switch;

        private Counter _count1 = new Counter();
        private Counter _count2 = new Counter();
        
        private WriteLock _writeLock;

        public SwitchLockData()
        {
            _switch = Properties.Settings.Default.Switch;
        }

        private Counter ReadCounter()
        {
            if (_switch)
                return _count1;
            else
                return _count2;
        }
        
        private Counter WriteCounter()
        {
            if (_switch)
                return _count2;
            else
                return _count1;
        }

        public int ActiveReaders
        {
            get
            {
                lock (this)
                {
                    return ReadCounter().Count + WriteCounter().Count;
                }
            }
        }

        public bool AcquireSwitch()
        {
            lock (this)
            {
                ReadCounter().Increment();
                return _switch;
            }
        }

        public bool WriteSwitch
        {
            get
            {
                if (_writeLock == null)
                    throw new InvalidOperationException("Write lock not held");

                return !_switch;
            }
        }

        public WriteLock AcquireWriteLock()
        {
            //Make sure there isn't already a writelock
            if (_writeLock != null)
                throw new InvalidOperationException("Active write lock already in use");

            while (WriteCounter().Count > 0)
                Thread.Sleep(5000);

            lock(this)
            {
                _writeLock = new WriteLock(this);
            }

            return _writeLock;
        }

        public void ReleaseWriteLock(WriteLock writeLock)
        {
            if (_writeLock != writeLock)
                throw new InvalidOperationException("WriteLock not active");

            _writeLock = null;
        }

        public void ReleaseSwitch(bool val)
        {
            lock (this)
            {
                if (val == _switch)
                {
                    ReadCounter().Decrement();
                }
                else
                {
                    WriteCounter().Decrement();
                }
            }
        }

        public void Flip()
        {
            lock (this)
            {
                _switch = !_switch;
                Properties.Settings.Default.Switch = _switch;
                Properties.Settings.Default.Save();
            }

        }
    }

    class WriteLock : IDisposable
    {
        private SwitchLockData _lockData;
        
        public WriteLock(SwitchLockData lockData)
        {
            _lockData = lockData;
        }

        public void Dispose()
        {
            _lockData.ReleaseWriteLock(this);
        }
    }
        
    class SwitchLock : IDisposable
    {
        private SwitchLockData _lockData;

        public SwitchLock(SwitchLockData lockData)
        {
            _lockData = lockData;
            Switch = lockData.AcquireSwitch();
        }

        public bool Switch { get; private set; }

        public void Dispose()
        {
            _lockData.ReleaseSwitch(Switch);
        }
    }

}
