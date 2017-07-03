﻿using SM64_Diagnostic.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SM64_Diagnostic.Structs
{
    public class ControllerImageGui
    {
        public Image ButtonAImage;
        public Image ButtonBImage;
        public Image ButtonZImage;
        public Image ButtonStartImage;
        public Image ButtonRImage;
        public Image ButtonLImage;
        public Image ButtonCUpImage;
        public Image ButtonCDownImage;
        public Image ButtonCLeftImage;
        public Image ButtonCRightImage;
        public Image ButtonDUpImage;
        public Image ButtonDDownImage;
        public Image ButtonDLeftImage;
        public Image ButtonDRightImage;
        public Image ControlStickImage;
        public Image ControllerBaseImage;

        /*
        public CheckBox LockLabelsCheckbox;
        public TabControl TabControl;
        public ComboBox MapObjectToggleModeComboBox;
        public ComboBox SortMethodComboBox;
        public ComboBox LabelMethodComboBox;
        public NoTearFlowLayoutPanel FlowLayoutContainer;
        */

        ~ControllerImageGui()
        {
            ButtonAImage?.Dispose();
            ButtonBImage?.Dispose();
            ButtonZImage?.Dispose();
            ButtonStartImage?.Dispose();
            ButtonRImage?.Dispose();
            ButtonLImage?.Dispose();
            ButtonCUpImage?.Dispose();
            ButtonCDownImage?.Dispose();
            ButtonCLeftImage?.Dispose();
            ButtonCRightImage?.Dispose();
            ButtonDUpImage?.Dispose();
            ButtonDDownImage?.Dispose();
            ButtonDLeftImage?.Dispose();
            ButtonDRightImage?.Dispose();
            ControlStickImage?.Dispose();
            ControllerBaseImage?.Dispose();
        }
    }
}