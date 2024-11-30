using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json; // Обеспечиваем доступ к JsonSerializer
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OOP_LR7
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> AvailableItems { get; set; }
        private ObservableCollection<string> SelectedItems { get; set; }

        private string draggedItem;

        public MainWindow()
        {
            InitializeComponent();

            // Инициализация исходных данных
            AvailableItems = new ObservableCollection<string>
            {
                "Яблоки", "Бананы", "Вишня", "Малина", "Клубника",
                "Арбузы", "Груши", "Смородина", "Киви", "Лимоны"
            };

            SelectedItems = new ObservableCollection<string>();

            AvailableItemsList.ItemsSource = AvailableItems;
            SelectedItemsList.ItemsSource = SelectedItems;

            UpdateCountText();
        }

        private void MoveToSelected_Click(object sender, RoutedEventArgs e)
        {
            var itemsToMove = new List<string>();
            foreach (var item in AvailableItemsList.SelectedItems)
            {
                itemsToMove.Add(item.ToString());
            }

            foreach (var item in itemsToMove)
            {
                AvailableItems.Remove(item);
                SelectedItems.Add(item);
            }

            UpdateCountText();
        }

        private void MoveToAvailable_Click(object sender, RoutedEventArgs e)
        {
            var itemsToMove = new List<string>();
            foreach (var item in SelectedItemsList.SelectedItems)
            {
                itemsToMove.Add(item.ToString());
            }

            foreach (var item in itemsToMove)
            {
                SelectedItems.Remove(item);
                AvailableItems.Add(item);
            }

            UpdateCountText();
        }

        private void RemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            var itemsToRemove = new List<string>();

            // Удаление из AvailableItemsList
            foreach (var item in AvailableItemsList.SelectedItems)
            {
                itemsToRemove.Add(item.ToString());
            }

            foreach (var item in itemsToRemove)
            {
                AvailableItems.Remove(item);
            }

            itemsToRemove.Clear();

            // Удаление 
            foreach (var item in SelectedItemsList.SelectedItems)
            {
                itemsToRemove.Add(item.ToString());
            }

            foreach (var item in itemsToRemove)
            {
                SelectedItems.Remove(item);
            }

            UpdateCountText();
        }


        // Перемещение с помощью мыши
        private void AvailableItemsList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var listBox = (ListBox)sender;
            var selectedItem = listBox.SelectedItem as string;

            if (selectedItem != null)
            {
                draggedItem = selectedItem;
                DragDrop.DoDragDrop(listBox, selectedItem, DragDropEffects.Move);
            }
        }

        // Перемещение обратно в Available Items с помощью мыши
        private void SelectedItemsList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var listBox = (ListBox)sender;
            var selectedItem = listBox.SelectedItem as string;

            if (selectedItem != null)
            {
                draggedItem = selectedItem;
                DragDrop.DoDragDrop(listBox, selectedItem, DragDropEffects.Move);
            }
        }

        
        private void AvailableItemsList_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        private void SelectedItemsList_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        
        private void AvailableItemsList_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void SelectedItemsList_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

       
        private void AvailableItemsList_Drop(object sender, DragEventArgs e)
        {
            if (draggedItem != null)
            {
                AvailableItems.Add(draggedItem);
                SelectedItems.Remove(draggedItem);
                UpdateCountText();
            }
        }

        private void SelectedItemsList_Drop(object sender, DragEventArgs e)
        {
            if (draggedItem != null)
            {
                SelectedItems.Add(draggedItem);
                AvailableItems.Remove(draggedItem);
                UpdateCountText();
            }
        }

        // Обновление текста с количеством элементов
        private void UpdateCountText()
        {
            AvailableCountText.Text = $"Доступные товары: {AvailableItems.Count}";
            SelectedCountText.Text = $"Выбранные товары: {SelectedItems.Count}";
        }

        // Сохранение состояния в файл
        private void SaveState_Click(object sender, RoutedEventArgs e)
        {
            var state = new
            {
                AvailableItems = AvailableItems.ToList(),
                SelectedItems = SelectedItems.ToList()
            };

            var json = JsonSerializer.Serialize(state);  // Сериализация состояния в JSON
            File.WriteAllText("state.json", json);
            MessageBox.Show("Статус приложения успешно сохранен");
        }

        // Загрузка состояния из файла
        private void LoadState_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("state.json"))
            {
                var json = File.ReadAllText("state.json");
                var state = JsonSerializer.Deserialize<State>(json);  // Десериализация в тип State

                AvailableItems.Clear();
                SelectedItems.Clear();

                foreach (var item in state.AvailableItems)
                    AvailableItems.Add(item);

                foreach (var item in state.SelectedItems)
                    SelectedItems.Add(item);

                UpdateCountText();
                MessageBox.Show("Статус приложения успешно загружен");
            }
            else
            {
                MessageBox.Show("Сохраненное состояние не найдено");
            }
        }

        // Сброс состояния в начальное
        private void ResetState_Click(object sender, RoutedEventArgs e)
        {
            AvailableItems.Clear();
            SelectedItems.Clear();

            // Исходные данные
            AvailableItems.Add("Яблоки");
            AvailableItems.Add("Вишня");
            AvailableItems.Add("Малина");
            AvailableItems.Add("Клубника");
            AvailableItems.Add("Арбузы");
            AvailableItems.Add("Груши");
            AvailableItems.Add("Смородина");
            AvailableItems.Add("Киви");
            AvailableItems.Add("Лимоны");
            AvailableItems.Add("Бананы");


            UpdateCountText();
        }

        // Поиск в Available Items
        private void AvailableSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filter = AvailableSearchBox.Text.ToLower();
            AvailableItemsList.ItemsSource = string.IsNullOrEmpty(filter)
                ? AvailableItems
                : new ObservableCollection<string>(AvailableItems.Where(i => i.ToLower().Contains(filter)));
        }

        // Поиск в Selected Items
        private void SelectedSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filter = SelectedSearchBox.Text.ToLower();
            SelectedItemsList.ItemsSource = string.IsNullOrEmpty(filter)
                ? SelectedItems
                : new ObservableCollection<string>(SelectedItems.Where(i => i.ToLower().Contains(filter)));
        }

        // Класс состояния для десериализации
        public class State
        {
            public List<string> AvailableItems { get; set; }
            public List<string> SelectedItems { get; set; }
        }
    }
}
