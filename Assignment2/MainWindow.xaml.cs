using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Assignment2
{
    public partial class MainWindow : Window
    {
        private Thickness spacing = new Thickness(5);
        private HttpClient http = new HttpClient();
        private TextBox addFeedTextBox;
        private Button addFeedButton;
        private ComboBox selectFeedComboBox;
        private Button loadArticlesButton;
        private StackPanel articlePanel;
        private List<string> urls = new List<string>();

        private Dictionary<string, string> UrlNames = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            #region Design
            // Window options
            Title = "Feed Reader";
            Width = 800;
            Height = 400;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            var root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            var grid = new Grid();
            root.Content = grid;
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var addFeedLabel = new Label
            {
                Content = "Feed URL:",
                Margin = spacing
            };
            grid.Children.Add(addFeedLabel);

            addFeedTextBox = new TextBox
            {
                Margin = spacing,
                Padding = spacing
            };
            grid.Children.Add(addFeedTextBox);
            Grid.SetColumn(addFeedTextBox, 1);

            addFeedButton = new Button
            {
                Content = "Add Feed",
                Margin = spacing,
                Padding = spacing
            };
            grid.Children.Add(addFeedButton);
            Grid.SetColumn(addFeedButton, 2);

            addFeedButton.Click += AddFeed;

            var selectFeedLabel = new Label
            {
                Content = "Select Feed:",
                Margin = spacing
            };
            grid.Children.Add(selectFeedLabel);
            Grid.SetRow(selectFeedLabel, 1);

            selectFeedComboBox = new ComboBox
            {
                Margin = spacing,
                Padding = spacing,
                IsEditable = false
            };
            grid.Children.Add(selectFeedComboBox);
            Grid.SetRow(selectFeedComboBox, 1);
            Grid.SetColumn(selectFeedComboBox, 1);

            // Added this for default option with all feeds
            selectFeedComboBox.SelectedIndex = 0;
            selectFeedComboBox.Items.Add("All Feeds");

            loadArticlesButton = new Button
            {
                Content = "Load Articles",
                Margin = spacing,
                Padding = spacing
            };
            grid.Children.Add(loadArticlesButton);
            Grid.SetRow(loadArticlesButton, 1);
            Grid.SetColumn(loadArticlesButton, 2);

            loadArticlesButton.Click += LoadArticles;

            articlePanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = spacing
            };
            grid.Children.Add(articlePanel);
            Grid.SetRow(articlePanel, 2);
            Grid.SetColumnSpan(articlePanel, 3);
            #endregion
        }

        private async void LoadArticles(object sender, RoutedEventArgs e)
        {
            articlePanel.Children.Clear();
            loadArticlesButton.IsEnabled = false;

            List<Task<XDocument>> tasks = new List<Task<XDocument>>();
            if (selectFeedComboBox.SelectedIndex == 0)
            {
                tasks = urls.Select(LoadDocumentAsync).ToList();
            }
            else
            {
                tasks.Add(LoadDocumentAsync(UrlNames[(string)selectFeedComboBox.SelectedItem]));
            }

            var documents = await Task.WhenAll(tasks);

            var items = new List<XElement>();
            foreach (var document in documents)
            {
                items.AddRange(document.Descendants("item").Take(5));
            }

            items = items.OrderByDescending(i => i.Descendants("pubDate").First().Value).ToList();

            foreach (var item in items)
            {
                string title = item.Descendants("title").First().Value;
                DateTime date = DateTime.Parse(item.Descendants("pubDate").First().Value);
                string websiteTitle = item.Parent.Descendants("title").First().Value;

                var articlePlaceholder = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = spacing
                };
                articlePanel.Children.Add(articlePlaceholder);

                var articleTitle = new TextBlock
                {
                    Text = date + " - " + title,
                    FontWeight = FontWeights.Bold,
                    TextTrimming = TextTrimming.CharacterEllipsis
                };
                articlePlaceholder.Children.Add(articleTitle);

                var articleWebsite = new TextBlock
                {
                    Text = websiteTitle
                };
                articlePlaceholder.Children.Add(articleWebsite);
            }
            loadArticlesButton.IsEnabled = true;
        }

        private async void AddFeed(object sender, RoutedEventArgs e)
        {
            addFeedButton.IsEnabled = false;
            string url = addFeedTextBox.Text;

            var document = await LoadDocumentAsync(url);

            addFeedButton.IsEnabled = true;

                addFeedTextBox.Clear();

                string firstTitle = document.Descendants("title").First().Value;
                selectFeedComboBox.Items.Add(firstTitle);
                selectFeedComboBox.SelectedItem = firstTitle;
                UrlNames.Add(firstTitle, url);
                urls.Add(url);
        }
        private async Task<XDocument> LoadDocumentAsync(string url)
        {
            await Task.Delay(1000);
            var response = await http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            var feed = XDocument.Load(stream);
            return feed;
        }
    }
}
