using Parcel.CoreEngine.Helpers;
using Parcel.NExT.Python.Helpers;
using Parcel.Standard.System;
using Parcel.Types;

namespace Parcel.Framework.Dashboard
{
    public sealed class DashboardSettings
    {
        public string Title { get; set; } = "My Dashboard";
    }
    public enum ChartType
    {
        LineChart
    }
    public sealed class ChartData(ChartType type, DataGrid data)
    {
        public ChartType ChartType { get; set; } = type;
        public DataGrid Data { get; set; } = data;
    }

    public static class DashboardAppCreator
    {
        private const string StreamlitProgramName = "streamlit";

        #region Service Start
        public static DashboardSettings ConfigureDashboard(string title)
        {
            return new DashboardSettings 
            {
                Title = title 
            };
        }
        public static string ConfigureService(ChartData[] charts, DashboardSettings? settings)
        {
            ValidateDependencies();
            settings ??= new();

            // Generate bootstrap script
            string workingDirectory = FileSystem.GetTempFolderPath();
            string script = GenerateScript(charts, workingDirectory, settings);
            string scriptFile = Path.Combine(workingDirectory, "Script.py");
            File.WriteAllText(scriptFile, script);

            // Run background service
            string? streamlit = EnvironmentVariableHelper.FindProgram(StreamlitProgramName);
            if (streamlit != null)
                ProcessHelper.StartProcessInBackground(true, streamlit, workingDirectory, "run", Path.GetFileName(scriptFile));

            // Should return address
            return string.Empty;
        }
        #endregion

        #region Declarative Charts
        public static ChartData LineChart(DataGrid data)
        {
            return new ChartData(ChartType.LineChart, data);
        }
        #endregion

        #region Routines
        private static string GenerateScript(ChartData[] charts, string workingDirectory, DashboardSettings settings)
        {
            return $$""""
                import streamlit as st
                import pandas as pd
                import numpy as np

                # Data-Driven Construction Routines
                def line_chart(file_path): # Expect format: string header, numerical body, sequential data
                    chart_data = pd.read_csv(file_path)
                    st.line_chart(chart_data)

                def demo_section():
                    DATE_COLUMN = 'date/time'
                    DATA_URL = ('https://s3-us-west-2.amazonaws.com/'
                                'streamlit-demo-data/uber-raw-data-sep14.csv.gz')
                
                    @st.cache_data
                    def load_data(nrows):
                        data = pd.read_csv(DATA_URL, nrows=nrows)
                        lowercase = lambda x: str(x).lower()
                        data.rename(lowercase, axis='columns', inplace=True)
                        data[DATE_COLUMN] = pd.to_datetime(data[DATE_COLUMN])
                        return data
                
                    data_load_state = st.text('Loading data...')
                    data = load_data(10000)
                    data_load_state.text("Done! (using st.cache_data)")
                
                    if st.checkbox('Show raw data'):
                        st.subheader('Raw data')
                        st.write(data)
                
                    st.subheader('Number of pickups by hour')
                    hist_values = np.histogram(data[DATE_COLUMN].dt.hour, bins=24, range=(0,24))[0]
                    st.bar_chart(hist_values)
                
                    # Some number in the range 0-23
                    hour_to_filter = st.slider('hour', 0, 23, 17)
                    filtered_data = data[data[DATE_COLUMN].dt.hour == hour_to_filter]
                
                    st.subheader('Map of all pickups at %s:00' % hour_to_filter)
                    st.map(filtered_data)

                # Entrance
                def setup():
                    # Initial setup
                    st.title("{{settings.Title}}")

                    # Remove annoying Deploy button
                    st.markdown("""
                        <style>
                            .reportview-container {
                                margin-top: -2em;
                            }
                            #MainMenu {visibility: hidden;}
                            .stDeployButton {display:none;}
                            footer {visibility: hidden;}
                            #stDecoration {display:none;}
                        </style>
                    """, unsafe_allow_html=True)

                def show():
                    {{string.Join("\n    ", charts.Select(CreateChart))}}

                setup()
                show()
                """";

            string CreateChart(ChartData chart)
            {
                switch (chart.ChartType)
                {
                    case ChartType.LineChart:
                        return $"line_chart(r\"{SerializeData(chart.Data)}\")";
                    default:
                        throw new ArgumentException($"Unknown chart type: {chart.ChartType}");
                }
            }
            string SerializeData(DataGrid data)
            {
                string tempDataPath = Path.Combine(workingDirectory, $"{data.TableName}.csv");
                data.Save(tempDataPath);
                return tempDataPath;
            }
        }
        private static void ValidateDependencies()
        {
            // Check python availability
            string? python = EnvironmentVariableHelper.FindProgram(PythonRuntimeHelper.PythonExecutableName)
                ?? throw new FileNotFoundException($"Cannot find python on current computer.");

            // Check python version
            string? version = PythonRuntimeHelper.GetPythonVersion();
            string[] validMajorVersions = ["3.10", "3.11", "3.12", "3.13"];
            if (!validMajorVersions.Any(version.Contains))
                throw new InvalidProgramException($"Expects a python version in: {string.Join(", ", validMajorVersions)}");

            // Check pip availability
            string? pip = EnvironmentVariableHelper.FindProgram(PythonRuntimeHelper.PipExecutableName)
                ?? throw new FileNotFoundException($"Cannot find pip on current computer.");

            // Check streamlit availability
            string? streamlit = PythonRuntimeHelper.GetModuleVersion("streamlit");
            if (!streamlit.StartsWith("1.37"))
                throw new InvalidProgramException("Expect streamlit 1.37+.");

            // Check pythonet availability
            string? pythonnet = PythonRuntimeHelper.GetModuleVersion("pythonnet");
            if (!pythonnet.StartsWith("3.0"))
                throw new InvalidProgramException("Expect streamlit 3.0+.");
        }
        #endregion
    }
}
