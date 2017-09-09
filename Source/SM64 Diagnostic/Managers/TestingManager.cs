﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SM64_Diagnostic.Structs;
using SM64_Diagnostic.Structs.Configurations;
using System.Windows.Forms;
using static SM64_Diagnostic.Structs.Configurations.Config;
using static SM64_Diagnostic.Structs.Configurations.PositionControllerRelativeAngleConfig;

namespace SM64_Diagnostic.Managers
{
    public class TestingManager
    {
        CheckBox _checkBoxTestingRecord;
        Button _buttonTestingClear;
        Button _buttonTestingShow;

        Label _labelMetric1Name;
        Label _labelMetric2Name;
        Label _labelMetric3Name;
        Label _labelMetric4Name;
        Label _labelMetric5Name;
        Label _labelMetric6Name;

        Label _labelMetric1Value;
        Label _labelMetric2Value;
        Label _labelMetric3Value;
        Label _labelMetric4Value;
        Label _labelMetric5Value;
        Label _labelMetric6Value;

        GroupBox _groupBoxVarToRecord;
        RadioButton _radioButtonMario;
        RadioButton _radioButtonPenguin;

        enum VarToRecord { Mario, Penguin };

        VarToRecord _varToRecord;

        Dictionary<int, VarState> _varStateDictionary;
        int? _previousTimer;
        int _currentTimer;
        int _globalTimerDiff;
        int _collisions;
        int _badCollisions;
        int _gaps;

        public TestingManager(TabPage tabControl)
        {
            _checkBoxTestingRecord = tabControl.Controls["checkBoxTestingRecord"] as CheckBox;
            _checkBoxTestingRecord.Click += (sender, e) => SetRecordOn(_checkBoxTestingRecord.Checked);
            _buttonTestingClear = tabControl.Controls["buttonTestingClear"] as Button;
            _buttonTestingClear.Click += (sender, e) => ClearData();
            _buttonTestingShow = tabControl.Controls["buttonTestingShow"] as Button;
            _buttonTestingShow.Click += (sender, e) => ShowData();

            _labelMetric1Name = tabControl.Controls["labelMetric1Name"] as Label;
            _labelMetric2Name = tabControl.Controls["labelMetric2Name"] as Label;
            _labelMetric3Name = tabControl.Controls["labelMetric3Name"] as Label;
            _labelMetric4Name = tabControl.Controls["labelMetric4Name"] as Label;
            _labelMetric5Name = tabControl.Controls["labelMetric5Name"] as Label;
            _labelMetric6Name = tabControl.Controls["labelMetric6Name"] as Label;

            _labelMetric1Value = tabControl.Controls["labelMetric1Value"] as Label;
            _labelMetric2Value = tabControl.Controls["labelMetric2Value"] as Label;
            _labelMetric3Value = tabControl.Controls["labelMetric3Value"] as Label;
            _labelMetric4Value = tabControl.Controls["labelMetric4Value"] as Label;
            _labelMetric5Value = tabControl.Controls["labelMetric5Value"] as Label;
            _labelMetric6Value = tabControl.Controls["labelMetric6Value"] as Label;

            _groupBoxVarToRecord = tabControl.Controls["groupBoxVarToRecord"] as GroupBox;
            _radioButtonMario = _groupBoxVarToRecord.Controls["radioButtonMario"] as RadioButton;
            _radioButtonMario.Click += (sender, e) => _varToRecord = VarToRecord.Mario;
            _radioButtonPenguin = _groupBoxVarToRecord.Controls["radioButtonPenguin"] as RadioButton;
            _radioButtonPenguin.Click += (sender, e) => _varToRecord = VarToRecord.Penguin;

            if (_radioButtonMario.Checked)
            {
                _varToRecord = VarToRecord.Mario;
            }
            else
            {
                _varToRecord = VarToRecord.Penguin;
            }

            _labelMetric1Name.Text = "Data Count:";
            _labelMetric2Name.Text = "Collisions:";
            _labelMetric3Name.Text = "Bad Collisions:";
            _labelMetric4Name.Text = "Gaps:";
            _labelMetric5Name.Text = "Timer:";
            _labelMetric6Name.Text = "Var to Record:";

            _varStateDictionary = new Dictionary<int, VarState>();
            ClearData();
        }

        public abstract class VarState
        {
            public abstract List<Object> VarValues();
        }

        public class VarStateMario : VarState
        {
            public float X;
            public float Y;
            public float Z;
            public ushort Angle;
            public float Vspd;
            public float Hspd;

