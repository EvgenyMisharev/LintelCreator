using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

namespace LintelCreator
{
    /// <summary>
    /// Логика взаимодействия для LintelCreatorWPF.xaml
    /// </summary>
    public partial class LintelCreatorWPF : Window
    {
        Document Doc;
        public ObservableCollection<SymboolParameters> SymbolParametersCollection;
        public ObservableCollection<DimensionParameters> DimensionParametersCollection;
        public List<FamilySymbol> SymbolsListForTypesComboBoxColumn;
        List<FamilySymbol> FamilySymbolsForSymbolsList;

        public FamilySymbol LintelTargetFamilySymbol;

        List<Parameter> OpeningHeightParamList;
        List<Parameter> OpeningWidthParamList;

        public Parameter OpeningHeightParam;
        public Parameter OpeningWidthParam;

        LintelCreatorSettings Settings = null;

        public LintelCreatorWPF(Document doc, List<Family> lintelFamilysList, List<Parameter> openingParameters)
        {
            Doc = doc;
            OpeningWidthParamList = openingParameters.ToList();
            OpeningHeightParamList = openingParameters.ToList();

            InitializeComponent();
            comboBox_LintelFamilies.ItemsSource = lintelFamilysList;
            comboBox_LintelFamilies.DisplayMemberPath = "Name";
            comboBox_LintelFamilies.SelectedItem = comboBox_LintelFamilies.Items.GetItemAt(0);


            comboBox_OpeningHeight.ItemsSource = OpeningHeightParamList;
            comboBox_OpeningHeight.DisplayMemberPath = "Definition.Name";
            comboBox_OpeningHeight.SelectedItem = comboBox_OpeningHeight.Items.GetItemAt(0);

            comboBox_OpeningWidth.ItemsSource = OpeningWidthParamList;
            comboBox_OpeningWidth.DisplayMemberPath = "Definition.Name";
            comboBox_OpeningWidth.SelectedItem = comboBox_OpeningWidth.Items.GetItemAt(0);

            Settings = LintelCreatorSettings.GetSettings();
            if(Settings != null)
            {
                if (Settings.SelectedLintelFamilieName != null)
                {
                    Family family = lintelFamilysList.FirstOrDefault(f => f.Name == Settings.SelectedLintelFamilieName);
                    if(family != null)
                    {
                        comboBox_LintelFamilies.SelectedItem = family;
                    }
                    else
                    {
                        comboBox_LintelFamilies.SelectedItem = comboBox_LintelFamilies.Items.GetItemAt(0);
                    }
                }
               
                if (Settings.SelectedFamilySymbolName != null)
                {
                    FamilySymbol familySymbol = FamilySymbolsForSymbolsList.FirstOrDefault(fs => fs.Name == Settings.SelectedFamilySymbolName);
                    if(familySymbol != null)
                    {
                        listBox_SymbolsList.SelectedItem = familySymbol;
                    }
                    else
                    {
                        listBox_SymbolsList.SelectedItem = listBox_SymbolsList.Items.GetItemAt(0);
                    }
                }
                if (Settings.SelectedOpeningHeightParameterName != null)
                {
                    Parameter parameter = OpeningHeightParamList.FirstOrDefault(hp => hp.Definition.Name == Settings.SelectedOpeningHeightParameterName);
                    if(parameter != null)
                    {
                        comboBox_OpeningHeight.SelectedItem = parameter;
                    }
                    else
                    {
                        comboBox_OpeningHeight.SelectedItem = comboBox_OpeningHeight.Items.GetItemAt(0);
                    }
                }

                if (Settings.SelectedOpeningWidthParameterName != null)
                {
                    Parameter parameter = OpeningWidthParamList.FirstOrDefault(wp => wp.Definition.Name == Settings.SelectedOpeningWidthParameterName);
                    if (parameter != null)
                    {
                        comboBox_OpeningWidth.SelectedItem = parameter;
                    }
                    else
                    {
                        comboBox_OpeningWidth.SelectedItem = comboBox_OpeningWidth.Items.GetItemAt(0);
                    }
                }
            }
        }

        private void comboBox_LintelFamilies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<ElementId> familySymbolsIdList = ((sender as ComboBox).SelectedItem as Family).GetFamilySymbolIds().ToList();
            FamilySymbolsForSymbolsList = new List<FamilySymbol>();
            if (familySymbolsIdList.Count != 0)
            {
                foreach (ElementId symbolId in familySymbolsIdList)
                {
                    FamilySymbolsForSymbolsList.Add(Doc.GetElement(symbolId) as FamilySymbol);
                }
            }

            listBox_SymbolsList.ItemsSource = FamilySymbolsForSymbolsList;
            listBox_SymbolsList.DisplayMemberPath = "Name";

            textBox_SymbolName.Text = "";
        }

