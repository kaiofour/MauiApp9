//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Text;
//using System.Text.Json;


//namespace MauiApp9
//{
//    public static class TodoService
//    {
//        private static readonly ObservableCollection<TodoItem> _pendingTasks = new();
//        private static readonly ObservableCollection<TodoItem> _completedTasks = new();

//        public static ObservableCollection<TodoItem> PendingTasks => _pendingTasks;
//        public static ObservableCollection<TodoItem> CompletedTasks => _completedTasks;
//        public static TodoItem? SelectedItem { get; set; }

//        public static void MoveToCompleted(TodoItem item)
//        {
//            if (_pendingTasks.Contains(item))
//            {
//                _pendingTasks.Remove(item);
//                item.IsCompleted = true;
//                _completedTasks.Add(item);
//            }
//        }
//    }
//}


