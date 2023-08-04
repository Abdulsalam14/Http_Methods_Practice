using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Http_Methods_Client
{
    /// <summary>
    /// Interaction logic for FillUser.xaml
    /// </summary>
    /// 

    public partial class FillUser : Window
    {
        private TaskCompletionSource taskCompletionSource;
        MainWindow mainWindow;
        public FillUser(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            taskCompletionSource = new TaskCompletionSource();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtid.Text != "" && txtname.Text != "" && txtsurname.Text != "" && txtage.Text != "")
            {
                var b = int.TryParse(txtid.Text, out int id);
                var b2 = ushort.TryParse(txtage.Text, out ushort year);
                if (b && b2)
                {
                    mainWindow.user = new User(id, txtname.Text, txtsurname.Text, year);
                    taskCompletionSource.SetResult();
                    App.Current.Windows[1].Close();
                }
                else MessageBox.Show("Wrong Fill Id or Year");
            }
            else
            {
                MessageBox.Show("Fill");
            }
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            if (mainWindow.IsVisible == false) mainWindow.Show();
        }

        public async Task GetResultAsync()
        {
            await taskCompletionSource.Task;
        }
    }
}