        private void listBox_SymbolsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SymbolParametersCollection = new ObservableCollection<SymboolParameters>();
            List <SymboolParameters> symbolParametersCollection = new List<SymboolParameters>();
            LintelTargetFamilySymbol = (sender as ListBox).SelectedItem as FamilySymbol;

            if(LintelTargetFamilySymbol != null)
            {
                //Параметры вложенных типов 
                textBox_SymbolName.Text = LintelTargetFamilySymbol.Name;

                ParameterSet symboolParameterSet = LintelTargetFamilySymbol.Parameters;
                foreach(Parameter param in symboolParameterSet)
                {
                    if (((int)(param.Definition).ParameterType).Equals((int)ParameterType.FamilyType))
                    {
                        symbolParametersCollection.Add(new SymboolParameters(param, Doc.GetElement(param.AsElementId()) as FamilySymbol));
                    }
                }

                SymbolsListForTypesComboBoxColumn = new List<FamilySymbol>();
                foreach (SymboolParameters symboolParameters in symbolParametersCollection)
                {
                    List<ElementId> symbolsIdListForTypesComboBoxColumn = symboolParameters.FamilySymbolValue.Family.GetFamilySymbolIds().ToList();
                    foreach(ElementId symbolId in symbolsIdListForTypesComboBoxColumn)
                    {
                        FamilySymbol familySymbolForTypesComboBoxColumn = Doc.GetElement(symbolId) as FamilySymbol;

                        if (SymbolsListForTypesComboBoxColumn.FirstOrDefault(fs=>fs.Name == familySymbolForTypesComboBoxColumn.Name) == null)
                        {
                            SymbolsListForTypesComboBoxColumn.Add(familySymbolForTypesComboBoxColumn);
                        }
                    }
                }
                SymbolsListForTypesComboBoxColumn = SymbolsListForTypesComboBoxColumn.OrderBy(fs => fs.Name, new AlphanumComparatorFastString()).ToList();

                if (symbolParametersCollection.Count != 0)
                {
                    dataGrid_TypesParamList.IsEnabled = true;

                    SymbolParametersCollection = new ObservableCollection<SymboolParameters>(
                        symbolParametersCollection.OrderBy(kv => kv.ParameterValue.Definition.Name, new AlphanumComparatorFastString()));

                    foreach (SymboolParameters sP in SymbolParametersCollection)
                    {
                        sP.FamilySymbolValue = SymbolsListForTypesComboBoxColumn.FirstOrDefault(fs => fs.Name == sP.FamilySymbolValue.Name);
                    }
                }
                else
                {
                    dataGrid_TypesParamList.IsEnabled = false;
                }

                dataGrid_TypesParamList.ItemsSource = SymbolParametersCollection;
                dataGrid_TypesComboBoxColumn.ItemsSource = SymbolsListForTypesComboBoxColumn;


                //Параметры размеров
                DimensionParametersCollection = new ObservableCollection<DimensionParameters>();
                List<DimensionParameters> dimensionParametersCollection = new List<DimensionParameters>();
                foreach (Parameter param in symboolParameterSet)
                {
                    if (((int)(param.Definition).ParameterGroup).Equals((int)BuiltInParameterGroup.PG_GEOMETRY) && param.IsReadOnly == false)
                    {
                        DimensionParameters dimensionParameter = new DimensionParameters(param.Definition.Name, param.AsValueString(), param.Id);
                        dimensionParametersCollection.Add(dimensionParameter);
                    }
                }
                if (dimensionParametersCollection.Count != 0)
                {
                    DimensionParametersCollection = new ObservableCollection<DimensionParameters>(
                        dimensionParametersCollection.OrderBy(p => p.Name, new AlphanumComparatorFastString()));
                }

                dataGrid_DimensionsParamList.ItemsSource = DimensionParametersCollection;
            }
        }


        private void btn_SaveNewSymbol_Click(object sender, RoutedEventArgs e)
        {
            using (Transaction t = new Transaction(Doc))
            {
                FamilySymbol selectedFamilySymbol = listBox_SymbolsList.SelectedItem as FamilySymbol;
                if (selectedFamilySymbol == null)
                {
                    selectedFamilySymbol = listBox_SymbolsList.Items.GetItemAt(0) as FamilySymbol;
                }
                t.Start($"Сохранение типа {textBox_SymbolName.Text}");
                if (selectedFamilySymbol != null)
                {
                    List<ElementId> symbolsIdList = selectedFamilySymbol.Family.GetFamilySymbolIds().ToList();
                    List<FamilySymbol> symbolsList = new List<FamilySymbol>();
                    foreach(ElementId id in symbolsIdList)
                    {
                        symbolsList.Add(Doc.GetElement(id) as FamilySymbol);
                    }
                    FamilySymbol symbolWithName = symbolsList.FirstOrDefault(sy => sy.Name == textBox_SymbolName.Text);
                    if(symbolWithName != null)
                    {
                        if(SymbolParametersCollection.Count != 0)
                        {
                            foreach(SymboolParameters symboolParameters in SymbolParametersCollection)
                            {
                                selectedFamilySymbol.LookupParameter(symboolParameters.ParameterValue.Definition.Name).Set(symboolParameters.FamilySymbolValue.Id);
                            }
                        }
                        if(DimensionParametersCollection.Count != 0)
                        {
                            foreach (DimensionParameters dimensionParameters in DimensionParametersCollection)
                            {
                                selectedFamilySymbol.LookupParameter(dimensionParameters.Name).SetValueString(dimensionParameters.ValueString);
                            }
                        }
                    }
                    else
                    {
                        selectedFamilySymbol = selectedFamilySymbol.Duplicate(textBox_SymbolName.Text) as FamilySymbol;
                        if (SymbolParametersCollection.Count != 0)
                        {
                            foreach (SymboolParameters symboolParameters in SymbolParametersCollection)
                            {
                                selectedFamilySymbol.LookupParameter(symboolParameters.ParameterValue.Definition.Name).Set(symboolParameters.FamilySymbolValue.Id);
                            }
                        }
                        if (DimensionParametersCollection.Count != 0)
                        {
                            foreach (DimensionParameters dimensionParameters in DimensionParametersCollection)
                            {
                                selectedFamilySymbol.LookupParameter(dimensionParameters.Name).SetValueString(dimensionParameters.ValueString);
                            }
                        }
                    }

                    List<ElementId> familySymbolsIdList = selectedFamilySymbol.Family.GetFamilySymbolIds().ToList();
                    FamilySymbolsForSymbolsList = new List<FamilySymbol>();
                    if (familySymbolsIdList.Count != 0)
                    {
                        foreach (ElementId symbolId in familySymbolsIdList)
                        {
                            FamilySymbolsForSymbolsList.Add(Doc.GetElement(symbolId) as FamilySymbol);
                        }
                    }

                    listBox_SymbolsList.ItemsSource = FamilySymbolsForSymbolsList;
                    listBox_SymbolsList.DisplayMemberPath = "Name";
                    listBox_SymbolsList.SelectedItem = selectedFamilySymbol;
                    LintelTargetFamilySymbol = selectedFamilySymbol;
                }
                t.Commit();
            }
        }

        private void btn_DeleteSymbol_Click(object sender, RoutedEventArgs e)
        {
            using(Transaction t = new Transaction(Doc))
            {
                FamilySymbol symbolForDelete = listBox_SymbolsList.SelectedItem as FamilySymbol;
                if(symbolForDelete != null)
                {
                    Family family = symbolForDelete.Family;
                    t.Start($"Удаление типа {(listBox_SymbolsList.SelectedItem as FamilySymbol).Name}");
                    if (family.GetFamilySymbolIds().Count > 1)
                    {
                        Doc.Delete(symbolForDelete.Id);
                    }
                    t.Commit();

                    List<ElementId> familySymbolsIdList = family.GetFamilySymbolIds().ToList();
                    FamilySymbolsForSymbolsList = new List<FamilySymbol>();
                    if (familySymbolsIdList.Count != 0)
                    {
                        foreach (ElementId symbolId in familySymbolsIdList)
                        {
                            FamilySymbolsForSymbolsList.Add(Doc.GetElement(symbolId) as FamilySymbol);
                        }
                    }

                    listBox_SymbolsList.ItemsSource = FamilySymbolsForSymbolsList;
                    listBox_SymbolsList.DisplayMemberPath = "Name";
                }
            }
        }


        private void comboBox_OpeningHeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            label_OpeningHeightValue.Content = ((sender as ComboBox).SelectedItem as Parameter).AsValueString();
        }

        private void comboBox_OpeningWidth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            label_OpeningWidthValue.Content = ((sender as ComboBox).SelectedItem as Parameter).AsValueString();
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            OpeningHeightParam = comboBox_OpeningHeight.SelectedItem as Parameter;
            OpeningWidthParam = comboBox_OpeningWidth.SelectedItem as Parameter;
            SaveSettings();
            DialogResult = true;
            Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void LintelCreatorWPF_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                OpeningHeightParam = comboBox_OpeningHeight.SelectedItem as Parameter;
                OpeningWidthParam = comboBox_OpeningWidth.SelectedItem as Parameter;
                SaveSettings();
                DialogResult = true;
                Close();
            }

            else if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }

        private void SaveSettings()
        {
            Settings.SelectedLintelFamilieName = (comboBox_LintelFamilies.SelectedItem as Family).Name;
            Settings.SelectedFamilySymbolName = LintelTargetFamilySymbol.Name;
            Settings.SelectedOpeningHeightParameterName = OpeningHeightParam.Definition.Name;
            Settings.SelectedOpeningWidthParameterName = OpeningWidthParam.Definition.Name;
            Settings.Save();
        }
    }
}
