using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Samples.Helpers;
using Samples.ViewModel;
using Xamarin.Essentials;

namespace Samples.Model;

public class PermissionItem : ObservableObject
{
    public PermissionItem(string title, Permissions.BasePermission permission)
    {
        Title = title;
        Permission = permission;
        Status = PermissionStatus.Unknown;
    }

    public string Title { get; set; }

    public string? Rationale { get; set; }

    public PermissionStatus Status { get; set; }

    public Permissions.BasePermission Permission { get; set; }

    public ICommand CheckStatusCommand =>
        new Xamarin.Essentials.Command(async () =>
        {
            try
            {
                Status = await Permission.CheckStatusAsync();
                OnPropertyChanged(nameof(Status));
            }
            catch (Exception ex)
            {
                MessagingCenter.Send<PermissionItem, Exception>(this, nameof(PermissionException), ex);
            }
        });

    public ICommand RequestCommand =>
        new Xamarin.Essentials.Command(async () =>
        {
            try
            {
                Status = await Permission.RequestAsync();
                OnPropertyChanged(nameof(Status));
            }
            catch (Exception ex)
            {
                MessagingCenter.Send<PermissionItem, Exception>(this, nameof(PermissionException), ex);
            }
        });

    public ICommand ShouldShowRationaleCommand =>
        new Xamarin.Essentials.Command(() =>
        {
            try
            {
                Rationale = $"Should show rationale: {Permission.ShouldShowRationale()}";
                OnPropertyChanged(nameof(Rationale));
            }
            catch (Exception ex)
            {
                MessagingCenter.Send<PermissionItem, Exception>(this, nameof(PermissionException), ex);
            }
        });
}
