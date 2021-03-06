﻿using System.Windows;
using System.Windows.Input;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.Model.Services;
using Rhit.Applications.Model.Events;
using System;

namespace Rhit.Applications.ViewModel.Models {
    public class LoginViewModel : DependencyObject {

        public LoginViewModel() {
            LoginStatus = "Login to Access Data";
            LoginCommand = new RelayCommand(p => Login());
            DataCollector.Instance.LoginRequestReturned += new AuthenticationEventHandler(LoginRequestReturned);
            DataCollector.Instance.ServerErrorReturned += new ServiceEventHandler(ServerErrorReturned);
        }

        void ServerErrorReturned(object sender, ServiceEventArgs e) {
            LoginStatus = "Login Failed: Invalid credentials.";
        }

        public event EventHandler Authenticated;

        protected virtual void OnAuthenticated(EventArgs e) {
            if(Authenticated != null) Authenticated(this, e);
        }

        void LoginRequestReturned(object sender, AuthenticationEventArgs e) {
            if(!e.Authorized) {
                LoginStatus = "Login Failed: Invalid credentials.";
                return;
            }
            OnAuthenticated(new EventArgs());
        }


        #region Commands
        public ICommand LoginCommand { get; private set; }
        #endregion

        #region Properties for Dependency Properties
        public string LoginStatus {
            get { return (string) GetValue(LoginStatusProperty); }
            set { SetValue(LoginStatusProperty, value); }
        }

        public string UserName {
            get { return (string) GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        public string Password {
            get { return (string) GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }
        #endregion

        #region Dependency Properties
        public static readonly DependencyProperty LoginStatusProperty =
            DependencyProperty.Register("LoginStatus", typeof(string), typeof(LoginViewModel), new PropertyMetadata(""));

        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(LoginViewModel), new PropertyMetadata(""));

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(LoginViewModel), new PropertyMetadata(""));
        #endregion

        public void Login() {
            if(!CanLogin()) {
                LoginStatus = "Please enter both your user name and password.";
                return;
            }
            DataCollector.Instance.Login(Dispatcher, UserName, Password);
        }

        private bool CanLogin() {
            if(UserName != string.Empty && Password != string.Empty)
                return true;
            return false;
        }
    }
}
