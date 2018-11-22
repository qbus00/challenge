using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Challenge.ViewModels
{
    public class ReposViewModel : MvxViewModel
    {
        public IMvxCommand ResetTextCommand => new MvxCommand(ResetText);
        private void ResetText()
        {
            Text = "Hello MvvmCross";
        }

        private string _text = "Hello MvvmCross";
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }
    }
}
