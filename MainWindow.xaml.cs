using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LaTeX_Homework_Spawner {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Boolean configLoaded;
        private JObject config;
        private string configFailReason;

        private const string HeaderPrefix = "Homework Spawner v2.0 by Ethan Levine.";
        private const string HeaderSuffix = "View README.txt for help.";

        public MainWindow() {
            InitializeComponent();
            string configFile = "hwmk-spawner.conf";
            if (Application.Current.Properties["CommandLineArgs"] != null) {
                configFile = ((string[]) Application.Current.Properties["CommandLineArgs"])[0];
            }
            tryLoadConfig(configFile);
            applyConfigMessage();
        }

        private void tryLoadConfig(String confFile) {
            configLoaded = false;
            try {
                using (FileStream fs = new FileStream(confFile, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    using (StreamReader fr = new StreamReader(fs)) {
                        config = JObject.Parse(fr.ReadToEnd());
                        configLoaded = true;
                        applyConfig();
                    }
                }
            } catch (FileNotFoundException) {
                configFailReason = "The configuration file, " + confFile + ", could not be found.";
            } catch (IOException) {
                configFailReason = "An IO failure occured while reading the configuration file, " + confFile;
            } catch (JsonReaderException) {
                configFailReason = "An error occured while parsing the configuration file, " + confFile;
            }
        }

        private void applyConfigMessage() {
            if (configLoaded) {
                txbInfo.Text = HeaderPrefix + "  Configuration loaded.  " + HeaderSuffix;
            } else {
                txbInfo.Text = HeaderPrefix + "  Configuration not loaded.  " + HeaderSuffix + "\n" + configFailReason;
            }
        }

        private void applyConfigToTextBox(string conf, TextBox box) {
            if (configLoaded && config.SelectToken(conf) != null) {
                box.Text = (string) config.SelectToken(conf);
            }
        }

        private void applyConfigToCheckBox(string conf, CheckBox box) {
            if (configLoaded && config.SelectToken(conf) != null) {
                box.IsChecked = (bool) config.SelectToken(conf);
            }
        }

        private void jumpIfNotEmpty(TextBox from, TextBox to) {
            if (from.IsFocused && from.Text != "") {
                to.Focus();
            }
        }

        private void applyPackageSelection(string confPrefix) {
            applyConfigToCheckBox(confPrefix + ".siunitx", chkPackageSiunitx);
            applyConfigToCheckBox(confPrefix + ".tikz", chkPackageTikz);
            applyConfigToCheckBox(confPrefix + ".circuitikz", chkPackageCircuitikz);
            applyConfigToCheckBox(confPrefix + ".automata", chkPackageAutomata);
            applyConfigToCheckBox(confPrefix + ".shapes", chkPackageShapes);
            applyConfigToCheckBox(confPrefix + ".backgrounds", chkPackageBackgrounds);
            applyConfigToCheckBox(confPrefix + ".listings", chkPackageListings);
            applyConfigToCheckBox(confPrefix + ".matlab_listings", chkPackageMatlabListings);
            applyConfigToCheckBox(confPrefix + ".graphicx", chkPackageGraphicx);
            applyConfigToCheckBox(confPrefix + ".epstopdf", chkPackageEpstopdf);
            applyConfigToCheckBox(confPrefix + ".scalefig", chkPackageScalefig);
            applyConfigToCheckBox(confPrefix + ".float", chkPackageFloat);
            applyConfigToCheckBox(confPrefix + ".wrapfig", chkPackageWrapfig);
        }

        private void applyConfig() {
            applyConfigToTextBox("DefaultHomeworkDirectory", txtHomeworkDirectory);
            applyConfigToTextBox("DefaultAuthorName", txtAuthorName);
            applyConfigToCheckBox("CloseOnSpawn", chkCloseOnSpawn);
            applyConfigToCheckBox("OpenEditorOnSpawn", chkOpenEditor);
            applyPackageSelection("DefaultPackages");
        }

        private void applyConfigForClass(string code) {
            applyPackageSelection("ClassSpecs." + code + ".PackageOptions");
            applyConfigToTextBox("ClassSpecs." + code + ".ClassName", txtClassName);
            applyConfigToTextBox("ClassSpecs." + code + ".InstructorName", txtInstructorName); 
        }

        private string extractClassCode(string code) {
            if (code.Contains("-"))
                return code.Substring(0, code.LastIndexOf('-'));
            else
                return code;
        }

        private string extractClassSection(string code) {
            if (code.Contains("-"))
                return code.Substring(code.LastIndexOf('-') + 1);
            else
                return "";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            txtHomeworkDirectory.Focus();
            jumpIfNotEmpty(txtHomeworkDirectory, txtAuthorName);
            jumpIfNotEmpty(txtAuthorName, txtClassCode);
        }

        private void txtClassCode_LostFocus(object sender, RoutedEventArgs e) {
            applyConfigForClass(extractClassCode(txtClassCode.Text));
            // TODO - Figure out why this auto-jump isn't working.
            jumpIfNotEmpty(txtClassName, txtInstructorName);
            jumpIfNotEmpty(txtInstructorName, txtHomeworkTitle);
        }

        private void spawnHomework() {
            string result = HomeworkSpawner.Spawn(txtHomeworkDirectory.Text, "template.v2.tex", new Dictionary<string, string> {
                {"AuthorName", txtAuthorName.Text},
                {"ClassCode", extractClassCode(txtClassCode.Text)},
                {"ClassSection", extractClassSection(txtClassCode.Text)},
                {"ClassName", txtClassName.Text},
                {"InstructorName", txtInstructorName.Text},
                {"HomeworkTitle", txtHomeworkTitle.Text},
                {"AssignDate", txtAssignDate.Text},
                {"DueDate", txtDueDate.Text},
                {"Problems", txtProblems.Text}
            }, new Dictionary<string, bool> {
                {"siunitx", (bool) chkPackageSiunitx.IsChecked},
                {"tikz", (bool) chkPackageTikz.IsChecked},
                {"circuitikz", (bool) chkPackageTikz.IsChecked && (bool) chkPackageCircuitikz.IsChecked},
                {"automata", (bool) chkPackageTikz.IsChecked && (bool) chkPackageAutomata.IsChecked},
                {"shapes", (bool) chkPackageTikz.IsChecked && (bool) chkPackageShapes.IsChecked},
                {"backgrounds", (bool) chkPackageTikz.IsChecked && (bool) chkPackageBackgrounds.IsChecked},
                {"listings", (bool) chkPackageListings.IsChecked},
                {"matlab_listings", (bool) chkPackageListings.IsChecked && (bool) chkPackageMatlabListings.IsChecked},
                {"graphicx", (bool) chkPackageGraphicx.IsChecked},
                {"epstopdf", (bool) chkPackageGraphicx.IsChecked && (bool) chkPackageEpstopdf.IsChecked},
                {"scalefig", (bool) chkPackageGraphicx.IsChecked && (bool) chkPackageScalefig.IsChecked},
                {"float", (bool) chkPackageGraphicx.IsChecked && (bool) chkPackageFloat.IsChecked},
                {"wrapfig", (bool) chkPackageGraphicx.IsChecked && (bool) chkPackageWrapfig.IsChecked},
            }, (bool) chkOpenEditor.IsChecked);
            if ((bool) chkCloseOnSpawn.IsChecked && result == "") {
                this.Close();
            }
            if (result != "") {
                txbInfo.Text += "\n" + result;
            }
        }

        private void btnSpawn_Click(object sender, RoutedEventArgs e) {
            spawnHomework();
        }

        private void txt_PreviewKeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                spawnHomework();
            }
        }
    }
}
