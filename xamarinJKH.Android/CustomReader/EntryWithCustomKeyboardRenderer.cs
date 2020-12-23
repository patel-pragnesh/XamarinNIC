using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Android.Content;
using Android.InputMethodServices;
using System.Linq;
using Android.App;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using xamarinJKH;
using xamarinJKH.Droid.CustomReader;
using static Android.Resource;
using Exception = Java.Lang.Exception;
using Keyboard = Android.InputMethodServices.Keyboard;
using Keycode = Android.Views.Keycode;
using Object = Java.Lang.Object;
using RelativeLayout = Android.Widget.RelativeLayout;
using TextChangedEventArgs = Android.Text.TextChangedEventArgs;


[assembly: ExportRenderer(typeof(EntryWithCustomKeyboard), typeof(EntryWithCustomKeyboardRenderer))]

namespace xamarinJKH.Droid.CustomReader
{
    public class EntryWithCustomKeyboardRenderer : EntryRenderer, KeyboardView.IOnKeyboardActionListener
    {
        private Context context;

        private EntryWithCustomKeyboard entryWithCustomKeyboard;

        private KeyboardView mKeyboardView;
        private Keyboard mKeyboard;

        private InputTypes inputTypeToUse;

        private bool keyPressed;
        private KeyboardView.IOnKeyboardActionListener _onKeyboardActionListenerImplementation;

        public EntryWithCustomKeyboardRenderer(Context context) : base(context)
        {
            this.context = context;
        }

        private List<Keycode> keys = new List<Keycode>
        {
            Keycode.Num0,
            Keycode.Num1,
            Keycode.Num2,
            Keycode.Num3,
            Keycode.Num4,
            Keycode.Num5,
            Keycode.Num6,
            Keycode.Num7,
            Keycode.Num8,
            Keycode.Num9,
            Keycode.Comma,
            Keycode.Del,
            Keycode.Enter
        };
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            var newCustomEntryKeyboard = e.NewElement as EntryWithCustomKeyboard;
            var oldCustomEntryKeyboard = e.OldElement as EntryWithCustomKeyboard;

            if (newCustomEntryKeyboard == null && oldCustomEntryKeyboard == null)
                return;

            if (e.NewElement != null)
            {
                this.entryWithCustomKeyboard = newCustomEntryKeyboard;
                this.CreateCustomKeyboard();

                this.inputTypeToUse = this.entryWithCustomKeyboard.Keyboard.ToInputType() |
                                      InputTypes.TextFlagNoSuggestions;

                // Here we set the EditText event handlers
                this.EditText.FocusChange += Control_FocusChange;
                this.EditText.TextChanged += EditText_TextChanged;
                this.EditText.Click += EditText_Click;
                this.EditText.Touch += EditText_Touch;

                this.EditText.Background = null;
                var lp = new MarginLayoutParams(Control.LayoutParameters);
                lp.SetMargins(0, 0, 0, 10);
                LayoutParameters = lp;
                Control.LayoutParameters = lp;
                this.EditText.SetPadding(0, 0, 0, 0);
                SetPadding(0, 0, 0, 0);
            }

            // Dispose control
            if (e.OldElement != null)
            {
                this.EditText.FocusChange -= Control_FocusChange;
                this.EditText.TextChanged -= EditText_TextChanged;
                this.EditText.Click -= EditText_Click;
                this.EditText.Touch -= EditText_Touch;

                this.EditText.Background = null;
                var lp = new MarginLayoutParams(Control.LayoutParameters);
                lp.SetMargins(0, 0, 0, 10);
                LayoutParameters = lp;
                Control.LayoutParameters = lp;
                this.EditText.SetPadding(0, 0, 0, 0);
                SetPadding(0, 0, 0, 0);
            }
        }

        protected override void OnFocusChangeRequested(object sender, VisualElement.FocusRequestArgs e)
        {
            e.Result = true;

            if (e.Focus)
                this.Control.RequestFocus();
            else
                this.Control.ClearFocus();
        }

        #region EditText event handlers

