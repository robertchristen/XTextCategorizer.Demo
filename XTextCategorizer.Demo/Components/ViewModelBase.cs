using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XTextCategorizer.Demo.Components
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<TValue>(ref TValue member, TValue value, [CallerMemberName] string propertyName = null)
        {
            member = value;
            OnPropertyChanged(propertyName);
        }
    }
}
