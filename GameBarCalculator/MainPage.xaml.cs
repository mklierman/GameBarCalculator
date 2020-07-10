using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace GameBarCalculator
{
    using Dangl.Calculator;
    using Microsoft.Gaming.XboxGameBar;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Windows.UI.Popups;
    using Windows.UI.Xaml.Automation.Peers;
    using Windows.UI.Xaml.Automation.Provider;
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private XboxGameBarWidget widget = null;

        public MainPage()
        {
            this.InitializeComponent();
            numberTextBox.Background = numberTextBox.Background;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            widget = e.Parameter as XboxGameBarWidget;

            //Hook up events for when the ui is updated.
            if (widget != null)
                {
                    widget.SettingsClicked += Widget_SettingsClicked;
                    widget.PinnedChanged += Widget_PinnedChanged;
                }
        }

        private async void Widget_SettingsClicked(XboxGameBarWidget sender, object args)
        {
            await widget.ActivateSettingsAsync();
        }

        private async void Widget_PinnedChanged(XboxGameBarWidget sender, object args)
        {

        }

        private void zeroButton_Click(object sender, RoutedEventArgs e)
        {
            updateDisplayText("0");
        }

        private void oneButton_Click(object sender, RoutedEventArgs e)
        {
            updateDisplayText("1");
        }

        private void twoButton_Click(object sender, RoutedEventArgs e)
        {
            updateDisplayText("2");
        }

        private void threeButton_Click(object sender, RoutedEventArgs e)
        {
            updateDisplayText("3");
        }

        private void fourButton_Click(object sender, RoutedEventArgs e)
        {
            updateDisplayText("4");
        }

        private void fiveButton_Click(object sender, RoutedEventArgs e)
        {
            updateDisplayText("5");
        }

        private void sixButton_Click(object sender, RoutedEventArgs e)
        {
            updateDisplayText("6");
        }

        private void sevenButton_Click(object sender, RoutedEventArgs e)
        {
            updateDisplayText("7");
        }

        private void eightButton_Click(object sender, RoutedEventArgs e)
        {
            updateDisplayText("8");
        }

        private void nineButton_Click(object sender, RoutedEventArgs e)
        {
            updateDisplayText("9");
        }

        private void updateDisplayText(string number)
        {
            numberTextBox.Text += number;
            equalsButton.Focus(FocusState.Pointer);
        }

        private void decimalButton_Click(object sender, RoutedEventArgs e)
        {
            //Make sure we only add after numbers and don't add multiple
            Regex regex = new Regex(@"(\d+\.?\d*)$");
            string regexResult = regex.Match(numberTextBox.Text).Value;
            if (regexResult.Length > 0 && !regexResult.Contains("."))
            {
                updateDisplayText(".");
            }
            equalsButton.Focus(FocusState.Pointer);
        }

        private void ceButton_Click(object sender, RoutedEventArgs e)
        {
            //Remove last grouping
            int indexOfLastGroup = getLastGroupIndex();
            if (indexOfLastGroup > 0)
            {
                numberTextBox.Text = numberTextBox.Text.Remove(indexOfLastGroup);
            }
            else
            {
                numberTextBox.Text = "";
                numberTextBox.PlaceholderText = "0";
            }
            equalsButton.Focus(FocusState.Pointer);
        }

        private int getLastGroupIndex()
        {
            Regex regex = new Regex(@"(\d+\.?\d*)(?:[+\-*/]*)$");
            string regexResult = regex.Match(numberTextBox.Text).Value;
            return numberTextBox.Text.LastIndexOf(regexResult);

        }

        private void cButton_Click(object sender, RoutedEventArgs e)
        {
            numberTextBox.Text = "";
            numberTextBox.PlaceholderText = "0";
            equalsButton.Focus(FocusState.Pointer);
        }

        private void plusButton_Click(object sender, RoutedEventArgs e)
        {
            addOperator("+");
        }

        private void minusButton_Click(object sender, RoutedEventArgs e)
        {
            addOperator("-");
        }

        private void multiplyButton_Click(object sender, RoutedEventArgs e)
        {
            addOperator("*");
        }

        private void divideButton_Click(object sender, RoutedEventArgs e)
        {
            addOperator("/");
        }

        private void addOperator(string mathOp)
        {
            removeTrailingDecimal();
            if (numberTextBox.Text == "" && numberTextBox.PlaceholderText.Length > 0)
            {
                //A result is being displayed, add it to the textbox and then the operator
                numberTextBox.Text = numberTextBox.PlaceholderText;
                updateDisplayText(mathOp);
            }
            else if (getPreviousCharacter() == ")" || isLastCharacterDigit())
            {
                updateDisplayText(mathOp);
            }
            else
            {
                //Replace existing operator
                numberTextBox.Text = numberTextBox.Text.Remove(numberTextBox.Text.Length - 1);
                updateDisplayText(mathOp);
            }
            equalsButton.Focus(FocusState.Pointer);
        }

        private string getPreviousCharacter()
        {
            if (numberTextBox.Text.Length > 0)
            {
                int textLength = numberTextBox.Text.Length;
                return numberTextBox.Text.Substring(textLength - 1);
            }
            else
                return "";
        }

        private bool isLastCharacterDigit()
        {
            if (numberTextBox.Text.Length > 0)
            {
                return Char.IsDigit(getPreviousCharacter(), 0);
            }
            return false;
        }

        private void removeTrailingDecimal()
        {
            if (getPreviousCharacter() == ".")
            {
                numberTextBox.Text = numberTextBox.Text.Remove(numberTextBox.Text.Length - 1);
            }
        }

        private void equalsButton_Click(object sender, RoutedEventArgs e)
        {
            if (numberTextBox.Text.Length > 0)
            {
                removeTrailingDecimal();
                if (getPreviousCharacter() != ")" && !isLastCharacterDigit())
                    numberTextBox.Text = numberTextBox.Text.Remove(numberTextBox.Text.Length - 1);

                if (getPreviousCharacter() == ")" || isLastCharacterDigit())
                {
                    var result = Calculator.Calculate(numberTextBox.Text);
                    numberTextBox.PlaceholderText = result.Result.ToString();
                    numberTextBox.Text = "";
                }
            }
            equalsButton.Focus(FocusState.Pointer);
        }

        private void positiveNegativeButton_Click(object sender, RoutedEventArgs e)
        {
            //Add or remove minus sign from front of last group
            int indexOfLastGroup = getLastGroupIndex();
            if (numberTextBox.Text.Length > 0)
            {
                if (indexOfLastGroup > 1 && numberTextBox.Text.Substring(indexOfLastGroup - 1) == "-" || indexOfLastGroup > 2 && numberTextBox.Text.Substring(indexOfLastGroup - 2, 2) == "--") //Already has a minus in front "-1+2" or "1--2"
                {
                    numberTextBox.Text = numberTextBox.Text.Remove(indexOfLastGroup, 1);
                }
                else //Add minus to front of group
                {
                    numberTextBox.Text = numberTextBox.Text.Insert(indexOfLastGroup, "-");
                }
            }
            equalsButton.Focus(FocusState.Pointer);
        }

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
        {

        }

        private async void clickButton(Button button)
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(button);
            IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProv.Invoke();

            VisualStateManager.GoToState(button, "Pressed", true);
            await Task.Delay(100); // give the eye some time to see the press
            VisualStateManager.GoToState(button, "Normal", true);
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.NumberPad0:
                    clickButton(zeroButton);
                    break;
                case Windows.System.VirtualKey.NumberPad1:
                    clickButton(oneButton);
                    break;
                case Windows.System.VirtualKey.NumberPad2:
                    clickButton(twoButton);
                    break;
                case Windows.System.VirtualKey.NumberPad3:
                    clickButton(threeButton);
                    break;
                case Windows.System.VirtualKey.NumberPad4:
                    clickButton(fourButton);
                    break;
                case Windows.System.VirtualKey.NumberPad5:
                    clickButton(fiveButton);
                    break;
                case Windows.System.VirtualKey.NumberPad6:
                    clickButton(sixButton);
                    break;
                case Windows.System.VirtualKey.NumberPad7:
                    clickButton(sevenButton);
                    break;
                case Windows.System.VirtualKey.NumberPad8:
                    clickButton(eightButton);
                    break;
                case Windows.System.VirtualKey.NumberPad9:
                    clickButton(nineButton);
                    break;
                case Windows.System.VirtualKey.Add:
                    clickButton(plusButton);
                    break;
                case Windows.System.VirtualKey.Subtract:
                    clickButton(minusButton);
                    break;
                case Windows.System.VirtualKey.Multiply:
                    clickButton(multiplyButton);
                    break;
                case Windows.System.VirtualKey.Divide:
                    clickButton(divideButton);
                    break;
                case Windows.System.VirtualKey.Enter:
                    clickButton(equalsButton);
                    break;
                case Windows.System.VirtualKey.Execute:
                    clickButton(equalsButton);
                    break;
                case Windows.System.VirtualKey.Escape:
                    clickButton(cButton);
                    break;
                case Windows.System.VirtualKey.Delete:
                    clickButton(ceButton);
                    break;
                case Windows.System.VirtualKey.Back:
                    clickButton(undoButton);
                    break;

            }
        }

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            if (numberTextBox.Text.Length > 0)
            {
                numberTextBox.Text = numberTextBox.Text.Remove(numberTextBox.Text.Length - 1);
            }
            equalsButton.Focus(FocusState.Pointer);
        }
    }
}
