using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp9.Models
{
    public class TodoItem : INotifyPropertyChanged  // Add interface
    {
        // Keep all your existing properties exactly as they are
        public int item_id { get; set; }
        public string item_name { get; set; }
        public string item_description { get; set; }
        public string status { get; set; }
        public int user_id { get; set; }
        public string timemodified { get; set; }

        // Keep your computed property
        public string Title => item_name;

        // Modify just the IsCompleted property to support notifications
        private bool _isCompleted;
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged();
                }
            }
        }

        // Add this new code at the bottom (won't affect existing functionality)
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
