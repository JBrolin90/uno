﻿using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI;
using Windows.UI.Input.Preview.Injection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using FluentAssertions;
using MUXControlsTestApp.Utilities;
using Uno.Disposables;
using Uno.Extensions;
using Uno.UI.RuntimeTests.Helpers;
using Uno.UI.Xaml.Core;
using static Private.Infrastructure.TestServices;
using Color = Windows.UI.Color;
using Point = Windows.Foundation.Point;

namespace Uno.UI.RuntimeTests.Tests.Windows_UI_Xaml_Controls
{
	/// <summary>
	/// This partial is for testing the skia-based TextBox implementation.
	/// Most tests here should set UseOverlayOnSkia to false and HideCaret
	/// to true and then set them back at the end of the test.
	/// </summary>
	public partial class Given_TextBox
	{
		[TestMethod]
		public async Task When_Basic_Input()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox();

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var text = "Hello world";
			foreach (var c in text)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}

			await WindowHelper.WaitForIdle();
			Assert.AreEqual(text, SUT.Text);
			Assert.AreEqual(11, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Basic_Input_With_ArrowKeys()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox();

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			foreach (var c in "world")
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}

			await WindowHelper.WaitForIdle();
			Assert.AreEqual(5, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			for (int i = 1; i <= 5; i++)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.None));
				await WindowHelper.WaitForIdle();
				Assert.AreEqual(5 - i, SUT.SelectionStart);
				Assert.AreEqual(0, SUT.SelectionLength);
			}

			foreach (var c in "Hello ")
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}

			await WindowHelper.WaitForIdle();
			Assert.AreEqual("Hello world", SUT.Text);
			Assert.AreEqual(6, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Basic_Input_With_Home_End()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox();

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			foreach (var c in "world")
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Home, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			foreach (var c in "Hello ")
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}

			await WindowHelper.WaitForIdle();
			Assert.AreEqual("Hello world", SUT.Text);
			Assert.AreEqual(6, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.End, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(11, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Selection_With_Keyboard_NoMod_And_Shift()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox();

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			foreach (var c in "Hello world")
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}

			await WindowHelper.WaitForIdle();

			for (var i = 1; i <= 11; i++)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.Shift));
				await WindowHelper.WaitForIdle();
				Assert.AreEqual(11 - i, SUT.SelectionStart);
				Assert.AreEqual(i, SUT.SelectionLength);
			}

			for (var i = 1; i <= 5; i++)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift));
				await WindowHelper.WaitForIdle();
				Assert.AreEqual(i, SUT.SelectionStart);
				Assert.AreEqual(11 - i, SUT.SelectionLength);
			}

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(5, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.End, VirtualKeyModifiers.Shift));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(5, SUT.SelectionStart);
			Assert.AreEqual(6, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(11, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Home, VirtualKeyModifiers.Shift));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(11, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(11, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Ctrl_A()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Text = "hello world"
			};

			var keyDownCount = 0;
			SUT.KeyDown += (_, _) => keyDownCount++;

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.A, VirtualKeyModifiers.Control, unicodeKey: 'a'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, keyDownCount);
			Assert.AreEqual(SUT.Text.Length, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.A, VirtualKeyModifiers.Control, unicodeKey: 'a'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, keyDownCount);
			Assert.AreEqual(SUT.Text.Length, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Ctrl_Home_End()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				AcceptsReturn = true,
				Text = "lorem\nipsum\r\ndolor"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.End, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(SUT.Text.Length, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Home, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Ctrl_Delete()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Text = "lorem ipsum dolor"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Delete, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("ipsum dolor", SUT.Text);
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Delete, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("dolor", SUT.Text);
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Ctrl_Backspace()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Text = "lorem ipsum dolor"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.Select(SUT.Text.Length, 0);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Back, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("lorem ipsum ", SUT.Text);
			Assert.AreEqual(SUT.Text.Length, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Back, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("lorem ", SUT.Text);
			Assert.AreEqual(SUT.Text.Length, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Enter_But_Not_Multiline()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Text = "hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.Select(2, 0);
			await WindowHelper.WaitForIdle();

			var size = SUT.ActualSize;

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Enter, VirtualKeyModifiers.None, unicodeKey: '\r'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hello world", SUT.Text);
			Assert.AreEqual(2, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
			Assert.AreEqual(size, SUT.ActualSize);
		}

		[TestMethod]
		public async Task When_Selection_With_Keyboard_NoMod_Ctrl_And_Shift()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox();

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			foreach (var c in "Hello &(%&^( w0.rld")
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Home, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(6, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(13, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(15, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(16, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(19, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Text_Bigger_Than_TextBox()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 40
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			foreach (var c in "This should be a lot longer than the width of the TextBox.")
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}
			await WindowHelper.WaitForIdle();

			var sv = SUT.FindVisualChildByType<ScrollViewer>();
			sv.HorizontalOffset.Should().BeGreaterThan(0);
			Assert.AreEqual(sv.ScrollableWidth, sv.HorizontalOffset);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Home, VirtualKeyModifiers.None));
			sv.ScrollableWidth.Should().BeGreaterThan(0);
			Assert.AreEqual(0, sv.HorizontalOffset);
		}

		[TestMethod]
		public async Task When_KeyDown_Bubbles_Out()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 100,
				Text = "Hello world"
			};

			var keyDownCount = 0;
			SUT.KeyDown += (_, _) => keyDownCount++;

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(1, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(1, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.End, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(1, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.End, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(2, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(3, keyDownCount);


			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.Select(2, 0);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Up, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(4, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Down, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(4, keyDownCount);
			Assert.AreEqual(SUT.Text.Length, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Down, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(5, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Back, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(6, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.Select(0, 0);
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Back, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(7, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Delete, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(7, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.Select(SUT.Text.Length, 0);
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Delete, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(7, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.A, VirtualKeyModifiers.None, unicodeKey: 'A'));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(8, keyDownCount);
		}

		[TestMethod]
		public async Task When_Selection_Initial_Then_Text_Changed()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 40,
				Text = "Initial"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.Select(SUT.Text.Length, 0);
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(SUT.Text.Length, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.Text = "Changed";

			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_ReadOnly()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 40,
				Text = "Initial",
				IsReadOnly = true
			};

			var keyDownCount = 0;
			SUT.KeyDown += (_, _) => keyDownCount++;

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.A, VirtualKeyModifiers.None, unicodeKey: 'A'));

			await WindowHelper.WaitForIdle();
			Assert.AreEqual("Initial", SUT.Text);
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
			Assert.AreEqual(1, keyDownCount);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.None));

			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
			Assert.AreEqual(2, keyDownCount);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None));

			await WindowHelper.WaitForIdle();
			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
			Assert.AreEqual(2, keyDownCount);
		}

		[TestMethod]
		public async Task When_Long_Text_Unfocused()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 80,
				Text = "This should be a lot longer than the width of the TextBox."
			};

			var btn = new Button();

			var sp = new StackPanel
			{
				Children =
				{
					SUT,
					btn
				}
			};


			WindowHelper.WindowContent = sp;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			var sv = SUT.FindVisualChildByType<ScrollViewer>();

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.Select(SUT.Text.Length, 0);
			await WindowHelper.WaitForIdle();
			// DeleteButton Takes space to the right of sv
			LayoutInformation.GetLayoutSlot(SUT).Right.Should().BeGreaterThan(LayoutInformation.GetLayoutSlot(sv).Right + 10);

			btn.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			LayoutInformation.GetLayoutSlot(SUT).Right.Should().BeLessThan(LayoutInformation.GetLayoutSlot(sv).Right + 10);
		}

		[TestMethod]
		public async Task When_Scrolling_Updates_With_Movement()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 40,
				Text = "This should be a lot longer than the width of the TextBox."
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			var sv = SUT.FindVisualChildByType<ScrollViewer>();

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.Select(SUT.Text.Length, 0);
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(sv.ScrollableWidth, sv.HorizontalOffset);

			for (var i = 0; i < 6; i++)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.None));
				await WindowHelper.WaitForIdle();
				Assert.AreEqual(sv.ScrollableWidth, sv.HorizontalOffset);
			}

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			sv.HorizontalOffset.Should().BeLessThan(sv.ScrollableWidth);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Home, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, sv.HorizontalOffset);

			for (var i = 0; i < 7; i++)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None));
				await WindowHelper.WaitForIdle();
				sv.HorizontalOffset.Should().BeApproximately(0, 3); // CI reports different numbers than local, probably because of scaling difference, hence the tolerance
			}

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			sv.HorizontalOffset.Should().BeGreaterThan(0);
		}

		[TestMethod]
		public async Task When_Scrolling_Updates_After_Backspace()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "This should be a lot longer than the width of the TextBox."
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			var sv = SUT.FindVisualChildByType<ScrollViewer>();

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.Select(SUT.Text.Length, 0);
			await WindowHelper.WaitForIdle();
			// DeleteButton Takes space to the right of sv
			LayoutInformation.GetLayoutSlot(SUT).Right.Should().BeGreaterThan(LayoutInformation.GetLayoutSlot(sv).Right + 10);

			var svRight = LayoutInformation.GetLayoutSlot(SUT).Right;
			for (var i = 0; i < 5; i++)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Back, VirtualKeyModifiers.None));
				await WindowHelper.WaitForIdle();
				Assert.AreEqual(svRight, LayoutInformation.GetLayoutSlot(SUT).Right);
			}

			for (var i = 0; i < 10; i++)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Back, VirtualKeyModifiers.Control));
			}
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, sv.ScrollableWidth);
		}

		[TestMethod]
		public async Task When_Pointer_Tap()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Pointer_RightClick_No_Selection()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.PressRight();
			mouse.ReleaseRight();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Pointer_RightClick_Selection()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.Select(2, 2);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.PressRight();
			mouse.ReleaseRight();
			await WindowHelper.WaitForIdle();

			mouse.MoveBy(100, 0); // click out
			mouse.Press();
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(2, SUT.SelectionStart);
			Assert.AreEqual(2, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Pointer_Hold_Drag()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			mouse.MoveBy(-50, 0);

			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(9, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Pointer_Hold_Drag_OutOfBounds()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			mouse.MoveBy(0, 50);
			mouse.MoveBy(-150, 0);

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(10, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_LongText_Pointer_Hold_Drag_OutOfBounds()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "This should be a lot longer than the width of the TextBox."
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			mouse.MoveBy(0, 50);
			mouse.MoveBy(-150, 0);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(10, SUT.SelectionLength);

			mouse.MoveBy(0, 50);
			mouse.MoveBy(600, 0);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(SUT.Text.Length - 10, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Chunk_DoubleTapped()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			// double tap
			mouse.Press();
			mouse.Release();
			mouse.Press();
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(6, SUT.SelectionStart);
			Assert.AreEqual(5, SUT.SelectionLength);

			// the selection should start on the left
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.Shift));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(6, SUT.SelectionStart);
			Assert.AreEqual(4, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Chunk_DoubleTapHeld()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			// double tap
			mouse.Press();
			mouse.Release();
			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(6, SUT.SelectionStart);
			Assert.AreEqual(5, SUT.SelectionLength);

			mouse.MoveBy(-40, 0);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(SUT.Text.Length, SUT.SelectionLength);

			mouse.Release();
			await WindowHelper.WaitForIdle();

			// the selection should start on the right
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(SUT.Text.Length - 1, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Chunk_TripleTapped()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			// double tap
			mouse.Press();
			mouse.Release();
			mouse.Press();
			mouse.Release();
			mouse.Press();
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(SUT.Text.Length, SUT.SelectionLength);

			// the selection should start on the left
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.Shift));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(SUT.Text.Length - 1, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Typing_While_Pointer_Held()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			mouse.MoveBy(-50, 0);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(9, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("Hello world", SUT.Text);
			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(9, SUT.SelectionLength);
		}

		[TestMethod]
		[DataRow(VirtualKey.Left, VirtualKeyModifiers.None)]
		[DataRow(VirtualKey.Right, VirtualKeyModifiers.None)]
		[DataRow(VirtualKey.Up, VirtualKeyModifiers.None)]
		[DataRow(VirtualKey.Down, VirtualKeyModifiers.None)]
		[DataRow(VirtualKey.Home, VirtualKeyModifiers.None)]
		[DataRow(VirtualKey.End, VirtualKeyModifiers.None)]
		[DataRow(VirtualKey.Back, VirtualKeyModifiers.None)]
		[DataRow(VirtualKey.Delete, VirtualKeyModifiers.None)]
		[DataRow(VirtualKey.A, VirtualKeyModifiers.Control)]
		public async Task When_Move_Caret_While_Pointer_Held(VirtualKey key, VirtualKeyModifiers modifiers)
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			var handled = false;
			SUT.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler((_, e) => handled = e.Handled), true);

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			mouse.MoveBy(-50, 0);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(9, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, key, modifiers));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(false, handled);
			Assert.AreEqual("Hello world", SUT.Text);
			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(9, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Cut_While_Pointer_Held()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			mouse.MoveBy(-50, 0);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(9, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.X, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("Hd", SUT.Text);

			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			mouse.MoveBy(10, -1);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(1, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Paste_While_Pointer_Held()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var dp = new DataPackage();
			var text = "copied content";
			dp.SetText(text);
			Clipboard.SetContent(dp);

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			mouse.MoveBy(-50, 0);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(9, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.V, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("Hcopied contentd", SUT.Text);
			Assert.AreEqual(15, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			mouse.MoveBy(0, -1); // nudge the mouse a bit to recalculate selection, this is the behaviour on WinUI as well
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(2, SUT.SelectionStart);
			Assert.AreEqual(13, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Escape_While_Pointer_Held()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			mouse.MoveBy(-50, 0);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(9, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Escape, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();

			mouse.MoveBy(-10, 0);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(9, SUT.SelectionLength);

			// We're pretty much "not pressed" at all at this point, even if we're technically still holding the mouse
			// so we can actually type stuff in!

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.A, VirtualKeyModifiers.None, unicodeKey: 'a'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("Had", SUT.Text);
			Assert.AreEqual(2, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_NonAscii_Characters()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox();

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var text = "صباح الخير";
			foreach (var c in text)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}

			await WindowHelper.WaitForIdle();
			Assert.AreEqual(text, SUT.Text);
		}

		[TestMethod]
		public async Task When_Copy_Paste()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150
			};

			var handled = false;
			SUT.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler((_, args) => handled |= args.Handled), true);

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			var dp = new DataPackage();
			var text = "copied content";
			dp.SetText(text);
			Clipboard.SetContent(dp);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.V, VirtualKeyModifiers.Control, unicodeKey: 'v'));
			await WindowHelper.WaitForIdle();

			Assert.IsFalse(handled);
			Assert.AreEqual(text, SUT.Text);

			SUT.Select(2, 4);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.C, VirtualKeyModifiers.Control, unicodeKey: 'c'));
			await WindowHelper.WaitForIdle();

			Assert.IsFalse(handled);
			Assert.AreEqual(SUT.Text.Substring(2, 4), await Clipboard.GetContent()!.GetTextAsync());
			Assert.AreEqual(2, SUT.SelectionStart);
			Assert.AreEqual(4, SUT.SelectionLength);

			SUT.Select(SUT.Text.Length - 1, 0);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.V, VirtualKeyModifiers.Control, unicodeKey: 'v'));
			await WindowHelper.WaitForIdle();

			Assert.IsFalse(handled);
			Assert.AreEqual("copied contenpiedt", SUT.Text);
			Assert.AreEqual(SUT.Text.Length - 1, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.Select(6, 3);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.V, VirtualKeyModifiers.Control, unicodeKey: 'v'));
			await WindowHelper.WaitForIdle();

			Assert.IsFalse(handled);
			Assert.AreEqual("copiedpiedntenpiedt", SUT.Text);
			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Cut_Paste()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			var handled = false;
			SUT.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler((_, args) => handled |= args.Handled), true);

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Select(2, 4);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.X, VirtualKeyModifiers.Control, unicodeKey: 'x'));
			await WindowHelper.WaitForIdle();

			Assert.IsFalse(handled);
			Assert.AreEqual("llo ", await Clipboard.GetContent()!.GetTextAsync());
			Assert.AreEqual("Heworld", SUT.Text);
			Assert.AreEqual(2, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.Select(SUT.Text.Length - 1, 0);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.V, VirtualKeyModifiers.Control, unicodeKey: 'v'));
			await WindowHelper.WaitForIdle();

			Assert.IsFalse(handled);
			Assert.AreEqual("Heworlllo d", SUT.Text);
			Assert.AreEqual(SUT.Text.Length - 1, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.Select(6, 3);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.V, VirtualKeyModifiers.Control, unicodeKey: 'v'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("Heworlllo  d", SUT.Text);
			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Multiline_Simple()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				AcceptsReturn = true
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Text = "hello";
			await WindowHelper.WaitForIdle();

			var height = SUT.ActualHeight;

			SUT.Text = "hello\rworld";
			await WindowHelper.WaitForIdle();

			SUT.ActualHeight.Should().BeGreaterThan(height * 1.2);
		}

		[TestMethod]
		public async Task When_Multiline_LineFeed()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				AcceptsReturn = true
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Text = "lorem\nipsum\r\ndolor";

			await WindowHelper.WaitForIdle();
			Assert.AreEqual("lorem\ripsum\rdolor", SUT.Text);
		}

		[TestMethod]
		public async Task When_Multiline_Return_Selected()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				AcceptsReturn = true
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Text = "hello\rworld";
			SUT.Select(4, 0);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("o\r", SUT.SelectedText);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("o\rw", SUT.SelectedText);
		}

		[TestMethod]
		public async Task When_Up_WithWithout_Shift()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Text = "Lorem ipsum"
			};

			var keyDownCount = 0;
			SUT.KeyDown += (_, _) => keyDownCount++;

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Select(4, 0);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Up, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(4, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
			Assert.AreEqual(1, keyDownCount);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Up, VirtualKeyModifiers.Shift));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(4, SUT.SelectionLength);
			Assert.AreEqual(1, keyDownCount);
		}

		[TestMethod]
		public async Task When_Multiline_NewLine_UpDown()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				AcceptsReturn = true,
				Text = "Lorem ipsum\rdolor sit\ramet consectetur\radipiscing"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.Select(17, 0);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Up, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(4, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Down, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(16, SUT.SelectionStart); // notice how up -> down -> up doesn't necessarily end up back where it started, this is correct
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Up, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(4, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Down, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(16, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Multiline_Wrapping_UpDown()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 120,
				TextWrapping = TextWrapping.Wrap,
				Text = "Lorem ipsum dolor sit amet consectetur adipiscing"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.Select(17, 0);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Up, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(4, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Down, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(16, SUT.SelectionStart); // notice how up -> down -> up doesn't necessarily end up back where it started, this is correct
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Up, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(4, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Down, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(16, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Multiline_NewLine_LeftRight()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				AcceptsReturn = true,
				Text = "Lorem ipsum\rdolor sit\ramet consectetur\radipiscing"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.Select(11, 0);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(12, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Multiline_Wrapping_LeftRight()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 120,
				TextWrapping = TextWrapping.Wrap,
				Text = "Lorem ipsum dolor sit amet consectetur adipiscing"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.Select(11, 0);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(12, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Multiline_Keyboard_Chunking()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				AcceptsReturn = true,
				Text =
				"""
				Lorem 
				     
				
				ipsum

				&&^
				    
				
				
				"""
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(6, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(7, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(12, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(13, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(14, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(19, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(20, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(21, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(24, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(25, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(29, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(30, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(31, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Multiline_Text_Ends_In_Return()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				AcceptsReturn = true,
				Text = "hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForLoaded(SUT);
			await WindowHelper.WaitForIdle();

			var height = SUT.ActualHeight;

			SUT.Text += "\r";

			SUT.ActualHeight.Should().BeGreaterThan(height * 1.2);
		}

		[TestMethod]
		public async Task When_Multiline_Wrapping_Text_Ends_In_Too_Many_Spaces()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				AcceptsReturn = true,
				TextWrapping = TextWrapping.Wrap,
				Text = "hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForLoaded(SUT);
			await WindowHelper.WaitForIdle();

			var height = SUT.ActualHeight;

			SUT.Text = "mmmmmmmmm               ";

			// Trailing space shouldn't wrap
			Assert.AreEqual(height, SUT.ActualHeight);
		}

		[TestMethod]
		public async Task When_Text_Changed_Events()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Text = "hello world"
			};

			var output = "";
			SUT.TextChanged += (o, _) => output += $"TextChanged {((TextBox)o).Text}\n";
			SUT.SelectionChanged += (o, _) => output += $"SelectionChanged {((TextBox)o).Text}\n";

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForLoaded(SUT);
			await WindowHelper.WaitForIdle();

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.A, VirtualKeyModifiers.Shift, unicodeKey: 'a'));
			await WindowHelper.WaitForIdle();

			var expected =
			"""
			TextChanged hello world
			SelectionChanged ahello world
			TextChanged ahello world
			
			""";

			Assert.AreEqual(expected.Replace("\r\n", "\n"), output);
		}

		[TestMethod]
		public async Task When_Multiline_Pointer_Tap()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 250,
				AcceptsReturn = true,
				Text = "Lorem\ripsum dolor sit\ramet consectetur\radipiscing"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(38, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			mouse.MoveTo(bounds.GetCenter() - new Point(40, 10));
			await WindowHelper.WaitForIdle();

			mouse.Press();
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(17, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Multiline_Pointer_DoubleTap()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 250,
				AcceptsReturn = true,
				Text = "Lorem\ripsum dolor sit\ramet consectetur\radipiscing"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			mouse.Release();
			await WindowHelper.WaitForIdle();

			mouse.MoveTo(bounds.GetCenter() - new Point(40, 10));
			await WindowHelper.WaitForIdle();

			mouse.Press();
			mouse.Release();
			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(12, SUT.SelectionStart);
			Assert.AreEqual(6, SUT.SelectionLength);

			mouse.MoveBy(-40, 10);
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(12, SUT.SelectionStart);
			Assert.AreEqual(15, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.Shift));

			Assert.AreEqual(12, SUT.SelectionStart);
			Assert.AreEqual(14, SUT.SelectionLength);

			mouse.Press();
			mouse.Release();
			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(22, SUT.SelectionStart);
			Assert.AreEqual(5, SUT.SelectionLength);

			mouse.MoveBy(40, -10);
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(12, SUT.SelectionStart);
			Assert.AreEqual(15, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift));

			Assert.AreEqual(13, SUT.SelectionStart);
			Assert.AreEqual(14, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Multiline_Wrapping_Pointer_DoubleTap()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				AcceptsReturn = true,
				TextWrapping = TextWrapping.Wrap,
				Text = "first line\rsecond longlonglongworddddddddddddddd"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForLoaded(SUT);
			await WindowHelper.WaitForIdle();

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			mouse.MoveTo(SUT.GetAbsoluteBounds().GetCenter());
			mouse.Press();
			mouse.Release();
			mouse.Press();
			await WindowHelper.WaitForIdle();

			// clicking at the end of a wrapping line should select starting from the wrapped part of the line (i.e. the continuing line after)
			Assert.AreEqual(18, SUT.SelectionStart);
			Assert.AreEqual(SUT.Text.Length - 18, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Multiline_Pointer_TripleTap()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 250,
				AcceptsReturn = true,
				Text = "elit aliquam\rullamcorper\rcommodoprimis\rornare himenaeos"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			mouse.Release();
			mouse.Press();
			mouse.Release();
			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(25, SUT.SelectionStart);
			Assert.AreEqual(14, SUT.SelectionLength);

			mouse.MoveBy(0, 20);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(25, SUT.SelectionStart);
			Assert.AreEqual(30, SUT.SelectionLength);

			mouse.MoveBy(0, -30);
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(13, SUT.SelectionStart);
			Assert.AreEqual(26, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Multiline_Pointer_TripleTap_With_Wrapping()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 250,
				AcceptsReturn = true,
				TextWrapping = TextWrapping.Wrap,
				Text =
					"""
					Lorem ipsum dolor sit amet consectetur adipiscing, elit aliquam u
					llamcorper commodo primis ornare himenaeos, inceptos tellus accumsan praesent laoreet. Pharetra semper ullamcorper neque mollis vestibulum luctus gravida facilisi rhoncus, rutrum massa bibendum vitae imp
					erdiet quisque fames dignissim, varius curae erat risus platea orci quis scelerisque. Auctor erat vestibulum enim sodales sapien nam litora rhoncus condimentum praesent, platea dui odio eros integer id gravida turpis semper nisi maecenas, nascetur dictumst sed arcu aenean varius dis leo habitant.
					"""
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			mouse.Release();
			mouse.Press();
			mouse.Release();
			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(270, SUT.SelectionStart);
			Assert.AreEqual(297, SUT.SelectionLength);

			mouse.MoveBy(0, -50);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(66, SUT.SelectionStart);
			Assert.AreEqual(501, SUT.SelectionLength);

			mouse.MoveBy(0, -100);
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(SUT.Text.Length, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Text_Cleared_No_Paint()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 250,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SelectAll();
			await WindowHelper.WaitForIdle();
			await Task.Delay(100);

			var canvas = SUT.FindVisualChildByType<Canvas>();
			var initial = await UITestHelper.ScreenShot(canvas);
			ImageAssert.HasColorInRectangle(initial, new Rectangle(System.Drawing.Point.Empty, initial.Size), SUT.SelectionHighlightColor.Color);

			SUT.Text = "";
			await WindowHelper.WaitForIdle();
			await Task.Delay(100);

			// No residual colors on canvas
			var cleared = await UITestHelper.ScreenShot(canvas);
			ImageAssert.DoesNotHaveColorInRectangle(cleared, new Rectangle(System.Drawing.Point.Empty, cleared.Size), SUT.SelectionHighlightColor.Color);
		}

		[TestMethod]
		public async Task When_Undo_Redo_Basic()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 40
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hello", SUT.Text);

			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);
		}

		[TestMethod]
		public async Task When_Undo_Redo_Keyboard_Basic()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 40
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hello", SUT.Text);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Z, VirtualKeyModifiers.Control, unicodeKey: 'z'));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("", SUT.Text);
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Y, VirtualKeyModifiers.Control, unicodeKey: 'z'));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);
		}

		[TestMethod]
		public async Task When_Typing_with_Backspace_Undo_Redo()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Back, VirtualKeyModifiers.None));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hello", SUT.Text);

			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);
		}

		[TestMethod]
		public async Task When_Typing_Over_Selection_Undo_Redo()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.Select(6, 5);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hello hello", SUT.Text);

			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello world", SUT.Text);
			Assert.AreEqual(6, SUT.SelectionStart);
			Assert.AreEqual(5, SUT.SelectionLength);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello hello", SUT.Text);
			Assert.AreEqual(SUT.Text.Length, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Undo_Redo_ContextMenu_Basic()
		{
			using var _ = new TextBoxFeatureConfigDisposable();
			using var __ = new DisposableAction(() => (VisualTreeHelper.GetOpenPopupsForXamlRoot(WindowHelper.XamlRoot)).ForEach((_, p) => p.IsOpen = false));

			var SUT = new TextBox
			{
				Width = 40
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hello", SUT.Text);

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			mouse.MoveTo(SUT.GetAbsoluteBounds().GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.PressRight();
			mouse.ReleaseRight();
			await WindowHelper.WaitForIdle();

			var flyoutItems = (VisualTreeHelper.GetOpenPopupsForXamlRoot(SUT.XamlRoot)[0].Child as FrameworkElement).FindChildren<MenuFlyoutItem>().ToList();
			Assert.AreEqual(3, flyoutItems.Count);

			mouse.MoveTo(flyoutItems[1].GetAbsoluteBounds().GetCenter());
			mouse.Press();
			mouse.Release();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("", SUT.Text);

			mouse.MoveTo(SUT.GetAbsoluteBounds().GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.PressRight();
			mouse.ReleaseRight();
			await WindowHelper.WaitForIdle();

			flyoutItems = (VisualTreeHelper.GetOpenPopupsForXamlRoot(SUT.XamlRoot)[0].Child as FrameworkElement).FindChildren<MenuFlyoutItem>().ToList();
			Assert.AreEqual(3, flyoutItems.Count);

			mouse.MoveTo(flyoutItems[1].GetAbsoluteBounds().GetCenter());
			mouse.Press();
			mouse.Release();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);
		}

		[TestMethod]
		public async Task When_Text_Changed_History_Cleared()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 40
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hello", SUT.Text);

			SUT.Text = "Changed";
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("Changed", SUT.Text);
			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("Changed", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("Changed", SUT.Text);
		}

		[TestMethod]
		public async Task When_ClearUndoRedoHistory()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 40
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hello", SUT.Text);

			SUT.ClearUndoRedoHistory();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hello", SUT.Text);
			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);
		}

		[TestMethod]
		public async Task When_Typing_Nothing()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 40
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None)); // break typing run
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Back, VirtualKeyModifiers.None));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Back, VirtualKeyModifiers.None));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Back, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("he", SUT.Text);
			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("he", SUT.Text);
			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("", SUT.Text);
		}

		[TestMethod]
		[DataRow(VirtualKey.Y)] // redo
		[DataRow(VirtualKey.C)] // copy
		public async Task When_Redo_Copy_DoesNot_Break_Typing(VirtualKey key)
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 40
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, key, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hello", SUT.Text);
			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("", SUT.Text);
		}

		[TestMethod]
		[DataRow(VirtualKey.X)] // cut
		[DataRow(VirtualKey.V)] // paste
		public async Task When_Cut_Paste_Breaks_Typing(VirtualKey key)
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var dataPackage = new DataPackage();
			dataPackage.SetText("");
			Clipboard.SetContent(dataPackage); // even with nothing to paste, typing still breaks

			var SUT = new TextBox
			{
				Width = 40
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, key, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hello", SUT.Text);
			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("he", SUT.Text);
			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("he", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);
		}

		[TestMethod]
		public async Task When_CanRedo_CanUndo()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 40
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			Assert.IsFalse(SUT.CanRedo);
			Assert.IsFalse(SUT.CanUndo);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None)); // break typing run
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hellohello", SUT.Text);

			Assert.IsFalse(SUT.CanRedo);
			Assert.IsTrue(SUT.CanUndo);

			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);

			Assert.IsTrue(SUT.CanRedo);
			Assert.IsTrue(SUT.CanUndo);

			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("", SUT.Text);

			Assert.IsTrue(SUT.CanRedo);
			Assert.IsFalse(SUT.CanUndo);

			SUT.Redo();

			Assert.IsTrue(SUT.CanRedo);
			Assert.IsTrue(SUT.CanUndo);
		}

		[TestMethod]
		public async Task When_Pointer_Clicked_Typing_Ends()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hello", SUT.Text);

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			mouse.Release();
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hellohello", SUT.Text);

			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);
			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hellohello", SUT.Text);
		}

		[TestMethod]
		public async Task When_Pointer_Pressed_Undo()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hello", SUT.Text);

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			// no release
			await WindowHelper.WaitForIdle();

			// Shouldn't be able to undo
			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);

			mouse.Release();
			await WindowHelper.WaitForIdle();

			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);
		}

		[TestMethod]
		public async Task When_Pointer_Pressed_Redo()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hello", SUT.Text);

			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("", SUT.Text);

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			// no release
			await WindowHelper.WaitForIdle();

			// Shouldn't be able to redo
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("", SUT.Text);

			mouse.Release();
			await WindowHelper.WaitForIdle();

			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);
			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("", SUT.Text);
		}

		[TestMethod]
		public async Task When_Unfocused_Typing_Ends()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hello", SUT.Text);

			VisualTree.GetFocusManagerForElement(SUT)!.TryMoveFocusInstance(FocusNavigationDirection.Next);
			await WindowHelper.WaitForIdle();

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hellohello", SUT.Text);

			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);
			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hellohello", SUT.Text);
		}

		[TestMethod]
		public async Task When_Caret_Moves_Typing_Ends()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.H, VirtualKeyModifiers.None, unicodeKey: 'h'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.E, VirtualKeyModifiers.None, unicodeKey: 'e'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.O, VirtualKeyModifiers.None, unicodeKey: 'o'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hello", SUT.Text);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.None));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.L, VirtualKeyModifiers.None, unicodeKey: 'l'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("hellllo", SUT.Text);

			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);
			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hellllo", SUT.Text);
		}

		[TestMethod]
		public async Task When_Repeated_Delete_Undo_Redo()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "hello"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Delete, VirtualKeyModifiers.None));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Delete, VirtualKeyModifiers.None));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Delete, VirtualKeyModifiers.None));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Delete, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();

			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("lo", SUT.Text);
			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("llo", SUT.Text);
			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("ello", SUT.Text);
			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("ello", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("llo", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("lo", SUT.Text);
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("o", SUT.Text);
		}

		[TestMethod]
		public async Task When_Ctrl_Delete_Undo_Redo()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Delete, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();

			SUT.Undo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("hello world", SUT.Text);
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(6, SUT.SelectionLength); // When Ctrl-delete is undone, we select what was (un)deleted!!!
			SUT.Redo();
			await WindowHelper.WaitForIdle();
			Assert.AreEqual("world", SUT.Text);
		}

		[TestMethod]
		public async Task When_Paste_Does_Not_Change_Text()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "t"
			};

			WindowHelper.WindowContent = SUT;

			await UITestHelper.Load(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.End, VirtualKeyModifiers.None));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.Shift));
			await WindowHelper.WaitForIdle();

			var dp = new DataPackage();
			var text = "t";
			dp.SetText(text);
			Clipboard.SetContent(dp);

			SUT.PasteFromClipboard();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_FeatureConfiguration_Changes()
		{
			var useOverlay = FeatureConfiguration.TextBox.UseOverlayOnSkia;
			using var _ = Disposable.Create(() => FeatureConfiguration.TextBox.UseOverlayOnSkia = useOverlay);

			FeatureConfiguration.TextBox.UseOverlayOnSkia = true;

			var SUT = new TextBox
			{
				Width = 150,
				Text = "hello world"
			};

			await UITestHelper.Load(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.TextBoxView.DisplayBlock.Opacity);

			FeatureConfiguration.TextBox.UseOverlayOnSkia = false;

			await UITestHelper.Load(new Button()); // a random control to unload SUT

			Assert.AreEqual(1, SUT.TextBoxView.DisplayBlock.Opacity);
		}

		[TestMethod]
		public async Task When_Caret_Color_DarkMode()
		{
			var useOverlay = FeatureConfiguration.TextBox.UseOverlayOnSkia;
			using var _1 = Disposable.Create(() => FeatureConfiguration.TextBox.UseOverlayOnSkia = useOverlay);

			// The TextBox is purposefully empty. We want the only content pixels to come from the caret.
			var SUT = new TextBox
			{
				Width = 150
			};

			await UITestHelper.Load(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var random = new Random();
			var i = 0;
			for (; i < 20; i++)
			{
				await Task.Delay(random.Next(75, 126));
				var screenshot = await UITestHelper.ScreenShot(SUT);
				if (HasColorInRectangle(screenshot, new Rectangle(0, 0, screenshot.Width / 2, screenshot.Height), Colors.Black) &&
					!HasColorInRectangle(screenshot, new Rectangle(screenshot.Width / 2, 0, screenshot.Width / 2, screenshot.Height), Colors.Black))
				{
					break;
				}
			}

			Assert.IsTrue(i < 20);

			using var _2 = ThemeHelper.UseDarkTheme();
			await WindowHelper.WaitForIdle();

			for (; i < 20; i++)
			{
				await Task.Delay(random.Next(75, 126));
				var screenshot = await UITestHelper.ScreenShot(SUT);
				if (HasColorInRectangle(screenshot, new Rectangle(0, 0, screenshot.Width / 2, screenshot.Height), Colors.White) &&
					!HasColorInRectangle(screenshot, new Rectangle(screenshot.Width / 2, 0, screenshot.Width / 2, screenshot.Height), Colors.White))
				{
					break;
				}
			}

			Assert.IsTrue(i < 20);
		}

		private static bool HasColorInRectangle(RawBitmap screenshot, Rectangle rect, Color expectedColor)
		{
			for (var x = rect.Left; x < rect.Right; x++)
			{
				for (var y = rect.Top; y < rect.Bottom; y++)
				{
					var pixel = screenshot.GetPixel(x, y);
					if (expectedColor == pixel)
					{
						return true;
					}
				}
			}

			return false;
		}

		private class TextBoxFeatureConfigDisposable : IDisposable
		{
			private bool _useOverlay;
			private bool _hideCaret;

			public TextBoxFeatureConfigDisposable()
			{
				_useOverlay = FeatureConfiguration.TextBox.UseOverlayOnSkia;
				_hideCaret = FeatureConfiguration.TextBox.HideCaret;

				FeatureConfiguration.TextBox.UseOverlayOnSkia = false;
				FeatureConfiguration.TextBox.HideCaret = true;
			}

			public void Dispose()
			{
				FeatureConfiguration.TextBox.UseOverlayOnSkia = _useOverlay;
				FeatureConfiguration.TextBox.HideCaret = _hideCaret;
			}
		}
	}
}
