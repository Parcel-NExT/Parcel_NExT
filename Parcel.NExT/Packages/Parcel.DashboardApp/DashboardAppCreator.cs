using Parcel.CoreEngine.Helpers;
using Parcel.NExT.Python.Helpers;
using Parcel.Standard.System;

namespace Parcel.Framework.Dashboard
{
    public static class DashboardAppCreator
    {
        private const string StreamlitProgramName = "streamlit";

        #region Service Start
        public static string ConfigureService()
        {
            ValidateDependencies();

            string workingDirectory = FileSystem.GetTempFolderPath();
            string script = GenerateScript(workingDirectory);

            string? streamlit = EnvironmentVariableHelper.FindProgram(StreamlitProgramName);
            if (streamlit != null)
                ProcessHelper.StartProcessInBackground(true, streamlit, workingDirectory, "run", Path.GetFileName(script));

            return string.Empty; // Should return address
        }
        #endregion

        #region Routines
        private static string GenerateScript(string workingDirectory)
        {
            string path = Path.Combine(workingDirectory, "Script.py");
            File.WriteAllText(path, """"
                import streamlit as st
                import pandas as pd
                import numpy as np

                st.title('Uber pickups in NYC')

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
                """");
            return path;
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

            // Check bpy availability
            string? streamlit = PythonRuntimeHelper.GetModuleVersion("streamlit");
            if (!streamlit.StartsWith("1.37"))
                throw new InvalidProgramException("Expect streamlit 1.37+.");
        }
        #endregion
    }
}
