﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using Grocery.Core.Models.Enums;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class GroceryListViewModel : BaseViewModel
    {
        public ObservableCollection<GroceryList> GroceryLists { get; set; }
        private readonly IGroceryListService _groceryListService;
        private readonly GlobalViewModel _global;

        [ObservableProperty]
        string clientName = string.Empty;

        public GroceryListViewModel(IGroceryListService groceryListService, GlobalViewModel global) 
        {
            _groceryListService = groceryListService;
            _global = global;
            GroceryLists = new(_groceryListService.GetAll());
            Title = "Boodschappenlijst";
            clientName = _global.Client.Name;
        }

        [RelayCommand]
        public async Task SelectGroceryList(GroceryList groceryList)
        {
            Dictionary<string, object> paramater = new() { { nameof(GroceryList), groceryList } };
            await Shell.Current.GoToAsync($"{nameof(Views.GroceryListItemsView)}?Titel={groceryList.Name}", true, paramater);
        }
        public override void OnAppearing()
        {
            base.OnAppearing();
            GroceryLists = new(_groceryListService.GetAll());
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            GroceryLists.Clear();
        }

        [RelayCommand]
        public async Task ShowBoughtProducts()
        {
            if (_global.Client.Role == Role.Admin)
                await Shell.Current.GoToAsync(nameof(Views.BoughtProductsView));
        }
    }
}
