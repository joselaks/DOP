using DOP.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace DOP.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static readonly string SampleDocumentFile = "SampleExcelDocument.xlsx";

        public MainViewModel()
        {
            this.OpenSampleCommand = new DelegateCommand(this.OpenSample);
        }

        private Workbook workbook;
        public Workbook Workbook
        {
            get
            {
                return this.workbook ?? new Workbook();
            }
            set
            {
                if (this.workbook != value)
                {
                    this.workbook = value;
                    this.OnPropertyChanged("Workbook");
                }
            }
        }

        private ICommand openSampleCommand = null;
        public ICommand OpenSampleCommand
        {
            get
            {
                return this.openSampleCommand;
            }
            set
            {
                if (this.openSampleCommand != value)
                {
                    this.openSampleCommand = value;
                    OnPropertyChanged("OpenSampleCommand");
                }
            }
        }

        private void OpenSample(object param)
        {
            using (Stream stream = FileHelper.GetSampleResourceStream(MainViewModel.SampleDocumentFile))
            {
                this.Workbook = new XlsxFormatProvider().Import(stream);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
