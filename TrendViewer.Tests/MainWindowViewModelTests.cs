using TrendViewer.DataProcessing;
using TrendViewer.Tests.Utilities;
using Moq;
using TrendViewer.Helpers;
using TrendViewer.Models;
using TrendViewer.ViewModels;
using Xunit;

namespace TrendViewer.Tests
{
    /// <summary>
    /// Tests for <see cref="MainWindowViewModel"/>
    /// </summary>
    public class MainWindowViewModelTests
    {
        private readonly IMainWindowViewModel _model;
        private readonly Mock<IMainWindowModel> _dataModelMock = new Mock<IMainWindowModel>();
        private readonly Mock<IOpenFileDialogHelper> _openFileDialogHelperMock = new Mock<IOpenFileDialogHelper>();

        /// <summary>
        /// Model initialization
        /// </summary>
        public MainWindowViewModelTests()
        {
            _model = new MainWindowViewModel(_dataModelMock.Object, _openFileDialogHelperMock.Object);
        }

        /// <summary>
        /// Test for recalculate command
        /// </summary>
        [Fact]
        public void RecalculateCommand_Test()
        {
            // Act
            _model.RecalculateCommand.Execute(null);

            // Assert
            _dataModelMock.Verify(x => x.RecalculateStatistics(It.IsAny<int>()), Times.Once);
        }

        /// <summary>
        /// Test for open file command
        /// </summary>
        [Fact]
        public void OpenFileCommand_Test()
        {
            // Arrange
            _openFileDialogHelperMock
                .Setup(x => x.OpenDialog())
                .Returns("filePath");

            // Act
            _model.OpenFileCommand.Execute(null);

            // Assert
            _dataModelMock.Verify(x => x.OnFilePathUpdated(It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Test for FilePath property raises the NotifyPropertyChanged event
        /// </summary>
        [Fact]
        public void FilePath_WillRaiseNotifyEvent_Test()
        {
            // Assert
            _model.ShouldNotifyOn(vm => vm.FilePath)
                .When(vm => vm.FilePath = "string");
        }

        /// <summary>
        /// Test for IsDataLoaded property raises the NotifyPropertyChanged event
        /// </summary>
        [Fact]
        public void IsDataLoaded_WillRaiseNotifyEvent_Test()
        {
            // Assert
            _model.ShouldNotifyOn(vm => vm.IsDataLoaded)
                .When(vm => vm.IsDataLoaded = true);
        }

        /// <summary>
        /// Test for NumberOfValues property raises the NotifyPropertyChanged event
        /// </summary>
        [Fact]
        public void NumberOfValues_WillRaiseNotifyEvent_Test()
        {
            // Assert
            _model.ShouldNotifyOn(vm => vm.NumberOfValues)
                .When(vm => vm.NumberOfValues = 20);
        }
    }
    
}