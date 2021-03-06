﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SM64_Diagnostic.Structs;
using System.Windows.Forms;
using SM64_Diagnostic.Utilities;
using SM64_Diagnostic.Controls;
using System.Drawing;
using SM64_Diagnostic.Structs.Configurations;

namespace SM64_Diagnostic.Managers
{
    public class DataManager
    {
        protected List<IDataContainer> _dataControls;
        protected NoTearFlowLayoutPanel _variableTable;
        protected List<IDataContainer> _specialWatchVars;

        public DataManager(List<WatchVariable> data, NoTearFlowLayoutPanel variableTable)
        {
            _variableTable = variableTable;
            _dataControls = new List<IDataContainer>();
            InitializeSpecialVariables();

            AddWatchVariables(data);
        }

        protected void RemoveWatchVariables(IEnumerable<IDataContainer> watchVars)
        {
            foreach (var watchVar in watchVars)
            {
                if (_dataControls.Contains(watchVar))
                    _dataControls.Remove(watchVar);

                _variableTable.Controls.Remove(watchVar.Control);
            }
        }

        protected List<IDataContainer> AddWatchVariables(IEnumerable<WatchVariable> watchVars, Color? color = null)
        {
            var newControls = new List<IDataContainer>();
            foreach (WatchVariable watchVar in watchVars)
            {
                if (watchVar.IsSpecial && _specialWatchVars != null)
                {
                    if (_specialWatchVars.Exists(w => w.SpecialName == watchVar.SpecialType))
                    {
                        var specialVar = _specialWatchVars.Find(w => w.SpecialName == watchVar.SpecialType);
                        specialVar.Name = watchVar.Name;
                        _variableTable.Controls.Add(specialVar.Control);
                        newControls.Add(specialVar);
                        if (watchVar.BackroundColor.HasValue)
                            specialVar.Color = watchVar.BackroundColor.Value;
                        else if (color.HasValue)
                            specialVar.Color = color.Value;
                    }
                    else
                    {
                        var failedContainer = new DataContainer(watchVar.Name);
                        failedContainer.Text = "Couldn't Find";
                        _variableTable.Controls.Add(failedContainer.Control);
                    }
                    continue;
                }

                WatchVariableControl watchControl = new WatchVariableControl(watchVar);
                if (color.HasValue)
                    watchControl.Color = color.Value;
                _variableTable.Controls.Add(watchControl.Control);
                _dataControls.Add(watchControl);
                newControls.Add(watchControl);
            }

            return newControls;
        }

        protected virtual void InitializeSpecialVariables()
        {
        }

        public virtual void Update(bool updateView = false)
        {
            // Update watch variables
            foreach (var watchVar in _dataControls)
                watchVar.Update();
        }
    }
}
