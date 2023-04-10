using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Net;

namespace hww1
{
    interface IAdapter
    {
        void Add(string filename, string name, string surname, string phone, string address);
        void Update(string filename, string name, string surname, string phone, string address);
        string Get(string filename);
    }

class JsonAdapter : IAdapter
    {
        public JsonAdapter() { }

        public void Add(string filename, string name, string surname, string phone, string address)
        {
            var data = new { Name = name, Surname = surname, Phone = phone, Address = address };
            string json = JsonSerializer.Serialize(data);
            File.WriteAllText(filename, json);
        }

        public string Get(string filename)
        {
            if (!File.Exists(filename))
            {
                return null;
            }

            string json = File.ReadAllText(filename);
            var data = JsonSerializer.Deserialize<dynamic>(json);
            return $"{data.Name} {data.Surname}\nPhone: {data.Phone}\nAddress: {data.Address}";
        }

        public void Update(string filename, string name, string surname, string phone, string address)
        {
            if (!File.Exists(filename))
            {
                return;
            }

            string json = File.ReadAllText(filename);
            var data = JsonSerializer.Deserialize<dynamic>(json);
            data.Name = name;
            data.Surname = surname;
            data.Phone = phone;
            data.Address = address;
            json = JsonSerializer.Serialize(data);
            File.WriteAllText(filename, json);
        }
    }

    class XmlAdapter : IAdapter
    {
        public XmlAdapter() { }

        public void Add(string filename, string name, string surname, string phone, string address)
        {
            var data = new { Name = name, Surname = surname, Phone = phone, Address = address };
            var serializer = new XmlSerializer(typeof(object));
            using (var stream = new StreamWriter(filename))
            {
                serializer.Serialize(stream, data);
            }
        }

        public string Get(string filename)
        {
            if (!File.Exists(filename))
            {
                return null;
            }

            var serializer = new XmlSerializer(typeof(object));
            using (var stream = new StreamReader(filename))
            {
                var data = serializer.Deserialize(stream);
                dynamic obj = data;
                return $"{obj.Name} {obj.Surname}\nPhone: {obj.Phone}\nAddress: {obj.Address}";
            }
        }

        public void Update(string filename, string name, string surname, string phone, string address)
        {
            if (!File.Exists(filename))
            {
                return;
            }

            var serializer = new XmlSerializer(typeof(object));
            using (var stream = new StreamReader(filename))
            {
                var data = serializer.Deserialize(stream);
                dynamic obj = data;
                obj.Name = name;
                obj.Surname = surname;
                obj.Phone = phone;
                obj.Address = address;
                using (var writer = new StreamWriter(filename))
                {
                    serializer.Serialize(writer, data);
                }
            }
        }
    }

    public partial class MainWindow : Window
    {
        private IAdapter adapter;

        public MainWindow()
        {
            InitializeComponent();

            JSONBox.Checked += OnAdapterSelected;
            XMLBox.Checked += OnAdapterSelected;

            OnAdapterSelected(null, null);
        }

        private void OnAdapterSelected(object sender, RoutedEventArgs e)
        {
            if (JSONBox.IsChecked == true)
            {
                adapter = new JsonAdapter();
            }
            else if (XMLBox.IsChecked == true)
            {
                adapter = new XmlAdapter();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string filename = Filename.Text;
            string name = nameInput.Text;
            string surname = surnameInput.Text;
            string phone = phoneInput.Text;
            string address = addressInput.Text;
            adapter?.Add(filename, name, surname, phone, address);
        }

        private void GetButton_Click(object sender, RoutedEventArgs e)
        {
            string filename = Filename.Text;

            string result = adapter?.Get(filename);
            string name = nameInput.Text;

            if (result == null)
            {
                MessageBox.Show("File not found");
            }
            else
            {
                adapter?.Get(name);
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string filename = Filename.Text;
            string name = nameInput.Text;
            string surname = surnameInput.Text;
            string phone = phoneInput.Text;
            string address = addressInput.Text;

            adapter?.Update(filename, name, surname, phone, address);
        }
    } }