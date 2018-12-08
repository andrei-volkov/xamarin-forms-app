﻿using AnrixApp.Models;
using AnrixApp.Services;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace AnrixApp.ViewModels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GroupsPage : ContentPage
	{
        public delegate void ListUpdated(Faculty faculty);
        public static event ListUpdated OnListUpdated;
        public static Faculty Faculty;

        public GroupsPage ()
		{
            OnListUpdated += delegate (Faculty faculty)
            {
                BindingContext = null;
                BindingContext = faculty.getGroups();
            };
            InitializeComponent();        
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                FileData filedata = await CrossFilePicker.Current.PickFile();
                Faculty = FileReaderService.ReadFromFile(filedata.FilePath);

                OnListUpdated(Faculty);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error occur", $"Something go wrong: {ex.Message}", "Back");
            }
        }

        private void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            Faculty = MockFacultyData.getMockicngFaculty();
            OnListUpdated(Faculty);
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var content = e.Item as Group;
            await Navigation.PushAsync(new StudentsPage(content));
        }

        public static void UpdateList(Faculty faculty) => OnListUpdated(faculty);
    }
}