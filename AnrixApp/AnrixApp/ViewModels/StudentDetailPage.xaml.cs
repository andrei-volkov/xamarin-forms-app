﻿using AnrixApp.Models;
using Plugin.Settings;
using System;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static AnrixApp.ViewModels.GroupsPage;

namespace AnrixApp.ViewModels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StudentDetailPage : ContentPage
	{
        private Student CurrentStudent;

		public StudentDetailPage (Student student)
		{
            BindingContext = student;
            CurrentStudent = student;
		    InitializeComponent ();
            if (student is StudentRequest)
            { 
                ToolbarItems.Remove(SecondBarItem);
                ToolbarItems.Remove(ThirdBarItem);   
            }

            var Group =  GlobalFaculty.GetGroups()
                                      .Where(g => g.NumberOfGroup.Equals(CurrentStudent.NumberOfGroup))
                                      .FirstOrDefault();
            if (Group != null)
            {
                var Groupsmates = Group.GetStudents();
                Groupsmates.Remove(CurrentStudent);
                StudentsList.ItemsSource = Groupsmates;

            }
            var color = Color.FromHex(CrossSettings.Current.GetValueOrDefault("Color", "000000"));
            BigStack.BackgroundColor = color;
            CachedImage.BackgroundColor = color;
            popupImageView.BackgroundColor = color;
            BigGrid.BackgroundColor = color;
            Stack1.BackgroundColor = color;
            Stack2.BackgroundColor = color;
            Label1.BackgroundColor = color;

        }

        private async void ToolbarItem_Clicked(object sender, System.EventArgs e)
        {
            var a = GlobalFaculty;
            a.RemoveStudent(BindingContext as Student);
            UpdateList(a);
            await Navigation.PopAsync();
        }

        private void Show_Popup(object sender, EventArgs e)
        {
            if (!(CurrentStudent is StudentRequest))
            {
                popupImageView.IsVisible = true;
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var a = GlobalFaculty;
            popupImageView.IsVisible = false;
            CurrentStudent.PhotoUrl = UserLink.Text ?? "big_student_face.png";

            UserLink.Text = null;
            BindingContext = null;
            BindingContext = CurrentStudent;
            UpdateList(a);

        }

        private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = $"Name: {CurrentStudent.Name}\n" +
                $"Surname: {CurrentStudent.Surname}\n" +
                $"Patronymic: {CurrentStudent.Patronymic}\n" +
                $"Average mark: {CurrentStudent.AverageMark}\n" +
                $"Group: {CurrentStudent.NumberOfGroup}\n"  
                + (CurrentStudent.PhotoUrl.Equals("big_student_face.png") ? 
                "No photo" : $"{CurrentStudent.PhotoUrl}")
            });
        }

        private void ToolbarItem_Clicked_2(object sender, EventArgs e)
        {
            StudentTitle.IsVisible = false;
            Patronymic.IsVisible = false;
            BigGrid.IsVisible = false; 

            EditStack.IsVisible = true;

            EditName.IsTabStop = false;
            EditSurname.IsTabStop = false;
            EditPatronymic.IsTabStop = false;
            EditAverageMark.IsTabStop = false;

        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            try {
                var number = double.Parse(EditAverageMark.Text);
                if (number > 10 || number < 0)
                    throw new ArgumentException("Mark can't be <0 or >10");

                AllContent.IsVisible = false;
                Animation.IsVisible = true;
                Animation.Play();
              

            } catch(Exception ex)
            {
                await DisplayAlert("Error occurred", "Invalid mark\n" +
                    $"Info: {ex.Message}", "OK");
            }

        }

        private async void StudentsList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new StudentDetailPage(e.Item as Student));
        }

        private void Animation_OnFinish(object sender, EventArgs e)
        {
            var number = double.Parse(EditAverageMark.Text);
            AllContent.IsVisible = true;
            Animation.IsVisible = false;

            var a = GlobalFaculty;

            StudentTitle.IsVisible = true;
            Patronymic.IsVisible = true;
            BigGrid.IsVisible = true;

            EditStack.IsVisible = false;

            CurrentStudent.AverageMark = number;
            CurrentStudent.Name = EditName.Text;
            CurrentStudent.Surname = EditSurname.Text;
            CurrentStudent.Patronymic = EditPatronymic.Text;
            CurrentStudent.Title = Title = EditName.Text + " " + EditSurname.Text;

            BindingContext = null;
            BindingContext = CurrentStudent;
            UpdateList(a);
        }
    }
}