﻿using System;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.View;
using AndroidX.AppCompat.View.Menu;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Card;
using Madamin.Unfollow.Instagram;
using Square.Picasso;

namespace Madamin.Unfollow.ViewHolders
{
    internal class FanViewHolder : 
        RecyclerView.ViewHolder,
        MenuBuilder.ICallback
    {
        private readonly MaterialCardView _card;
        private readonly TextView _fullNameTextView;
        private readonly TextView _userNameTextView;
        private readonly ImageView _avatarImageView;

        private readonly MenuPopupHelper _popup;

        private readonly IFanItemClickListener _listener;

        public FanViewHolder(View item, IFanItemClickListener listener) : base(item)
        {
            _card = item.FindViewById<MaterialCardView>(Resource.Id.item_user_card);
            _fullNameTextView = item.FindViewById<TextView>(Resource.Id.item_user_fullname);
            _userNameTextView = item.FindViewById<TextView>(Resource.Id.item_user_username);
            _avatarImageView = item.FindViewById<ImageView>(Resource.Id.item_user_avatar);

            var menu = new MenuBuilder(ItemView.Context);
            menu.SetCallback(this);
            var inflater = new SupportMenuInflater(ItemView.Context);
            inflater.Inflate(Resource.Menu.popup_fan, menu);

            var optionMenuButton = item.FindViewById(Resource.Id.item_user_more);
            _popup = new MenuPopupHelper(ItemView.Context, menu);
            _popup.SetAnchorView(optionMenuButton);
            _popup.SetForceShowIcon(true);

            _listener = listener;

            if (optionMenuButton == null || _card == null)
                return;

            _card.Click += Item_Click;
            _card.LongClick += Item_LongClick;
            optionMenuButton.Click += More_Click;
        }

        public void BindData(User user, bool selected)
        {
            _fullNameTextView.Text = user.Fullname;
            _userNameTextView.Text = "@" + user.Username;
            _card.Checked = selected;
            Picasso.Get()
                .Load(user.ProfilePhotoUrl)
                .Placeholder(Resource.Drawable.ic_person_black_48dp)
                .Fit()
                .CenterCrop()
                .NoFade()
                .Error(Resource.Drawable.ic_error_black_48dp)
                .Into(_avatarImageView);
        }

        private void Item_Click(object sender, EventArgs e)
        {
            if (!_listener.OnItemClick(BindingAdapterPosition))
            {
                _listener.OnItemOpen(BindingAdapterPosition);
            }
        }

        private void Item_LongClick(object sender, View.LongClickEventArgs e)
        {
            _listener.OnItemLongClick(BindingAdapterPosition);
        }

        private void More_Click(object sender, EventArgs e)
        {
            _popup.Show();
        }

        bool MenuBuilder.ICallback.OnMenuItemSelected(MenuBuilder builder, IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.popup_fan_item_open:
                    _listener.OnItemOpen(BindingAdapterPosition);
                    return true;
                case Resource.Id.popup_fan_item_select:
                    _listener.OnItemSelect(BindingAdapterPosition);
                    return true;
                case Resource.Id.popup_fan_item_follow:
                    _listener.OnItemFollow(BindingAdapterPosition);
                    return true;
                case Resource.Id.popup_fan_item_add_whitelist:
                    _listener.OnItemAddToWhitelist(BindingAdapterPosition);
                    return true;
                default:
                    return false;
            }
        }

        void MenuBuilder.ICallback.OnMenuModeChange(MenuBuilder builder) {}
    }

    internal interface IFanItemClickListener
    {
        bool OnItemClick(int position);
        void OnItemLongClick(int position);
        void OnItemOpen(int position);
        void OnItemSelect(int position);
        void OnItemFollow(int position);
        void OnItemAddToWhitelist(int position);
    }
}