        private void Control_FocusChange(object sender, FocusChangeEventArgs e)
        {
            // Workaround to avoid null reference exceptions in runtime
            if (this.EditText.Text == null)
                this.EditText.Text = string.Empty;

            if (e.HasFocus)
            {
                this.mKeyboardView.OnKeyboardActionListener = this;

                if (this.Element.Keyboard == Xamarin.Forms.Keyboard.Text)
                    this.CreateCustomKeyboard();

                this.ShowKeyboardWithAnimation();
            }
            else
            {
                // When the control looses focus, we set an empty listener to avoid crashes
                this.mKeyboardView.OnKeyboardActionListener = new NullListener();

                this.HideKeyboardView();
            }
        }

        private void EditText_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Ensure no key is pressed to clear focus
            if (this.EditText.Text.Length != 0 && !this.keyPressed)
            {
                this.EditText.ClearFocus();
                return;
            }
        }

        private void EditText_Click(object sender, System.EventArgs e)
        {
            ShowKeyboardWithAnimation();
        }

        private void EditText_Touch(object sender, TouchEventArgs e)
        {
            this.EditText.InputType = InputTypes.Null;

            this.EditText.OnTouchEvent(e.Event);

            this.EditText.InputType = this.inputTypeToUse;

            e.Handled = true;
        }

        #endregion

        #region keyboard related

        private void CreateCustomKeyboard()
        {
            var activity = Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity;

            var rootView = activity.FindViewById(global::Android.Resource.Id.Content);
            var activityRootView = (ViewGroup) ((ViewGroup) rootView).GetChildAt(0);

            this.mKeyboardView = activityRootView.FindViewById<KeyboardView>(Droid.Resource.Id.customKeyboard);

            // If the previous line fails, it means the keyboard needs to be created and added
            if (this.mKeyboardView == null)
            {
                this.mKeyboardView =
                    (KeyboardView) activity.LayoutInflater.Inflate(Resource.Layout.customkeyboard, null);
                this.mKeyboardView.Id = Resource.Id.customKeyboard;
                this.mKeyboardView.Focusable = true;
                this.mKeyboardView.FocusableInTouchMode = true;

                this.mKeyboardView.Release += (sender, e) => { };

                var layoutParams = new RelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
                layoutParams.AddRule(LayoutRules.AlignParentBottom);
                activityRootView.AddView(this.mKeyboardView, layoutParams);
            }
            EditText.SetTextIsSelectable(true);
            // EditText.SetCursorVisible(true);
            this.HideKeyboardView();

            this.mKeyboard = new Keyboard(this.context, Resource.Xml.special_keyboard);

            this.SetCurrentKeyboard();
        }

        private void SetCurrentKeyboard()
        {
            this.mKeyboardView.Keyboard = this.mKeyboard;
        }

        // Method to show our custom keyboard
        private void ShowKeyboardWithAnimation()
        {
            // First we must ensure that custom keyboard is hidden to
            // prevent showing it multiple times
            if (this.mKeyboardView.Visibility == ViewStates.Gone)
            {
                // Ensure native keyboard is hidden
                var imm = (InputMethodManager) this.context.GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(this.EditText.WindowToken, 0);

                this.EditText.InputType = InputTypes.Null;

                var animation = AnimationUtils.LoadAnimation(this.context, Resource.Animation.abc_fade_in);
                this.mKeyboardView.Animation = animation;

                this.mKeyboardView.Enabled = true;

                // Show custom keyboard with animation
                this.mKeyboardView.Visibility = ViewStates.Visible;
            }
        }

        // Method to hide our custom keyboard
        private void HideKeyboardView()
        {
            this.mKeyboardView.Visibility = ViewStates.Gone;
            this.mKeyboardView.Enabled = false;

            this.EditText.InputType = InputTypes.Null;
        }

        #endregion

        // Implementing IOnKeyboardActionListener interface


        public void OnKey(Keycode primaryCode, Keycode[] keyCodes)
        {
            if (!this.EditText.IsFocused)
                return;

            // Ensure key is pressed to avoid removing focus
            this.keyPressed = true;

            // Create event for key press
            long eventTime = JavaSystem.CurrentTimeMillis();

            var ev = new KeyEvent(eventTime, eventTime, KeyEventActions.Down, primaryCode, 0, 0, 0, 0,
                KeyEventFlags.SoftKeyboard | KeyEventFlags.KeepTouchMode);

            // Ensure native keyboard is hidden
            var imm = (InputMethodManager) this.context.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(this.EditText.WindowToken, HideSoftInputFlags.None);

            this.EditText.InputType = this.inputTypeToUse;

            if (!keys.Contains(ev.KeyCode))
            {
                return;
            }
            
            switch (ev.KeyCode)
            {
                case Keycode.Enter:
                    // Sometimes EditText takes long to update the HasFocus status
                    if (this.EditText.HasFocus)
                    {
                        // Close the keyboard, remove focus and launch command asociated action
                        this.HideKeyboardView();

                        this.ClearFocus();

                        this.entryWithCustomKeyboard.EnterCommand?.Execute(null);
                    }

                    break;

                case Keycode.Comma:
                    if (this.EditText.HasFocus)
                    {
                        string editTextText = EditText.Text;
                        int count = editTextText.Count(f => f == ',');
                        if (count >= 1)
                        {
                            return;
                        }

                        if (string.IsNullOrEmpty(editTextText))
                        {
                            return;
                        }

                        if (this.entryWithCustomKeyboard.DecimalPoint == 0)
                        {
                            return;
                        }
                        
                    }
                    break;
                case Keycode.Del:
                    break;
                default:
                    if (this.EditText.HasFocus)
                    {
                        string editTextText = EditText.Text;
                        if (editTextText.Length == this.entryWithCustomKeyboard.IntegerPoint && !editTextText.Contains(","))
                        {
                            return;
                        }
                        else
                        {
                            if (editTextText.Contains(","))
                            {
                                var numbers = editTextText.Split(',', '.');
                                int commaIndex = editTextText.IndexOf(",", StringComparison.Ordinal);
                                int cursorIndex = EditText.SelectionStart;
                                if (commaIndex < cursorIndex)
                                {
                                    if (numbers[1].Length == this.entryWithCustomKeyboard.DecimalPoint)
                                    {
                                        return;
                                    }
                                }
                                else
                                {
                                    if (numbers[0].Length == this.entryWithCustomKeyboard.IntegerPoint)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
    
            // Set the cursor at the end of the text
            // this.EditText.SetSelection(this.EditText.Text.Length);

            if (this.EditText.HasFocus)
            {
                this.DispatchKeyEvent(ev);

                this.keyPressed = false;
            }
        }

        void KeyboardView.IOnKeyboardActionListener.OnPress(Keycode primaryCode)
        {
            OnPress(primaryCode);
        }

        void KeyboardView.IOnKeyboardActionListener.OnRelease(Keycode primaryCode)
        {
            OnRelease(primaryCode);
        }

        public void OnPress([GeneratedEnum] Keycode primaryCode)
        {
        }

        public void OnRelease([GeneratedEnum] Keycode primaryCode)
        {
        }


        public void OnText(ICharSequence text)
        {
        }

        public void SwipeDown()
        {
        }

        public void SwipeLeft()
        {
        }

        public void SwipeRight()
        {
        }

        public void SwipeUp()
        {
        }

        private class NullListener : Object, KeyboardView.IOnKeyboardActionListener
        {
            public void OnKey(global::Android.Views.Keycode primaryCode, global::Android.Views.Keycode[] keyCodes)
            {
            }

            public void OnPress(global::Android.Views.Keycode primaryCode)
            {
            }

            public void OnRelease(global::Android.Views.Keycode primaryCode)
            {
            }

            public void OnText(ICharSequence text)
            {
            }

            public void SwipeDown()
            {
            }

            public void SwipeLeft()
            {
            }

            public void SwipeRight()
            {
            }

            public void SwipeUp()
            {
            }
        }
    }
}