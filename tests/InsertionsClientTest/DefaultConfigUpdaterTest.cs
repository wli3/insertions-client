﻿using Microsoft.Net.Insertions.Api;
using Microsoft.Net.Insertions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InsertionsClientTest
{
	[TestClass]
	public class DefaultConfigUpdaterTest
	{
		[TestMethod()]
		public void TestLoadNullFile()
		{
			DefaultConfigUpdater updater = new DefaultConfigUpdater();
			Assert.IsFalse(updater.TryLoad(null, out string error));
			Assert.IsNotNull(error);
		}

		[TestMethod]
		public void TestLoadMissingFile()
		{
			DefaultConfigUpdater updater = new DefaultConfigUpdater();
			Assert.IsFalse(updater.TryLoad("some nonexistant file.txt", out string error));
			Assert.IsNotNull(error);
		}

		[TestMethod]
		public void TestLoadFile()
		{
			DefaultConfigUpdater updater = new DefaultConfigUpdater();
			string defaultConfigPath = Path.Combine(Environment.CurrentDirectory, "Assets", "default.config");
			Assert.IsTrue(File.Exists(defaultConfigPath));
			Assert.IsTrue(updater.TryLoad(defaultConfigPath, out string error), error);
			Assert.IsNull(error);
		}

		[TestMethod]
		public void TestDefaultConfigContent()
		{
			// Load file
			DefaultConfigUpdater updater = new DefaultConfigUpdater();
			string defaultConfigPath = Path.Combine(Environment.CurrentDirectory, "Assets", "default.config");
			Assert.IsTrue(File.Exists(defaultConfigPath));
			Assert.IsTrue(updater.TryLoad(defaultConfigPath, out string error), error);
			Assert.IsNull(error);

			// Test Content: change a version in default.config
			Assert.IsTrue(updater.TryUpdatePackage("VS.Tools.Roslyn", "1.2.3.4.5", out _));
			List<FileSaveResult> saveResults = updater.Save();
			Assert.IsNotNull(saveResults);
			Assert.AreEqual(1, saveResults.Count);

			Assert.IsTrue(saveResults[0].DidSucceed);
		}

		[TestMethod]
		public void TestPackageConfigContent()
		{
			// Load file
			DefaultConfigUpdater updater = new DefaultConfigUpdater();
			string defaultConfigPath = Path.Combine(Environment.CurrentDirectory, "Assets", "default.config");
			Assert.IsTrue(File.Exists(defaultConfigPath));
			Assert.IsTrue(updater.TryLoad(defaultConfigPath, out string error), error);
			Assert.IsNull(error);

			// Test Content: change a version in a packageconfig
			Assert.IsTrue(updater.TryUpdatePackage("Microsoft.IdentityModel.Clients.ActiveDirectory", "1.2.3.4.5", out _));
			List<FileSaveResult> saveResults = updater.Save();
			Assert.IsNotNull(saveResults);
			Assert.AreEqual(1, saveResults.Count);

			Assert.IsTrue(saveResults[0].DidSucceed);
		}

		[TestMethod]
		public void TestMissingPackageUpdate()
		{
			// Load file
			DefaultConfigUpdater updater = new DefaultConfigUpdater();
			string defaultConfigPath = Path.Combine(Environment.CurrentDirectory, "Assets", "default.config");
			Assert.IsTrue(File.Exists(defaultConfigPath));
			Assert.IsTrue(updater.TryLoad(defaultConfigPath, out string error), error);
			Assert.IsNull(error);

			// Update a package that doesn't exist
			Assert.IsFalse(updater.TryUpdatePackage("Some.Package.Nobody.Created", "xxx", out _));
			List<FileSaveResult> saveResults = updater.Save();
			Assert.IsNotNull(saveResults);
			Assert.AreEqual(saveResults.Count, 0);
		}

		[TestMethod]
		public void TestSetCorrectVersionDefaultConfig()
		{
			// Load file
			DefaultConfigUpdater updater = new DefaultConfigUpdater();
			string defaultConfigPath = Path.Combine(Environment.CurrentDirectory, "Assets", "default.config");
			Assert.IsTrue(File.Exists(defaultConfigPath));
			Assert.IsTrue(updater.TryLoad(defaultConfigPath, out string error), error);
			Assert.IsNull(error);

			// Test Content
			string versionNumber = Guid.NewGuid().ToString();
			// Edit a package that is inside default.config
			Assert.IsTrue(updater.TryUpdatePackage("VS.Redist.X86.Retail.Bin.I386.HelpDocs.Intellisense.NETPortableV4_0.1055", versionNumber, out _));
			List<FileSaveResult> saveResults = updater.Save();
			Assert.IsNotNull(saveResults);
			Assert.AreEqual(saveResults.Count, 1);

			Assert.IsTrue(saveResults[0].DidSucceed);
			Assert.IsTrue(File.ReadAllText(saveResults[0].Path).Contains(versionNumber), "Written version number was not found in the saved file.");
		}

		[TestMethod]
		public void TestSetCorrectVersionPackageConfig()
		{
			// Load file
			DefaultConfigUpdater updater = new DefaultConfigUpdater();
			string defaultConfigPath = Path.Combine(Environment.CurrentDirectory, "Assets", "default.config");
			Assert.IsTrue(File.Exists(defaultConfigPath));
			Assert.IsTrue(updater.TryLoad(defaultConfigPath, out string error), error);
			Assert.IsNull(error);

			// Test Content
			string versionNumber = Guid.NewGuid().ToString();
			// Edit a package that is inside a packageconfig file
			Assert.IsTrue(updater.TryUpdatePackage("Microsoft.VisualStudio.Language.NavigateTo.Implementation", versionNumber, out _));
			List<FileSaveResult> saveResults = updater.Save();
			Assert.IsNotNull(saveResults);
			Assert.AreEqual(saveResults.Count, 1);

			Assert.IsTrue(saveResults[0].DidSucceed);
			Assert.IsTrue(File.ReadAllText(saveResults[0].Path).Contains(versionNumber), "Written version number was not found in the saved file.");
		}
	}
}
