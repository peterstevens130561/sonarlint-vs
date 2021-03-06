﻿using Moq;
using Xunit;
using Xunit.Extensions;

namespace NuGet.Test
{
    public class PackageRestoreConsentTest
    {
        [Fact]
        public void MissingSettingsKeyReturnsFalseForIsGranted()
        {
            // Arrange
            var settings = new Mock<ISettings>();
            var environmentReader = new Mock<IEnvironmentVariableReader>();

            var packageRestore = new PackageRestoreConsent(settings.Object, environmentReader.Object);

            // Act
            bool isGranted = packageRestore.IsGranted;

            // Assert
            Assert.False(isGranted);
        }

        [Fact]
        public void IncorrectSettingsValueReturnsFalseForIsGranted()
        {
            // Arrange
            var settings = new Mock<ISettings>();
            settings.Setup(s => s.GetValue("packageRestore", "enabled")).Returns("wrong value");
            var environmentReader = new Mock<IEnvironmentVariableReader>();

            var packageRestore = new PackageRestoreConsent(settings.Object, environmentReader.Object);

            // Act
            bool isGranted = packageRestore.IsGranted;

            // Assert
            Assert.False(isGranted);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("true")]
        [InlineData("1 ")]
        [InlineData(" TRUE")]
        public void CorrectSettingsValueReturnsTrueForIsGranted(string settingsValue)
        {
            // Arrange
            var settings = new Mock<ISettings>();
            settings.Setup(s => s.GetValue("packageRestore", "enabled")).Returns(settingsValue);
            var environmentReader = new Mock<IEnvironmentVariableReader>();

            var packageRestore = new PackageRestoreConsent(settings.Object, environmentReader.Object);

            // Act
            bool isGranted = packageRestore.IsGranted;
            bool isGrantedInSettings = packageRestore.IsGrantedInSettings;

            // Assert
            Assert.True(isGranted);
            Assert.True(isGrantedInSettings);
        }

        [Theory]
        [InlineData("2")]
        [InlineData("wrong")]
        public void InCorrectEnvironmentVariableReturnsFalseForIsGranted(string environmentValue)
        {
            // Arrange
            var settings = Mock.Of<ISettings>();

            var environmentReader = new Mock<IEnvironmentVariableReader>();
            environmentReader.Setup(
                r => r.GetEnvironmentVariable("EnableNuGetPackageRestore")).
                Returns(environmentValue);

            var packageRestore = new PackageRestoreConsent(settings, environmentReader.Object);

            // Act
            bool isGranted = packageRestore.IsGranted;

            // Assert
            Assert.False(isGranted);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("   1")]
        [InlineData("1  ")]
        [InlineData("true")]
        [InlineData(" True")]
        [InlineData(" True   ")]
        public void CorrectEnvironmentVariableReturnsTrueForIsGranted(string environmentValue)
        {
            // Arrange
            var settings = new Mock<ISettings>();

            var environmentReader = new Mock<IEnvironmentVariableReader>();
            environmentReader.Setup(
                r => r.GetEnvironmentVariable("EnableNuGetPackageRestore")).
                Returns(environmentValue);

            var packageRestore = new PackageRestoreConsent(settings.Object, environmentReader.Object);

            // Act
            bool isGranted = packageRestore.IsGranted;

            // Assert
            Assert.True(isGranted);
        }

        [Theory]
        [InlineData("1", "")]
        [InlineData("", "1")]
        [InlineData("0", "1")]
        [InlineData("false", "true")]
        [InlineData("true", "false")]
        public void GrantingConsentInEitherSettingOrEnvironmentGrantsConsent(string settingsValue, string environmentValue)
        {
            // Arrange
            var settings = new Mock<ISettings>();
            var environmentReader = new Mock<IEnvironmentVariableReader>(); 
            
            settings.Setup(s => s.GetValue("packageRestore", "enabled")).Returns(settingsValue);
            environmentReader.Setup(r => r.GetEnvironmentVariable("EnableNuGetPackageRestore")).Returns(environmentValue);

            var packageRestore = new PackageRestoreConsent(settings.Object, environmentReader.Object);

            // Act
            bool isGranted = packageRestore.IsGranted;

            // Assert
            Assert.True(isGranted);
        }

        [Theory]
        [InlineData("", null, false)]
        [InlineData("  ", null, false)]
        [InlineData("0", "abcd", false)]
        [InlineData("blah", null, false)]
        [InlineData("", "false", false)]
        [InlineData("   ", "false", false)]
        [InlineData("   ", "0", false)]
        [InlineData("", "true", true)]
        [InlineData("   ", "true", true)]
        [InlineData("blah", "false", false)]
        public void IsGrantedFallsBackToEnvironmentVariableIfSettingsValueIsEmptyOfWhitespaceString(string settingsValue, string environmentValue, bool expected)
        {
            // Arrange
            var settings = new Mock<ISettings>(MockBehavior.Strict);
            settings.Setup(s => s.GetValue("packageRestore", "enabled")).Returns(settingsValue);

            var environmentReader = new Mock<IEnvironmentVariableReader>();
            environmentReader.Setup(
                r => r.GetEnvironmentVariable("EnableNuGetPackageRestore")).
                Returns(environmentValue);

            var packageRestore = new PackageRestoreConsent(settings.Object, environmentReader.Object);

            // Act
            bool isGranted = packageRestore.IsGranted;
            bool isGrantedInSettings = packageRestore.IsGrantedInSettings;

            // Assert
            Assert.Equal(expected, isGranted);
            Assert.False(isGrantedInSettings);
        }

        [Fact]
        public void SettingIsGrantedToFalseDeleteTheSectionInConfigFile()
        {
            // Arrange
            var settings = new Mock<ISettings>();
            settings.Setup(s => s.GetValue("packageRestore", "enabled")).Returns("true");
            var environmentReader = new Mock<IEnvironmentVariableReader>();

            var packageRestore = new PackageRestoreConsent(settings.Object, environmentReader.Object);

            // Act
            packageRestore.IsGrantedInSettings = false;

            // Assert
            settings.Verify(s => s.DeleteSection("packageRestore"), Times.Once());
        }

        [Fact]
        public void SettingIsGrantedToTrueSetsTheFlagInConfigFile()
        {
            // Arrange
            var settings = new Mock<ISettings>();
            settings.Setup(s => s.GetValue("packageRestore", "enabled"));
            var environmentReader = new Mock<IEnvironmentVariableReader>();

            var packageRestore = new PackageRestoreConsent(settings.Object, environmentReader.Object);

            // Act
            packageRestore.IsGrantedInSettings = true;

            // Assert
            settings.Verify(s => s.SetValue("packageRestore", "enabled", "True"), Times.Once());
        }
    }
}
