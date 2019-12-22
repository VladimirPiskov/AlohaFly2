using System;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;

namespace Aloha.Alerts
{
    public static class SampleContentService
    {
        public static ObservableCollection<OutlookSection> GetOutlookSections()
        {
            var result = new ObservableCollection<OutlookSection>();
            var calendarGroups = new ObservableCollection<CheckBoxItem>()
            {
                new CheckBoxItem() { Text = "Personal" },
                new CheckBoxItem() { Text = "Team" },
                new CheckBoxItem() { Text = "Company" }
            };
            result.Add(new OutlookSection
            {
                Name = "Calendar",
                Content = calendarGroups,
                SelectedItem = DateTime.Today,
                IconPath = "../Images/calendar_32x32.png",
                MinimizedIconPath = "../Images/calendar_16x16.png"
            });

            return result;
        }

        public static ObservableCollection<Appointment> GetCalendarAppointments()
        {
            var result = new ObservableCollection<Appointment>();
            var today = DateTime.Today;
            SampleContentService.AddAppointmentsForTheWeek(result, today, "Morning Meeting", "Team", "CalendarType", new TimeSpan(8, 0, 0), new TimeSpan(9, 0, 0));
            SampleContentService.AddAppointmentsForTheWeek(result, today, "Coffee Break", "Personal", "CalendarType", new TimeSpan(14, 0, 0), new TimeSpan(14, 30, 0));
            SampleContentService.AddAppointmentsForTheWeek(result, today, "Client Meeting", "Company", "CalendarType", new TimeSpan(16, 0, 0), new TimeSpan(17, 0, 0));
            var friday = today.AddDays(-(int)today.DayOfWeek).AddDays(5);
            var releaseParty = new Appointment()
            {
                Start = new DateTime(friday.Year, friday.Month, friday.Day, 18, 0, 0),
                End = new DateTime(friday.Year, friday.Month, friday.Day, 23, 0, 0),
                Subject = "Release Party"
            };
            result.Add(releaseParty);

            return result;
        }

        private static void AddAppointmentsForTheWeek(ObservableCollection<Appointment> result, DateTime today, string subject, string resourceName, string resourceType, TimeSpan start, TimeSpan end)
        {
            var monday = today.AddDays(-(int)today.DayOfWeek).AddDays(1);
            var pattern = new RecurrencePattern()
            {
                Frequency = RecurrenceFrequency.Daily,
                MaxOccurrences = 5
            };
            var appointment = new Appointment()
            {
                Start = new DateTime(monday.Year, monday.Month, monday.Day, start.Hours, start.Minutes, start.Seconds),
                End = new DateTime(monday.Year, monday.Month, monday.Day, end.Hours, end.Minutes, end.Seconds),
                Subject = subject,
                RecurrenceRule = new RecurrenceRule(pattern)
            };
            appointment.Resources.Add(new Resource(resourceName, resourceType));
            result.Add(appointment);
        }
    }
}