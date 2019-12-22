using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;

namespace Aloha.Alerts.ViewModels
{
    class MyViewModel
    {
        private ObservableCollection<Appointment> appointments;

        public ObservableCollection<Appointment> Appointments
        {
            get
            {
                if (this.appointments == null)
                {
                    this.appointments = this.CreateAppointments();
                }
                return this.appointments;
            }
        }

        private ObservableCollection<Appointment> CreateAppointments()
        {
            ObservableCollection<Appointment> apps = new ObservableCollection<Appointment>();

            var app1 = new Appointment()
            {
                Subject = "Front-End Meeting",
                Start = DateTime.Today.AddHours(9),
                End = DateTime.Today.AddHours(10)
            };
            apps.Add(app1);

            var app2 = new Appointment()
            {
                Subject = "Planning Meeting",
                Start = DateTime.Today.AddHours(11),
                End = DateTime.Today.AddHours(12)
            };
            apps.Add(app2);

            return apps;
        }
    }
}
