﻿using System.Collections.ObjectModel;
using Vortex.Presentation.Wpf;
using TcoCognexVision.Converters;
using Vortex.Connector;
using System.Text;

using System.Linq;
using System.Collections.Generic;
using System;
using TcOpen.Inxton.Input;
using RelayCommand = TcOpen.Inxton.Input.RelayCommand;
using Microsoft.Win32;
using System.Windows;
using TcoCore;
using StringBuilder = System.Text.StringBuilder;

namespace TcoCognexVision
{
    public class TcoDesigner_v_2_x_xViewModel : RenderableViewModel
    {
        private const string Messenger = "_messenger";
        private IEnumerable<TcoCore.IsTask> _tasks;

        private StringBuilder _sb = new StringBuilder();

        public TcoDesigner_v_2_x_xViewModel() : base()
        {
            Model = new TcoDesigner_v_2_x_x();
            GenerateSpecificSymbolsCommand = new RelayCommand(a => GenerateSpecificSymbols());
            GenerateGenericSymbolsCommand = new RelayCommand(a => GenerateGenericSymbols());

        }

        private void GenerateGenericSymbols()
        {

            GetSymbols(this.Component._genericData, Filter, ref _sb);
            try
            {
                
                var sfd = new SaveFileDialog();
                sfd.FileName = Component._genericData.AttributeName + DateTime.Now.ToString("_yyyyMMdd_HHmmss");
                sfd.DefaultExt = "txt";
                sfd.ShowDialog();


                using (var sw = new System.IO.StreamWriter(sfd.FileName))
                {


                    sw.Write(_sb);

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void GenerateSpecificSymbols()
        {
            _sb.Clear();
            GetSymbols(this.Component._specificData,Filter,ref _sb );
            try
            {
               
                var sfd = new SaveFileDialog();
                sfd.FileName = Component._specificData.AttributeName + DateTime.Now.ToString("_yyyyMMdd_HHmmss");
                sfd.DefaultExt = "txt";
                sfd.ShowDialog();


                using (var sw = new System.IO.StreamWriter(sfd.FileName))
                {


                    sw.Write(_sb);

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private  void GetSymbols(IVortexObject obj, string filter, ref StringBuilder sb)
        {

            var splitted = filter.Split(';');
           
            foreach (var item in obj.GetValueTags())
            {
                if (string.IsNullOrEmpty(filter) && !item.Symbol.Contains(Messenger))
                {

                    sb.AppendLine($"{item.Symbol};{((dynamic)item).Cyclic.GetType().ToString()}; ;");

                }
                else
                    foreach (var split in splitted)
                    {
                        if (item.Symbol.Contains(split)  && !item.Symbol.Contains(Messenger))
                        {

                            sb.AppendLine($"{item.Symbol};{((dynamic)item).Cyclic.GetType().ToString()};");

                        }
                    }
                

            }

            foreach (var item in obj.GetChildren())
            {
                GetSymbols(item, filter,ref _sb);
            }
        }
        public IEnumerable<IsTask> Tasks
        {
            get { if (_tasks == null) _tasks = Component.GetChildren<IsTask>();
                      var internalTasks =  _tasks.Where(p => p.Symbol.Contains("Internal"));
                _tasks = _tasks.Except(internalTasks); 
                return _tasks; }
        }

        public TcoDesigner_v_2_x_x Component { get; private set; }
        public override object Model 
        { 
            get => Component; 
            set 
            {
                Component = value as TcoDesigner_v_2_x_x;
      
            }
        }

        public string Filter { get; set; } = string.Empty;
        public RelayCommand GenerateSpecificSymbolsCommand { get; private set; }
        public RelayCommand GenerateGenericSymbolsCommand { get; private set; }
    }
    
    public class TcoDesigner_v_2_x_xServiceViewModel : TcoDesigner_v_2_x_xViewModel
    {
    }
}