            public static VarState GetCurrent()
            {
                return new VarStateMario()
                {
                    X = Config.Stream.GetSingle(Config.Mario.StructAddress + Config.Mario.XOffset),
                    Y = Config.Stream.GetSingle(Config.Mario.StructAddress + Config.Mario.YOffset),
                    Z = Config.Stream.GetSingle(Config.Mario.StructAddress + Config.Mario.ZOffset),
                    Angle = Config.Stream.GetUInt16(Config.Mario.StructAddress + Config.Mario.YawFacingOffset),
                    Vspd = Config.Stream.GetSingle(Config.Mario.StructAddress + Config.Mario.VSpeedOffset),
                    Hspd = Config.Stream.GetSingle(Config.Mario.StructAddress + Config.Mario.HSpeedOffset),
                };
            }

            public static List<string> VarNames()
            {
                return new List<string>()
                {
                    "X", "Y", "Z", "Angle", "Vspd", "Hspd"
                };
            }

            public override List<Object> VarValues()
            {
                return new List<Object>()
                {
                    X, Y, Z, Angle, Vspd, Hspd
                };
            }

            public static string VarNamesString()
            {
                return String.Join("\t", VarNames());
            }

            public override string ToString()
            {
                return String.Join("\t", VarValues());
            }

            public override bool Equals(object obj)
            {
                if (!(obj is VarStateMario)) return false;
                VarStateMario other = obj as VarStateMario;
                return Enumerable.SequenceEqual(this.VarValues(), other.VarValues());
            }
        }

        public class VarStatePenguin : VarState
        {
            public double Progress;

            public static VarStatePenguin GetCurrent()
            {
                return new VarStatePenguin()
                {
                    Progress = 0,
                };
            }

            public static List<string> VarNames()
            {
                return new List<string>()
                {
                    "Progress"
                };
            }

            public override List<Object> VarValues()
            {
                return new List<Object>()
                {
                    Progress
                };
            }

            public static string VarNamesString()
            {
                return String.Join("\t", VarNames());
            }

            public override string ToString()
            {
                return String.Join("\t", VarValues());
            }

            public override bool Equals(object obj)
            {
                if (!(obj is VarStatePenguin)) return false;
                VarStatePenguin other = obj as VarStatePenguin;
                return Enumerable.SequenceEqual(this.VarValues(), other.VarValues());
            }
        }

        private void SetRecordOn(bool recordOn)
        {
            if (recordOn) Config.RefreshRateFreq = 0;
            else Config.RefreshRateFreq = 30;
        }

        private void ClearData()
        {
            _varStateDictionary.Clear();
            _previousTimer = null;
            _currentTimer = 0;
            _globalTimerDiff = 0;
            _collisions = 0;
            _badCollisions = 0;
            _gaps = 0;
        }

        private void ShowData()
        {
            var triangleInfoForm = new TriangleInfoForm();
            triangleInfoForm.SetDictionary(_varStateDictionary, "Timer", VarStateMario.VarNamesString());
            triangleInfoForm.ShowDialog();
        }

        public void Update(bool updateView)
        {
            if (!updateView) return;

            // get current stream values
            uint marioObjAddress = Config.Stream.GetUInt32(Config.Mario.ObjectReferenceAddress);
            _currentTimer = Config.Stream.GetInt32(marioObjAddress + Config.ObjectSlots.TimerOffset);

            VarState varState = VarStateMario.GetCurrent();

            if (_checkBoxTestingRecord.Checked)
            {
                // check for key collisions
                bool keyCollision = _varStateDictionary.ContainsKey(_currentTimer);
                if (keyCollision) _collisions++;

                // check for value collisions
                bool valueCollision = keyCollision && _varStateDictionary[_currentTimer].Equals(varState);
                if (keyCollision && !valueCollision) _badCollisions++;

                // check the global timer difference
                if (_previousTimer != null)
                {
                    _globalTimerDiff = _currentTimer - _previousTimer.Value;
                    if (_globalTimerDiff > 1) _gaps += _globalTimerDiff - 1;
                }

                // update dictionary if need be
                if (!keyCollision) _varStateDictionary[_currentTimer] = varState;

                // update previous global timer value
                _previousTimer = _currentTimer;
            }

            _labelMetric1Value.Text = _varStateDictionary.Count.ToString();
            _labelMetric2Value.Text = _collisions.ToString();
            _labelMetric3Value.Text = _badCollisions.ToString();
            _labelMetric4Value.Text = _gaps.ToString();
            _labelMetric5Value.Text = _currentTimer.ToString();
            _labelMetric6Value.Text = _varToRecord.ToString();
        }
    }
}
