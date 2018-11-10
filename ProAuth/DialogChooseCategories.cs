﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using ProAuth.Data;
using ProAuth.Utilities;
using DialogFragment = Android.Support.V4.App.DialogFragment;

namespace ProAuth
{
    internal class DialogChooseCategories : DialogFragment
    {
        public int AuthPosition { get; }

        private readonly CategorySource _categorySource;
        private readonly Action<bool, int> _itemClick;
        private readonly EventHandler _onClose;

        private readonly List<string> _checkedCategories;

        private RecyclerView _categoryList;
        private ChooseCategoriesAdapter _categoryAdapter;

        public DialogChooseCategories(CategorySource categorySource, EventHandler onClose, Action<bool, int> itemClick, int authPosition, List<string> checkedCategories)
        {
            _categorySource = categorySource;
            _itemClick = itemClick;
            _onClose = onClose;
            _checkedCategories = checkedCategories;
            AuthPosition = authPosition;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(Activity);
            alert.SetTitle(Resource.String.chooseCategories);
            alert.SetCancelable(false);
            alert.SetPositiveButton(Resource.String.ok, (EventHandler<DialogClickEventArgs>) null);

            View view = Activity.LayoutInflater.Inflate(Resource.Layout.dialogChooseCategories, null);
            _categoryList = view.FindViewById<RecyclerView>(Resource.Id.dialogChooseCategories_list);
            alert.SetView(view);

            AlertDialog dialog = alert.Create();
            dialog.Show();

            LinearLayoutManager layout = new LinearLayoutManager(Context);
            DividerItemDecoration decoration = new DividerItemDecoration(Context, layout.Orientation);
            _categoryList.AddItemDecoration(decoration);
            _categoryList.SetLayoutManager(layout);

            _categoryAdapter = new ChooseCategoriesAdapter(_categorySource);
            _categoryAdapter.ItemClick += _itemClick;

            _categoryList.SetAdapter(_categoryAdapter);
            _categoryList.HasFixedSize = true;
            _categoryList.SetItemViewCacheSize(20);

            Button okButton = dialog.GetButton((int) DialogButtonType.Positive);
            okButton.Click += _onClose.Invoke;

            foreach(string category in _checkedCategories)
            {
                int index = _categorySource.Categories.FindIndex(c => c.Id == category);
                _categoryAdapter.CheckedStatus[index] = true;
            }

            return dialog;
        }
    }
}