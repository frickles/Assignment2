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

            // A method for the button-click event that i made
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

            var tasks = urls.Select(LoadDocumentAsync).ToList();
            var load = await Task.WhenAll(tasks);
            foreach (var url in urls)
            {
                var document = XDocument.Load(url);
                foreach (var item in document.Descendants("item"))
                {
                    string title = item.Descendants("title").First().Value;
                    DateTime date = DateTime.Parse(item.Descendants("pubDate").First().Value);
                    string websiteTitle = document.Descendants("title").First().Value;


                    // All feeds // Working
                    if (selectFeedComboBox.SelectedIndex == 0)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            var articlePlaceholder = new StackPanel
                            {
                                Orientation = Orientation.Vertical,
                                Margin = spacing
                            };
                            articlePanel.Children.Add(articlePlaceholder);

                            var articleTitle = new TextBlock
                            {
                                Text = date + " - " + title + " #" + (i + 1),
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
                    }
            else
            {
                articlePanel.Children.Clear();
                //foreach (var url in urls)
                //{
                //    await Task.WhenAll(tasks);
                //    var document = XDocument.Load(url);
                //    //string[] allTitles = document.Descendants("item").Select(t => t.Value).ToArray();

                //    foreach (var item in document.Descendants("item"))
                //    {
                //        string title = item.Descendants("title").First().Value;
                //        DateTime date = DateTime.Parse(item.Descendants("pubDate").First().Value);
                //        string websiteTitle = document.Descendants("title").First().Value;

                        for (int i = 0; i < 5; i++)
                        {
                            var articlePlaceholder = new StackPanel
                            {
                                Orientation = Orientation.Vertical,
                                Margin = spacing
                            };
                            articlePanel.Children.Add(articlePlaceholder);

                            var articleTitle = new TextBlock
                            {
                                //Text = "2021-01-02 12:34 - Placeholder for an actual article title #" + (i + 1),
                                Text = date + " - " + title + " #" + (i + 1),
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

                    }
                }
            }
            loadArticlesButton.IsEnabled = true;
        }

        private async void AddFeed(object sender, RoutedEventArgs e)
        {
            addFeedButton.IsEnabled = false;
            string text = addFeedTextBox.Text;

            await LoadDocumentAsync(text);

            addFeedButton.IsEnabled = true;
            ////https://www.comingsoon.net/feed
            ////https://www.cinemablend.com/rss/topic/news/movies

            if (addFeedTextBox.Text == "")
            {
                MessageBox.Show("Please enter a valid URL");
            }
            else
            {
                addFeedTextBox.Clear();

                var document = XDocument.Load(text);
                string firstTitle = document.Descendants("title").First().Value;
                selectFeedComboBox.Items.Add(firstTitle);
                selectFeedComboBox.SelectedItem = firstTitle;
                urls.Add(text);
            }
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
