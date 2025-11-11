using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JIDS.ViewModels;

namespace JIDS.Views
{
    public partial class ConfigurationListView : UserControl
    {
        public ConfigurationListView()
        {
            InitializeComponent();

            // DataContext is expected to be set by DI / shell (MainWindow) before navigation.
            // If it's not set, leave it null so designers won't execute runtime code.
            // You can optionally resolve a ViewModel from Application.Current.Resources here
            // if you've registered one at application startup.
        }

        // Row double-click will execute the EditCommand on the VM with the clicked item as parameter
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                var vm = DataContext as ConfigurationListViewModel;
                var item = row.Item;
                if (vm?.EditCommand != null && vm.EditCommand.CanExecute(item))
                {
                    vm.EditCommand.Execute(item);
                }
            }
        }
    }
}