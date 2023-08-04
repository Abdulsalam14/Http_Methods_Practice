using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Http_Methods_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client;
        HttpRequestMessage? request;
        public User? user;
        TaskCompletionSource<int>? selectedid;
        public MainWindow()
        {
            InitializeComponent();
            client = new HttpClient();
            combo.Items.Add("Get");
            combo.Items.Add("GetById");
            combo.Items.Add("Post");
            combo.Items.Add("Put");
            combo.Items.Add("Delete");
        }


        private async void combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            request = new HttpRequestMessage()
            {
                RequestUri = new Uri(@"http://localhost:27002/")
            };
            txtid.Text = "";
            if (combo.SelectedIndex == 0)
            {
                fillid.Visibility = Visibility.Hidden;
                request.Method = HttpMethod.Get;
                var getresponse = await client.SendAsync(request);
                var content = await getresponse.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<User>>(content);
                list.Visibility = Visibility.Visible;
                list.ItemsSource = users;
            }
            else if (combo.SelectedIndex == 1)
            {
                selectedid = new();
                list.Visibility = Visibility.Hidden;
                fillid.Visibility = Visibility.Visible;
                var getid = await SelectedIdResult();
                request.Method = HttpMethod.Get;
                request.Content = new StringContent(getid.ToString());
                var byidresponse = await client.SendAsync(request);
                var answer = await byidresponse.Content.ReadAsStringAsync();
                if (byidresponse.IsSuccessStatusCode)
                {
                    var getuser = JsonSerializer.Deserialize<User>(answer);
                    var listuser = new List<User>() { getuser! };
                    list.ItemsSource = listuser;
                    list.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Incorrect Id");
                    combo.SelectedItem = null;
                }
                fillid.Visibility = Visibility.Hidden;
            }
            else if (combo.SelectedIndex == 2)
            {
                fillid.Visibility = Visibility.Hidden;
                list.Visibility = Visibility.Collapsed;
                combo.SelectedItem = null;
                request.Method = HttpMethod.Post;
                Hide();
                FillUser fill = new FillUser(this);
                fill.Show();
                await fill.GetResultAsync();
                Show();
                request.Content = new StringContent(JsonSerializer.Serialize(user));
                var postresponse = await client.SendAsync(request);
                MessageBox.Show($"{await postresponse.Content.ReadAsStringAsync()}");

            }
            else if (combo.SelectedIndex == 3)
            {
                fillid.Visibility = Visibility.Hidden;
                list.Visibility = Visibility.Collapsed;
                combo.SelectedItem = null;
                request.Method = HttpMethod.Put;
                Hide();
                FillUser change = new FillUser(this);
                change.Show();
                await change.GetResultAsync();
                Show();
                request.Content = new StringContent(JsonSerializer.Serialize(user));
                var putresponse = await client.SendAsync(request);
                MessageBox.Show($"{await putresponse.Content.ReadAsStringAsync()}");

            }
            else if (combo.SelectedIndex == 4)
            {
                selectedid = new();
                list.Visibility = Visibility.Hidden;
                fillid.Visibility = Visibility.Visible;
                var id = await SelectedIdResult();
                request.Method = HttpMethod.Delete;
                request.Content = new StringContent(id.ToString());
                var deleteresponse = await client.SendAsync(request);
                MessageBox.Show(await deleteresponse.Content.ReadAsStringAsync());
                fillid.Visibility = Visibility.Hidden;
                combo.SelectedItem = null;
            }
        }


        private void Okbtn_Click(object sender, RoutedEventArgs e)
        {
            var b = int.TryParse(txtid.Text, out int changedid);
            if (!b)
            {
                MessageBox.Show("Only ID number");
                return;
            }
            selectedid?.SetResult(changedid);
            txtid.Text = string.Empty;
            selectedid = new();

        }

        public async Task<int> SelectedIdResult()
        {
            return await selectedid!.Task;
        }
    }
}
