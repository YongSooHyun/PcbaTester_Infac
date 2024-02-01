using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.ViewModel
{

    internal class StepEditViewModel
    {
        /// <summary>
        /// 현재 편집중인 스텝.
        /// </summary>
        internal EolStep CurrentStep { get; private set; }

        private readonly System.Collections.IList stepList;
        private readonly EolStep originalStep;

        /// <summary>
        /// 스텝 유형을 표현하는 이미지.
        /// </summary>
        internal Image StepImage { get; private set; }

        /// <summary>
        /// 변경된 내용이 있는가를 나타낸다.
        /// </summary>
        internal bool Modified { get; private set; }

        internal StepEditViewModel(System.Collections.IList steps, object step)
        {
            stepList = steps;

            var eolStep = step as EolStep;
            originalStep = eolStep;

            if (CurrentStep != null)
            {
                CurrentStep.PropertyChanged -= Step_PropertyChanged;
            }
            CurrentStep = eolStep.Clone() as EolStep;
            CurrentStep.PropertyChanged += Step_PropertyChanged;
        }

        // 스텝의 프로퍼티가 변경될 때 호출됨.
        private void Step_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EolStep.RunResult))
            {
            }
            else
            {
                Modified = true;
            }
        }

        /// <summary>
        /// 스텝을 편집하는 폼의 타이틀을 만든다.
        /// </summary>
        /// <returns></returns>
        internal string GetTitle()
        {
            string title = "Edit Test Step";
            if (CurrentStep != null)
            {
                title += " - " + (Modified ? SequenceViewModel.ModifiedMark : "") + CurrentStep.Name;
            }
            return title;
        }

        /// <summary>
        /// 현재 스텝을 저장한다.
        /// </summary>
        internal void Save()
        {
            if (CurrentStep == null || originalStep == null)
            {
                return;
            }

            CurrentStep.CopyTo(originalStep);
            Modified = false;
        }

        /// <summary>
        /// 현재 스텝의 앞에 있는 스텝을 불러온다.
        /// </summary>
        /// <returns></returns>
        internal object GetPreviousStep()
        {
            if (stepList != null && originalStep != null)
            {
                int index = stepList.IndexOf(originalStep);
                if (index > 0)
                {
                    return stepList[index - 1];
                }
            }

            return null;
        }

        /// <summary>
        /// 현재 스텝의 뒤에 있는 스텝을 불러온다.
        /// </summary>
        /// <returns></returns>
        internal object GetNextStep()
        {
            if (stepList != null && originalStep != null)
            {
                int index = stepList.IndexOf(originalStep);
                if (index < stepList.Count - 1)
                {
                    return stepList[index + 1];
                }
            }

            return null;
        }

        /// <summary>
        /// 현재 스텝을 실행한다.
        /// </summary>
        /// <returns></returns>
        internal EolStep.TestResult RunCurrentStep(object elozTestSet, CancellationToken token)
        {
            try
            {
                return CurrentStep?.Execute(elozTestSet, true, token);
            }
            finally
            {
                //TestDevice.CloseAllDevices();
            }
        }

        internal bool GetCellBackColor(string propertyName, object modelObject, out Color backColor)
        {
            if (modelObject is EolStep.TestResult testResult)
            {
                switch (propertyName)
                {
                    case nameof(EolStep.TestResult.ResultStateDesc):
                        testResult.ResultState.GetColors(out backColor, out _);
                        return true;
                }
            }

            backColor = Color.White;
            return false;
        }

        internal static void RunShellImageViewer(Image image)
        {
            if (image == null)
            {
                return;
            }

            string extension;
            if (ImageFormat.Bmp.Equals(image.RawFormat))
            {
                extension = ".bmp";
            }
            else
            {
                extension = ".png";
            }

            var tempFile = Path.GetTempFileName() + extension;
            image.Save(tempFile, image.RawFormat);
            Utils.StartProcess(tempFile);
        }

        /// <summary>
        /// CP 프로퍼티를 보여주거나 숨긴다.
        /// </summary>
        internal void ToggleHiddenProperties()
        {
            if (CurrentStep != null)
            {
                CurrentStep.ToggleHiddenProperties();
            }
        }
    }
}
