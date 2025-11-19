using ClipboardHistoryManager.Services;
using ClipboardHistoryManager.ViewModels;
using NUnit.Framework;

namespace Tests
{
    public class MockClipboardService : IClipboardService
    {
        public string Text { get; private set; }
        public void SetText(string text)
        {
            Text = text;
        }
    }

    public class MockNotificationService : INotificationService
    {
        public string Message { get; private set; }
        public string Title { get; private set; }
        public void ShowNotification(string message, string title)
        {
            Message = message;
            Title = title;
        }
    }

    [TestFixture]
    public class MainViewModelTests
    {
        private MainViewModel _viewModel;
        private MockClipboardService _clipboardService;
        private MockNotificationService _notificationService;

        [SetUp]
        public void Setup()
        {
            _clipboardService = new MockClipboardService();
            _notificationService = new MockNotificationService();
            _viewModel = new MainViewModel(_clipboardService, _notificationService);
        }

        [Test]
        public void AddHistoryItem_AddsNewItem()
        {
            _viewModel.AddHistoryItem("test");
            Assert.That(_viewModel.ClipboardHistory.Count, Is.EqualTo(1));
            Assert.That(_viewModel.ClipboardHistory[0].Text, Is.EqualTo("test"));
        }

        [Test]
        public void AddHistoryItem_HandlesDuplicateItem()
        {
            _viewModel.AddHistoryItem("test");
            _viewModel.AddHistoryItem("test2");
            _viewModel.AddHistoryItem("test");
            Assert.That(_viewModel.ClipboardHistory.Count, Is.EqualTo(2));
            Assert.That(_viewModel.ClipboardHistory[0].Text, Is.EqualTo("test"));
        }

        [Test]
        public void AddHistoryItem_ManagesHistorySize()
        {
            for (int i = 0; i < 15; i++)
            {
                _viewModel.AddHistoryItem($"test{i}");
            }
            Assert.That(_viewModel.ClipboardHistory.Count, Is.EqualTo(10));
            Assert.That(_viewModel.ClipboardHistory[0].Text, Is.EqualTo("test14"));
        }

        [Test]
        public void RemoveItem_RemovesItem()
        {
            _viewModel.AddHistoryItem("test");
            var item = _viewModel.ClipboardHistory[0];
            _viewModel.RemoveItemCommand.Execute(item);
            Assert.That(_viewModel.ClipboardHistory.Count, Is.EqualTo(0));
        }

        [Test]
        public void LoadHistory_LoadsHistory()
        {
            var history = new List<string> { "test1", "test2" };
            _viewModel.LoadHistory(history);
            Assert.That(_viewModel.ClipboardHistory.Count, Is.EqualTo(2));
            Assert.That(_viewModel.ClipboardHistory[0].Text, Is.EqualTo("test1"));
        }
    }
}
