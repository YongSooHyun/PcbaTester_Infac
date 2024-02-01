using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// 프로젝트 저장, 읽기를 진행한다.
    /// </summary>
    static class TestProjectManager
    {
        // 프로젝트 파일 확장자.
        internal const string FileExtension = "prj";

        internal static string GetFileName(string projectName)
        {
            return $"{projectName}.{FileExtension}";
        }

        /// <summary>
        /// 프로젝트 파일이 있는지 검사한다.
        /// </summary>
        /// <param name="projectPath">검사하려는 프로젝트 이름.</param>
        /// <returns></returns>
        internal static bool Exists(string projectPath)
        {
            return File.Exists(projectPath);
        }

        /// <summary>
        /// 프로젝트 파일을 만든다.
        /// </summary>
        /// <param name="projectPath">만들려는 프로젝트 경로.</param>
        internal static void CreateProjectFile(string projectPath)
        {
            var project = new TestProject();
            project.Path = projectPath;
            Save(project);
        }

        /// <summary>
        /// 프로젝트를 파일에 저장한다. 파일 이름은 프로젝트의 이름으로 된다.
        /// </summary>
        /// <param name="obj">보관하려는 프로젝트 인스턴스.</param>
        internal static void Save(TestProject obj, string oldPath = null)
        {
            // 이전 이름이 새 이름과 같지 않다면, 이름 변경을 먼저 한다.
            if (oldPath != null && !oldPath.Equals(obj.Path, StringComparison.OrdinalIgnoreCase))
            {
                File.Move(oldPath, obj.Path);
            }

            using (var writer = new StreamWriter(obj.Path))
            {
                var xmlSerializer = new XmlSerializer(typeof(TestProject), typeof(TestProject).Namespace);
                xmlSerializer.Serialize(writer, obj);
            }
        }

        /// <summary>
        /// 프로젝트를 파일로부터 로딩한다.
        /// </summary>
        /// <param name="projectPath">로딩하려는 프로젝트 경로.</param>
        /// <returns>로딩한 프로젝트 인스턴스.</returns>
        internal static TestProject Load(string projectPath)
        {
            return LoadFile(projectPath);
        }

        /// <summary>
        /// 프로젝트를 파일로부터 로딩한다.
        /// </summary>
        /// <param name="fileName">로딩하려는 프로젝트 파일 경로.</param>
        /// <returns>로딩한 프로젝트 인스턴스.</returns>
        internal static TestProject LoadFile(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var xmlSerializer = new XmlSerializer(typeof(TestProject), typeof(TestProject).Namespace);
                var obj = xmlSerializer.Deserialize(stream) as TestProject;
                obj.Path = fileName;

                // 오늘 날짜와 보관된 Today를 비교해 같지 않으면 Today Probe Count 들을 초기화한다.
                //if (obj.Today.Date != DateTime.Now.Date)
                //{
                //    obj.Today = DateTime.Now;
                //    obj.TodayProbeCount = 0;
                //    obj.TodayPassCount = 0;
                //    stream.Close();
                //    Save(obj);
                //}

                return obj;
            }
        }

        internal static bool ValidateValue(string projectPath, string propertyName, string propertyValue, StringBuilder errorMessage)
        {
            switch (propertyName)
            {
                case nameof(TestProject.Path):
                    if (string.IsNullOrEmpty(propertyValue))
                    {
                        errorMessage?.Append("빈 문자열은 입력할 수 없습니다.");
                        return false;
                    }

                    // 프로젝트 이름은 파일 이름이므로, 유효한 파일 이름인가를 검사한다.
                    if (!Utils.IsValidPath(propertyValue))
                    {
                        errorMessage?.Append("프로젝트 경로가 허용되지 않는 문자를 포함하고 있습니다.");
                        return false;
                    }

                    // 해당 이름을 가진 파일이 이미 존재하면 안된다.
                    if (!projectPath.Equals(propertyValue, System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (File.Exists(propertyValue))
                        {
                            errorMessage?.Append("해당 이름을 가진 파일이 이미 존재합니다.");
                            return false;
                        }
                    }
                    break;
                //case nameof(TestProject.FID):
                //    // FID는 1 ~ 255 까지의 값만을 허용한다.
                //    int fidMinValue = 1;
                //    int fidMaxValue = 255;
                //    if (!int.TryParse(propertyValue, out int parsedFid) || parsedFid < fidMinValue || parsedFid > fidMaxValue)
                //    {
                //        errorMessage?.Append($"{fidMinValue} ~ {fidMaxValue} 의 수를 입력하세요.");
                //        return false;
                //    }
                //    break;
                case nameof(TestProject.Panel):
                    // Panel은 1 ~ 8 까지의 값만을 허용한다.
                    int panelMinValue = 1;
                    int panelMaxValue = 8;
                    if (!int.TryParse(propertyValue, out int parsedPanel) || parsedPanel < panelMinValue || parsedPanel > panelMaxValue)
                    {
                        errorMessage?.Append($"{panelMinValue} ~ {panelMaxValue} 의 수를 입력하세요.");
                        return false;
                    }
                    break;
                case nameof(TestProject.IctProjectName):
                    // 파일이름이 유효한지 검사.
                    if (!Utils.IsValidFileName(propertyValue))
                    {
                        errorMessage?.Append("유효하지 않은 파일이름입니다.");
                        return false;
                    }
                    break;
            }

            return true;
        }
    }
}
